using EntityStates;
using KinematicCharacterController;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates
{

    public class SpinDashEnd : BaseSkillState
    {
        public Vector3 launchTarget;

        private bool crit;

        private List<HealthComponent> hits;

        private float attackResetStopwatch;

        public int swingIndex;

        protected string animString = "DownAirGround";

        protected string hitboxName = "DownAirGround";

        protected DamageType damageType = DamageType.Generic;

        protected float damageCoefficient = 2f;

        protected float procCoefficient = 1f;

        protected float pushForce = 1900f;

        protected float baseDuration = 0.35f;

        protected float attackStartTime = 0f;

        protected float attackEndTime = 1f;

        protected float hitStopDuration = 0.09f;

        protected float attackRecoil = 2f;

        protected float hitHopVelocity = 0f;

        protected bool cancelled = false;

        protected string swingSoundString = "";

        protected string hitSoundString = "";

        protected string muzzleString = "SwingCenter";

        protected GameObject swingEffectPrefab;

        protected GameObject hitEffectPrefab;

        protected NetworkSoundEventIndex impactSound;

        public float duration;

        private bool hasFired;

        private float hitPauseTimer;

        protected OverlapAttack attack;

        protected bool inHitPause;

        private bool hasHopped;

        protected float stopwatch;

        protected Animator animator;

        private HitStopCachedState hitStopCachedState;

        private Vector3 storedVelocity;

        protected float anim = 1f;

        private bool jumpCancelled;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            impactSound = Modules.Assets.sword2HitSoundEvent.index;
            AttackSetup();
            StartAttack();
        }

        private void AttackSetup()
        {
            hits = new List<HealthComponent>();
            duration = baseDuration / attackSpeedStat;
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxName);
            }
            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = Modules.Assets.hitEffect;
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 0f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            attack.impactSound = impactSound;
            swingEffectPrefab = Modules.Assets.downAirEndEffect;
            muzzleString = "eDAirEnd";
            swingSoundString = "PunchSwing";
        }

        private void StartAttack()
        {
            base.characterBody.SetAimTimer(duration);
            Util.PlayAttackSpeedSound("DancerDownTilt", base.gameObject, attackSpeedStat);
            animator.SetBool("attacking", value: true);
            base.characterDirection.forward = base.inputBank.aimDirection;
            PlayAnimation("FullBody, Override", "DAirGround", "Slash.playbackRate", duration * 1.4f);
        }

        public virtual void OnHitEnemyAuthority(List<HurtBox> list)
        {
            Util.PlaySound("SwordHit2", base.gameObject);
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = base.characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(base.characterMotor, animator, "Slash.playbackRate");
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }

        private void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, base.gameObject, muzzleString, transmit: true);
        }

        private void FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                if (Util.HasEffectiveAuthority(base.gameObject))
                {
                    PlaySwingEffect();
                    Util.PlayAttackSpeedSound(swingSoundString, base.gameObject, attackSpeedStat);
                    AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                }
            }
            List<HurtBox> list = new List<HurtBox>();
            if (Util.HasEffectiveAuthority(base.gameObject) && attack.Fire(list))
            {
                OnHitEnemyAuthority(list);
            }
            if (!NetworkServer.active)
            {
                return;
            }
            Transform transform = FindModelChild("DAirGround");
            Vector3 position = transform.position;
            Vector3 halfExtents = transform.localScale * 0.5f;
            Quaternion rotation = transform.rotation;
            Collider[] array = Physics.OverlapBox(position, halfExtents, rotation, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox component = array[i].GetComponent<HurtBox>();
                if (!component)
                {
                    continue;
                }
                HealthComponent healthComponent = component.healthComponent;
                if (!healthComponent)
                {
                    continue;
                }
                TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
                if (component2.teamIndex == base.teamComponent.teamIndex || hits.Contains(healthComponent))
                {
                    continue;
                }
                hits.Add(healthComponent);
                HealthComponent healthComponent2 = healthComponent;
                if ((bool)healthComponent2)
                {
                    if ((bool)healthComponent2.body && (bool)healthComponent2.body.characterMotor)
                    {
                        healthComponent2.body.characterMotor.velocity = Vector3.zero;
                    }
                    if (!healthComponent2.body.isChampion || (healthComponent2.gameObject.name.Contains("Brother") && healthComponent2.gameObject.name.Contains("Body")))
                    {
                        LaunchEnemy(healthComponent2.body);
                    }
                }
            }
        }

        public void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = Vector3.up * 15f;
            Vector3 normalized = (vector + base.transform.position - body.transform.position).normalized;
            normalized *= pushForce;
            if ((bool)body.GetComponent<KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            CharacterMotor characterMotor = body.characterMotor;
            float num = 0.25f;
            if ((bool)characterMotor)
            {
                float num2 = Mathf.Max(100f, characterMotor.mass);
                num = num2 / 100f;
                normalized *= num;
                characterMotor.ApplyForce(normalized);
            }
            else if ((bool)body.rigidbody)
            {
                float num3 = Mathf.Max(50f, body.rigidbody.mass);
                num = num3 / 200f;
                normalized *= num;
                body.rigidbody.AddForce(normalized, ForceMode.Impulse);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.inputBank.moveVector = Vector3.zero;
            hitPauseTimer -= Time.fixedDeltaTime;
            if (hitPauseTimer <= 0f && inHitPause)
            {
                ConsumeHitStopCachedState(hitStopCachedState, base.characterMotor, animator);
                inHitPause = false;
                base.characterMotor.velocity = storedVelocity;
            }
            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if ((bool)base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;
                }
                if ((bool)animator)
                {
                    animator.SetFloat("Slash.playbackRate", 0f);
                }
            }
            if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
            {
                FireAttack();
            }
            else if (stopwatch >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            if (cancelled)
            {
                PlayAnimation("FullBody, Override", "BufferEmpty");
            }
            GetAimAnimator().enabled = true;
            animator.SetFloat("Slash.playbackRate", 1f);
            base.OnExit();
        }
    }
}