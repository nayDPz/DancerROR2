using EntityStates;
using KinematicCharacterController;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates
{

    public class SpinnyMove : BaseSkillState
    {
        public static float cooldownReductionOnHit;

        private Animator animator;

        private HitStopCachedState hitStopCachedState;

        private Vector3 storedVelocity;

        protected float hitStopDuration = 0.09f;

        private bool inHitPause;

        private float hitPauseTimer;

        private float duration;

        private float windupDuration = 0.08f;

        private float baseAttackInterval = 0.3f;

        private float attackInterval;

        private int numAttacks = 3;

        private int attacksFired;

        private float stopwatch;

        private GameObject swingEffectPrefab;

        private bool groundStart;

        private AnimationCurve dashSpeedCurve = new AnimationCurve(new Keyframe(0f, 5f), new Keyframe(0.35f, 0.5f), new Keyframe(1f, 0.5f));

        private Vector3 moveDirection;

        private float procCoefficient = 1f;

        private float damageCoefficient = 2.5f;

        private float pushForce = 2200f;

        private float attackRadius = 5.5f;

        private bool crit;

        public override void OnEnter()
        {
            animator = GetModelAnimator();
            StartAimMode(2f, snap: true);
            float attackSpeed = base.characterBody.attackSpeed;
            windupDuration /= attackSpeed;
            attackInterval = baseAttackInterval / attackSpeed;
            duration = attackInterval * (float)numAttacks + windupDuration;
            stopwatch = attackInterval;
            EntityStateMachine[] components = base.gameObject.GetComponents<EntityStateMachine>();
            foreach (EntityStateMachine entityStateMachine in components)
            {
                if (entityStateMachine.customName == "Weapon")
                {
                    entityStateMachine.SetNextStateToMain();
                }
            }
            groundStart = base.isGrounded;
            PlayAnimation("FullBody, Override", "Secondary", "Spinny.playbackRate", duration * 1.4f);
            moveDirection = base.inputBank.aimDirection;
            base.OnEnter();
        }

        private void Fire()
        {
            Vector3 position = moveDirection * (attackRadius - 1f) + base.transform.position;
            PlaySwingEffect();
            if (Util.HasEffectiveAuthority(base.gameObject))
            {
                attacksFired++;
                if (attacksFired >= numAttacks)
                {
                    pushForce = 1100f;
                }
                if (new BlastAttack
                {
                    attacker = base.gameObject,
                    procChainMask = default(ProcChainMask),
                    impactEffect = Modules.Assets.bigHitEffect.GetComponent<EffectComponent>().effectIndex,
                    losType = BlastAttack.LoSType.NearestHit,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    procCoefficient = procCoefficient,
                    bonusForce = Vector3.zero,
                    baseForce = 0f,
                    baseDamage = damageCoefficient * damageStat,
                    falloffModel = BlastAttack.FalloffModel.None,
                    radius = attackRadius,
                    position = position,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    teamIndex = GetTeam(),
                    inflictor = base.gameObject,
                    crit = RollCrit()
                }.Fire().hitCount > 0)
                {
                    OnHitEnemyAuthority();
                    Util.PlaySound("SwordHit2", base.gameObject);
                }
            }
            if (!NetworkServer.active)
            {
                return;
            }
            List<HealthComponent> list = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, attackRadius, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
                if (component2.teamIndex != base.teamComponent.teamIndex && !list.Contains(healthComponent))
                {
                    list.Add(healthComponent);
                    HealthComponent healthComponent2 = healthComponent;
                    if ((bool)healthComponent2 && (!healthComponent2.body.isChampion || (healthComponent2.gameObject.name.Contains("Brother") && healthComponent2.gameObject.name.Contains("Body"))))
                    {
                        LaunchEnemy(healthComponent2.body);
                    }
                }
            }
        }

        private void PlaySwingEffect()
        {
            string soundString = "Jab1";
            switch (attacksFired)
            {
                case 0:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.bigSwingEffect, base.gameObject, "eJab1", transmit: false);
                    soundString = "PunchSwing";
                    break;
                case 1:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.bigSwingEffect, base.gameObject, "eSecondary2", transmit: false);
                    soundString = "SwordSwing2";
                    break;
                case 2:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.bigSwingEffect, base.gameObject, "eFAir", transmit: false);
                    soundString = "SwordSwing3";
                    break;
                default:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.bigSwingEffect, base.gameObject, "eJab1", transmit: false);
                    break;
            }
            Util.PlayAttackSpeedSound(soundString, base.gameObject, attackSpeedStat);
        }

        private void OnHitEnemyAuthority()
        {
            if (base.activatorSkillSlot.rechargeStopwatch >= 0f)
            {
                base.activatorSkillSlot.rechargeStopwatch += cooldownReductionOnHit;
            }
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = base.characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(base.characterMotor, animator, "Spinny.playbackRate");
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }

        private void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = moveDirection * 50f;
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
            if (!(base.fixedAge > windupDuration))
            {
                return;
            }
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
                    animator.SetFloat("Spinny.playbackRate", 0f);
                }
            }
            if ((bool)base.characterMotor && attacksFired < numAttacks && !inHitPause)
            {
                float num = dashSpeedCurve.Evaluate(stopwatch / duration);
                if (base.isGrounded && !groundStart)
                {
                    moveDirection.y = 0f;
                    moveDirection = moveDirection.normalized;
                }
                base.characterDirection.forward = moveDirection;
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion += num * 7f * moveDirection * Time.fixedDeltaTime;
            }
            if (stopwatch >= attackInterval && attacksFired < numAttacks)
            {
                Fire();
                stopwatch = 0f;
            }
            if (attacksFired >= numAttacks)
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
            base.OnExit();
        }
    }
}