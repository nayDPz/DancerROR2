using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dancer.SkillStates.DirectionalM1
{

    public class AttackDown2 : BaseInputEvaluation
    {
        public Vector3 launchTarget;

        public bool launchVectorOverride;

        public bool cancelledFromSprinting;

        public bool earlyExitJump;

        public string critHitSoundString;

        private bool crit;

        private List<HealthComponent> hits;

        protected float attackResetInterval = 0.115f;

        private float attackResetStopwatch;

        public int swingIndex;

        protected string animString = "DownAir";

        protected string hitboxName = "DownAir";

        protected DamageType damageType = DamageType.Generic;

        protected float damageCoefficient = 1.25f;

        protected float procCoefficient = 0.5f;

        protected float pushForce = 500f;

        protected float baseDuration = 1.3f;

        protected float attackStartTime = 0f;

        protected float attackEndTime = 0.9f;

        protected float hitStopDuration = 0.12f;

        protected float attackRecoil = 2f;

        protected float hitHopVelocity = 0f;

        protected bool cancelled = false;

        protected string swingSoundString = "";

        protected string hitSoundString = "";

        private float fallSpeed = 3.75f;

        private bool hitGround;

        protected string muzzleString = "eDAir";

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

        private bool hasHopped;

        protected float stopwatch;

        protected Animator animator;

        private HitStopCachedState hitStopCachedState;

        private Vector3 storedVelocity;

        protected float anim = 1f;

        protected Vector3 slideVector;

        protected Quaternion slideRotation;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            if ((bool)modelLocator)
            {
                ChildLocator component = modelLocator.modelTransform.GetComponent<ChildLocator>();
                if ((bool)component)
                {
                    muzzleTransform = component.FindChild("eDAir");
                }
            }
            characterMotor.velocity.y = 0f;
            AttackSetup();
            StartAttack();
        }

        private void AttackSetup()
        {
            hits = new List<HealthComponent>();
            duration = baseDuration / attackSpeedStat;
            attackResetInterval /= attackSpeedStat;
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
            attack.hitEffectPrefab = Modules.Assets.stabHitEffect;
            attack.forceVector = Vector3.down * pushForce;
            attack.pushAwayForce = 0f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            attack.impactSound = Modules.Assets.sword1HitSoundEvent.index;
            swingEffectPrefab = Modules.Assets.downAirEffect;
            muzzleString = "eDAir";
        }

        private void StartAttack()
        {
            characterBody.SetAimTimer(duration);
            Util.PlayAttackSpeedSound("DancerSwing1", gameObject, attackSpeedStat);
            animator.SetBool("attacking", value: true);
            characterDirection.forward = inputBank.aimDirection;
            PlayAnimation("FullBody, Override", "DAir", "Slash.playbackRate", 0.75f);
        }

        public virtual void PlayHitSound()
        {
            Util.PlaySound("SwordHit", gameObject);
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
            swingEffect = UnityEngine.Object.Instantiate(swingEffectPrefab, muzzleTransform);
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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            bool flag = true;
            hitPauseTimer -= Time.fixedDeltaTime;
            if (hitPauseTimer <= 0f && inHitPause)
            {
                animator.SetFloat("Slash.playbackRate", 2f);
                ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
                inHitPause = false;
                characterMotor.velocity = storedVelocity;
            }
            if (!inHitPause)
            {
                attackResetStopwatch += Time.fixedDeltaTime;
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
            inputBank.moveVector /= 2f;
            if ((bool)characterMotor && !inHitPause)
            {
                characterMotor.rootMotion += fallSpeed * moveSpeedStat * Vector3.down * Time.fixedDeltaTime;
            }
            if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
            {
                if (attackResetStopwatch >= attackResetInterval)
                {
                    attack.ResetIgnoredHealthComponents();
                    attackResetStopwatch = 0f;
                }
                FireAttack();
            }
            if (characterMotor.isGrounded)
            {
                hitGround = true;
                outer.SetNextState(new AttackDown2End());
            }
            else if (stopwatch >= duration)
            {
                PlayCrossfade("FullBody, Override", "BufferEmpty", "Slash.playbackRate", duration * anim, 0.01f);
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnExit()
        {
            if ((bool)swingEffect)
            {
                UnityEngine.Object.Destroy(swingEffect);
            }
            GetAimAnimator().enabled = true;
            animator.SetFloat("Slash.playbackRate", 1f);
            base.OnExit();
        }
    }
}
