using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates.DirectionalM1
{
    public class BaseDirectionalM1 : BaseInputEvaluation
    {
        public Vector3 launchTarget;

        public bool launchVectorOverride;

        private bool jumpCancelled;

        private bool crit;

        private List<HealthComponent> hits;

        protected Vector3 slideVector;

        protected Quaternion slideRotation;

        private bool inAir;

        private Vector3 dashDirection;

        protected bool canRecieveInput = true;

        protected bool isDash = false;

        protected bool isCombo = false;

        protected bool isAerial = false;

        protected bool isSus = false;

        protected bool isFlinch = false;

        protected AnimationCurve dashSpeedCurve;

        protected float earlyExitTime;

        protected float anim = 1f;

        protected string animString = "Jab1";

        protected string hitboxName = "Jab";

        protected DamageType damageType = DamageType.Generic;

        protected float damageCoefficient = 2.5f;

        protected float procCoefficient = 1f;

        protected float pushForce = 300f;

        protected Vector3 bonusForce = Vector3.zero;

        protected float baseDuration = 0.3f;

        protected float attackStartTime = 0f;

        protected float attackEndTime = 0.4f;

        protected float hitStopDuration = 0.06f;

        protected float attackRecoil = 2f;

        protected float hitHopVelocity = 2f;

        protected bool cancelled = false;

        protected string swingSoundString = "";

        protected string hitSoundString = "";

        protected string muzzleString = "";

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

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
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
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == hitboxName);
            }
            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = hitEffectPrefab;
            if (launchVectorOverride)
            {
                attack.forceVector = Vector3.zero;
                attack.pushAwayForce = 0f;
            }
            else
            {
                attack.forceVector = bonusForce;
                attack.pushAwayForce = pushForce;
            }
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            attack.impactSound = impactSound;
        }

        private void StartAttack()
        {
            characterBody.SetAimTimer(duration);
            animator.SetBool("attacking", value: true);
            characterDirection.forward = inputBank.aimDirection;
            PlayAnimation("FullBody, Override", animString, "Slash.playbackRate", duration * anim);
            if (!isAerial)
            {
                if (isDash)
                {
                    characterMotor.moveDirection = Vector3.zero;
                }
                else
                {
                    characterMotor.moveDirection = Vector3.zero;
                }
            }
            if (isDash && Util.HasEffectiveAuthority(gameObject))
            {
                characterMotor.velocity *= 0.2f;
                SetSlideVector();
            }
        }

        public virtual void SetSlideVector()
        {
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
            if (!inHitPause && hitStopDuration > 0f && !jumpCancelled)
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

        public virtual void PlayHitSound()
        {
            Util.PlaySound(hitSoundString, gameObject);
        }

        private void FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                Util.PlayAttackSpeedSound(swingSoundString, gameObject, attackSpeedStat);
                if (Util.HasEffectiveAuthority(gameObject))
                {
                    PlaySwingEffect();
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
                if (component2.teamIndex == teamComponent.teamIndex || hits.Contains(healthComponent))
                {
                    continue;
                }
                hits.Add(healthComponent);
                HealthComponent healthComponent2 = healthComponent;
                if ((bool)healthComponent2)
                {
                    if (isSus && (bool)healthComponent2.body && (bool)healthComponent2.body.characterMotor)
                    {
                        healthComponent2.body.characterMotor.velocity.y = 4f;
                    }
                    if (isFlinch && (bool)healthComponent2.body)
                    {
                        ForceFlinch(healthComponent2.body);
                    }
                    if (launchVectorOverride && !healthComponent2.body.isChampion || healthComponent2.gameObject.name.Contains("Brother") && healthComponent2.gameObject.name.Contains("Body"))
                    {
                        LaunchEnemy(healthComponent2.body);
                    }
                }
            }
        }

        public virtual void LaunchEnemy(CharacterBody body)
        {
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
            if (canRecieveInput)
            {
                EvaluateInput();
            }
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
            if (isDash && (bool)characterMotor && !inHitPause)
            {
                float num = dashSpeedCurve.Evaluate(stopwatch / duration);
                float num2 = !hasHopped ? 1f : 0.65f;
                characterMotor.rootMotion += 0.6f * num2 * (slideRotation * (num * moveSpeedStat * slideVector * Time.fixedDeltaTime));
            }
            if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
            {
                FireAttack();
            }
            if (stopwatch >= duration * earlyExitTime && nextState != null)
            {
                SetNextState();
            }
            if (stopwatch >= duration)
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
