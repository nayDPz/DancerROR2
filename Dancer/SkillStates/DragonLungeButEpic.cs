using EntityStates;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates
{

    public class DragonLungeButEpic : BaseSkillState
    {
        public static float smallHopStrength = 12f;

        public static float antiGravityStrength = 30f;

        public static float pullForce = 3f;

        public static float damageCoefficient = 3.75f;

        public static float procCoefficient = 1f;

        public static float baseDuration = 0.8f;

        public static float force = 0f;

        public static float recoil = 1f;

        public static float range = 62f;

        private DancerComponent weaponAnimator;

        private CharacterBody hitTarget;

        private bool hitEnemy;

        private bool hitWorld;

        private float stopwatch;

        private Vector3 hitPoint;

        private float duration;

        private float fireTime;

        private bool hasFired;

        private bool hasHit;

        private float hitTime;

        private Animator animator;

        private string muzzleString;

        private static float antigravityStrength;

        private GameObject netTargetObject;

        public override void OnEnter()
        {
            base.OnEnter();
            weaponAnimator = GetComponent<DancerComponent>();
            StartAimMode();
            duration = baseDuration / attackSpeedStat;
            fireTime = 0.4f * duration;
            base.characterBody.SetAimTimer(2f);
            animator = GetModelAnimator();
            muzzleString = "LanceBase";
            PlayAnimation("FullBody, Override", "DragonLunge", "DragonLunge.playbackRate", duration * 0.975f);
            if ((bool)base.characterMotor && smallHopStrength != 0f)
            {
                base.characterMotor.velocity.y = smallHopStrength;
            }
            base.characterDirection.forward = base.inputBank.aimDirection;
        }

        public override void OnExit()
        {
            weaponAnimator.StopWeaponOverride();
            animator.SetFloat("DragonLunge.playbackRate", 1f);
            base.OnExit();
        }

        private void Fire()
        {
            if (hasFired)
            {
                return;
            }
            hasFired = true;
            base.characterBody.AddSpreadBloom(1.5f);
            EffectManager.SimpleMuzzleFlash(Modules.Assets.dragonLungeEffect, base.gameObject, muzzleString, transmit: false);
            if (!base.isAuthority)
            {
                return;
            }
            Ray aimRay = GetAimRay();
            AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
            BulletAttack bulletAttack = new BulletAttack
            {
                bulletCount = 1u,
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = damageCoefficient * damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Stun1s,
                falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                maxDistance = range,
                force = force,
                hitMask = LayerIndex.CommonMasks.bullet,
                minSpread = 0f,
                maxSpread = 0f,
                isCrit = RollCrit(),
                owner = base.gameObject,
                muzzleName = muzzleString,
                smartCollision = false,
                procChainMask = default(ProcChainMask),
                procCoefficient = procCoefficient,
                radius = 2f,
                sniper = false,
                stopperMask = LayerIndex.CommonMasks.bullet,
                weapon = null,
                tracerEffectPrefab = null,
                spreadPitchScale = 0f,
                spreadYawScale = 0f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = null
            };
            bulletAttack.hitCallback = delegate (BulletAttack bullet, ref BulletAttack.BulletHit hitInfo)
            {
                bool result = BulletAttack.defaultHitCallback(bullet, ref hitInfo);
                if ((bool)hitInfo.hitHurtBox)
                {
                    HurtBox hitHurtBox = hitInfo.hitHurtBox;
                    if ((bool)hitHurtBox)
                    {
                        HealthComponent healthComponent = hitHurtBox.healthComponent;
                        if ((bool)healthComponent && (bool)healthComponent.body)
                        {
                            hitTarget = healthComponent.body;
                            hitEnemy = true;
                            netTargetObject = hitTarget.gameObject;
                        }
                    }
                }
                return result;
            };
            bulletAttack.Fire();
            if (hitWorld || hitEnemy)
            {
                OnHitAnyAuthority();
            }
            else
            {
                Util.PlaySound("DSpecialSwing", base.gameObject);
            }
            Vector3 vector = hitPoint - base.transform.position;
            if (hitEnemy && hitTarget.coreTransform.position != Vector3.zero)
            {
                vector = hitTarget.coreTransform.position - base.transform.position;
                weaponAnimator.WeaponRotationOverride(vector * 500f + base.transform.position);
            }
            else if (hitPoint != Vector3.zero && vector.magnitude > 0f)
            {
                weaponAnimator.WeaponRotationOverride(vector * 500f + base.transform.position);
            }
            else
            {
                weaponAnimator.WeaponRotationOverride(aimRay.GetPoint(range));
            }
        }

        private void OnHitAnyAuthority()
        {
            Util.PlaySound("DSpecialHit", base.gameObject);
            base.characterMotor.velocity = Vector3.zero;
            hasHit = true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (base.fixedAge < fireTime)
            {
                base.characterDirection.forward = base.inputBank.aimDirection;
                CharacterMotor characterMotor = base.characterMotor;
                characterMotor.velocity.y = characterMotor.velocity.y + antigravityStrength * Time.fixedDeltaTime * (1f - stopwatch / fireTime);
            }
            if (base.fixedAge >= fireTime * 0.85f && !hasFired)
            {
                weaponAnimator.WeaponRotationOverride(GetAimRay().GetPoint(range));
            }
            if (base.fixedAge >= fireTime)
            {
                Fire();
                base.characterDirection.forward = ((hitPoint != Vector3.zero) ? (hitPoint - base.transform.position) : base.characterDirection.forward);
            }
            if (hasHit)
            {
                base.characterMotor.velocity = Vector3.zero;
                animator.SetFloat("DragonLunge.playbackRate", 0f);
            }
            if (!(base.fixedAge >= duration) || !base.isAuthority)
            {
                return;
            }
            if (hasHit)
            {
                Util.PlaySound("DSpecialPull", base.gameObject);
                if (!hitEnemy || !hitTarget)
                {
                    if (hitWorld)
                    {
                        float num = Mathf.Max((hitPoint - base.transform.position).magnitude - 2f, 0f);
                        Vector3 normalized = (hitPoint - base.transform.position).normalized;
                        Vector3 point = num * normalized + base.transform.position;
                        outer.SetNextState(new Pull
                        {
                            point = point,
                            hitWorld = hitWorld
                        });
                    }
                    else
                    {
                        outer.SetNextStateToMain();
                    }
                }
            }
            else
            {
                weaponAnimator.StopWeaponOverride();
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
