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

    public class SpinDash : BaseSkillState
    {
        public Vector3 launchTarget;

        public bool launchVectorOverride;

        public bool cancelledFromSprinting;

        public bool earlyExitJump;

        public string critHitSoundString;

        private List<HealthComponent> hits;

        private int numResets = 2;

        private int timesReset = 0;

        protected float attackResetInterval;

        private float attackResetStopwatch;

        public int swingIndex;

        protected string hitboxName = "SpinLunge";

        protected DamageType damageType = DamageType.Generic;

        protected float damageCoefficient = 5.4f;

        protected float procCoefficient = 0.75f;

        protected float pushForce = 3000f;

        protected float baseDuration = 0.55f;

        protected float attackStartTime = 0.15f;

        protected float attackEndTime = 0.9f;

        protected float hitStopDuration = 0.05f;

        protected float attackRecoil = 2f;

        protected float hitHopVelocity = 0f;

        protected bool cancelled = false;

        protected string swingSoundString = "";

        protected string hitSoundString = "";

        private float speedCoefficient = 35f;

        private bool hitGround;

        protected string muzzleString = "eDashAttack";

        private Transform muzzleTransform;

        protected GameObject swingEffectPrefab;

        private GameObject swingEffect;

        protected GameObject hitEffectPrefab;

        protected NetworkSoundEventIndex impactSound;

        public float duration;

        private bool hasFired;

        private float hitPauseTimer;

        protected OverlapAttack attack;

        protected bool inHitPause;

        protected float stopwatch;

        protected Animator animator;

        private Vector3 moveVector;

        private HitStopCachedState hitStopCachedState;

        private Vector3 storedVelocity;

        private DancerComponent dancerComponent;

        private bool reset = true;

        protected float anim = 1f;

        protected Vector3 slideVector;

        protected Quaternion slideRotation;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            if ((bool)base.modelLocator)
            {
                ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
                if ((bool)component)
                {
                    muzzleTransform = component.FindChild("eDashAttack");
                }
            }
            dancerComponent = base.characterBody.GetComponent<DancerComponent>();
            moveVector = base.inputBank.aimDirection;
            moveVector.y = Mathf.Clamp(moveVector.y, -0.2f, 0.2f);
            moveVector = moveVector.normalized;
            base.characterMotor.velocity.y = 0f;
            AttackSetup();
            StartAttack();
        }

        private void AttackSetup()
        {
            hits = new List<HealthComponent>();
            duration = baseDuration / attackSpeedStat;
            float num = duration * attackEndTime - duration * attackStartTime;
            attackResetInterval = num / (float)numResets;
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxName);
            }
            damageCoefficient = 5.4f / (float)(numResets + 1);
            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = Modules.Assets.stabHitEffect;
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 0f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            attack.impactSound = Modules.Assets.sword1HitSoundEvent.index;
            swingEffectPrefab = Modules.Assets.downAirEffect;
            muzzleString = "eDashAttack";
        }

        private void StartAttack()
        {
            base.characterBody.SetAimTimer(duration);
            Util.PlayAttackSpeedSound("DancerSwing1", base.gameObject, attackSpeedStat);
            animator.SetBool("attacking", value: true);
            base.characterDirection.forward = base.inputBank.aimDirection;
            PlayAnimation("FullBody, Override", "Drill", "DragonLunge.playbackRate", 1.1f);
        }

        public virtual void PlayHitSound()
        {
            Util.PlaySound("SwordHit", base.gameObject);
        }

        public virtual void OnHitEnemyAuthority(List<HurtBox> list)
        {
            PlayHitSound();
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = base.characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(base.characterMotor, animator, "DragonLunge.playbackRate");
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }

        private void PlaySwingEffect()
        {
            swingEffect = UnityEngine.Object.Instantiate(swingEffectPrefab, muzzleTransform);
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
            if (!reset)
            {
                return;
            }
            reset = false;
            if (!NetworkServer.active)
            {
                return;
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
                if (component2.teamIndex != base.teamComponent.teamIndex && !hits.Contains(healthComponent))
                {
                    hits.Add(healthComponent);
                    HealthComponent healthComponent2 = healthComponent;
                    if ((bool)healthComponent2 && (!healthComponent2.body.isChampion || (healthComponent2.gameObject.name.Contains("Brother") && healthComponent2.gameObject.name.Contains("Body"))))
                    {
                        LaunchEnemy(healthComponent2.body);
                    }
                }
            }
        }

        private void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = moveVector * 50f;
            Vector3 normalized = (vector + base.transform.position - body.transform.position).normalized;
            normalized *= pushForce;
            if (timesReset == numResets)
            {
                normalized /= 4f;
            }
            if ((bool)body.GetComponent<KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            CharacterMotor characterMotor = body.characterMotor;
            float num = 0.25f;
            if ((bool)characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                float num2 = Mathf.Max(100f, characterMotor.mass);
                num = num2 / 100f;
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.inputBank.moveVector = Vector3.zero;
            base.characterMotor.moveDirection = Vector3.zero;
            hitPauseTimer -= Time.fixedDeltaTime;
            if (hitPauseTimer <= 0f && inHitPause)
            {
                animator.SetFloat("DragonLunge.playbackRate", 1f);
                ConsumeHitStopCachedState(hitStopCachedState, base.characterMotor, animator);
                inHitPause = false;
                base.characterMotor.velocity = storedVelocity;
            }
            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= duration * attackStartTime)
                {
                    attackResetStopwatch += Time.fixedDeltaTime;
                }
            }
            else
            {
                if ((bool)base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;
                }
                if ((bool)animator)
                {
                    animator.SetFloat("DragonLunge.playbackRate", 0f);
                }
            }
            if ((bool)base.characterMotor && !inHitPause)
            {
                float num = Mathf.Lerp(speedCoefficient * 1.25f, speedCoefficient * 0.75f, stopwatch / duration);
                base.characterMotor.rootMotion += num * moveVector * Time.fixedDeltaTime;
                base.characterMotor.velocity.y = 0f;
                base.characterDirection.forward = moveVector;
            }
            if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
            {
                if (attackResetStopwatch >= attackResetInterval)
                {
                    reset = true;
                    timesReset++;
                    attack.ResetIgnoredHealthComponents();
                    attackResetStopwatch = 0f;
                }
                if (timesReset <= numResets)
                {
                    FireAttack();
                }
            }
            if (stopwatch >= duration - attackResetInterval / 2f)
            {
                SmallHop(base.characterMotor, base.characterBody.jumpPower);
                outer.SetNextState(new SpinDashEnd());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            if ((bool)dancerComponent)
            {
                dancerComponent.StopBodyOverride();
            }
            if ((bool)swingEffect)
            {
                UnityEngine.Object.Destroy(swingEffect);
            }
            GetAimAnimator().enabled = true;
            animator.SetFloat("DragonLunge.playbackRate", 1f);
            base.OnExit();
        }
    }
}