using EntityStates;
using KinematicCharacterController;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates.M1
{

    public class UpAir : BaseSkillState
    {
        public Vector3 launchTarget;

        public string critHitSoundString;

        private bool crit;

        private bool secondAttack;

        private List<HealthComponent> hits;

        protected string animString = "UpAir";

        protected string hitboxName = "UpAir";

        protected DamageType damageType = DamageType.Generic;

        protected float damageCoefficient = 1.5f;

        protected float procCoefficient = 1f;

        protected float pushForce = 2200f;

        protected Vector3 bonusForce = Vector3.zero;

        protected float baseDuration = 0.8f;

        protected float attackStartTime = 0.1f;

        protected float attackEndTime = 0.8f;

        protected float attackResetTime = 0.575f;

        protected float hitStopDuration = 0.06f;

        protected float attackRecoil = 2f;

        protected float hitHopVelocity = 2f;

        protected bool cancelled = false;

        protected string swingSoundString = "";

        protected string hitSoundString = "";

        protected string muzzleString = "eUpAir1";

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

        private bool a = false;

        public override void OnEnter()
        {
            base.OnEnter();
            SmallHop(characterMotor, 9f);
            animator = GetModelAnimator();
            AttackSetup();
            StartAttack();
        }

        private void AttackSetup()
        {
            hits = new List<HealthComponent>();
            duration = baseDuration / attackSpeedStat;
            crit = RollCrit();
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == hitboxName);
            }
            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 0f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = crit;
            attack.impactSound = Modules.Assets.sword2HitSoundEvent.index;
            swingSoundString = "DancerSwing1";
            swingEffectPrefab = Modules.Assets.swingEffect;
            hitEffectPrefab = Modules.Assets.hitEffect;
        }

        private void StartAttack()
        {
            characterBody.SetAimTimer(duration);
            animator.SetBool("attacking", value: true);
            characterDirection.forward = inputBank.aimDirection;
            PlayAnimation("FullBody, Override", animString, "Slash.playbackRate", duration * anim);
        }

        public virtual void PlayHitSound()
        {
            if (secondAttack)
            {
                Util.PlaySound("SwordHit3", gameObject);
            }
            else
            {
                Util.PlaySound("SwordHit2", gameObject);
            }
        }

        public virtual void OnHitEnemyAuthority(List<HurtBox> list)
        {
            PlayHitSound();
            if (!hasHopped)
            {
                if ((bool)characterMotor && !characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    SmallHop(characterMotor, hitHopVelocity);
                }
                hasHopped = true;
            }
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, "Slash.playbackRate");
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }

        private void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, muzzleString, transmit: true);
        }

        private void FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                if (Util.HasEffectiveAuthority(gameObject))
                {
                    PlaySwingEffect();
                    Util.PlayAttackSpeedSound(swingSoundString, gameObject, attackSpeedStat);
                    AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                }
            }
            List<HurtBox> list = new List<HurtBox>();
            if (Util.HasEffectiveAuthority(gameObject) && attack.Fire(list))
            {
                OnHitEnemyAuthority(list);
            }
            if (!NetworkServer.active)
            {
                return;
            }
            if (!a)
            {
                a = true;
            }
            Transform transform = FindModelChild(hitboxName);
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
                if (component2.teamIndex != teamComponent.teamIndex && !hits.Contains(healthComponent))
                {
                    hits.Add(healthComponent);
                    HealthComponent healthComponent2 = healthComponent;
                    if ((bool)healthComponent2 && (!healthComponent2.body.isChampion || healthComponent2.gameObject.name.Contains("Brother") && healthComponent2.gameObject.name.Contains("Body")))
                    {
                        LaunchEnemy(healthComponent2.body);
                    }
                }
            }
        }

        public void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = Vector3.up * (secondAttack ? 40f : 10f);
            Vector3 normalized = (vector + transform.position - body.transform.position).normalized;
            normalized *= pushForce;
            if ((bool)body.GetComponent<KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            CharacterMotor characterMotor = body.characterMotor;
            float num = 0.25f;
            if ((bool)characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                float num2 = Mathf.Max(150f, characterMotor.mass);
                num = num2 / 150f;
                normalized *= num;
                characterMotor.ApplyForce(normalized);
            }
            else if ((bool)body.rigidbody)
            {
                body.rigidbody.velocity = Vector3.zero;
                float num3 = Mathf.Max(50f, body.rigidbody.mass);
                num = num3 / 200f;
                normalized *= num;
                body.rigidbody.AddForce(normalized, ForceMode.Impulse);
            }
        }

        protected virtual void ForceFlinch(CharacterBody body)
        {
            SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
            if ((bool)component && component.canBeHitStunned && Util.HasEffectiveAuthority(body.gameObject))
            {
                component.SetPain();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float num = Mathf.Clamp01(fixedAge / duration * attackResetTime) * 90f;
            hitPauseTimer -= Time.fixedDeltaTime;
            if (hitPauseTimer <= 0f && inHitPause)
            {
                ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
                inHitPause = false;
                characterMotor.velocity = storedVelocity;
            }
            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if ((bool)characterMotor)
                {
                    characterMotor.velocity = Vector3.zero;
                }
                if ((bool)animator)
                {
                    animator.SetFloat("Slash.playbackRate", 0f);
                }
            }
            if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
            {
                if (stopwatch >= duration * attackResetTime && !secondAttack)
                {
                    damageCoefficient = 2f;
                    hasFired = false;
                    secondAttack = true;
                    hitboxName = "UpAir2";
                    pushForce = 1200f;
                    hitStopDuration = 0.125f;
                    muzzleString = "eUpAir2";
                    swingSoundString = "ForwardAirStart";
                    swingEffectPrefab = Modules.Assets.dashAttackEffect;
                    attack = new OverlapAttack();
                    attack.damageType = DamageType.BonusToLowHealth;
                    attack.attacker = gameObject;
                    attack.inflictor = gameObject;
                    attack.teamIndex = GetTeam();
                    attack.damage = damageCoefficient * damageStat;
                    attack.procCoefficient = procCoefficient;
                    hitEffectPrefab = Modules.Assets.stabHitEffect;
                    attack.impactSound = Modules.Assets.sword3HitSoundEvent.index;
                    attack.forceVector = Vector3.zero;
                    attack.pushAwayForce = 0f;
                    HitBoxGroup hitBoxGroup = null;
                    Transform modelTransform = GetModelTransform();
                    if ((bool)modelTransform)
                    {
                        hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == hitboxName);
                    }
                    attack.hitBoxGroup = hitBoxGroup;
                    attack.isCrit = crit;
                    attack.impactSound = Modules.Assets.sword3HitSoundEvent.index;
                    attack.ResetIgnoredHealthComponents();
                }
                FireAttack();
            }
            else if (stopwatch >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnExit()
        {
            if (!hasFired)
            {
                FireAttack();
            }
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