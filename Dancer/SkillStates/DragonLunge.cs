using EntityStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Dancer.SkillStates
{

    public class DragonLunge : BaseSkillState
    {
        public static float popRadius = 4f;

        public static float cooldownOnMiss = 1f;

        public static float smallHopStrength = 12f;

        public static float antiGravityStrength = 30f;

        public static float pullForce = 3f;

        public static float radius = 1.75f;

        public static float damageCoefficient = 5.5f;

        public static float procCoefficient = 1f;

        public static float baseDuration = 0.85f;

        public static float force = 0f;

        public static float recoil = 1f;

        public static float range = 70f;

        private DancerComponent weaponAnimator;

        private float earlyExitTime = 0.35f;

        private bool hitWorld;

        private float stopwatch;

        private Vector3 hitPoint;

        private float duration;

        private float fireTime;

        private bool hasFired;

        private Animator animator;

        private string muzzleString;

        private static float antigravityStrength;

        public override void OnEnter()
        {
            base.OnEnter();
            weaponAnimator = GetComponent<DancerComponent>();
            StartAimMode();
            duration = baseDuration / attackSpeedStat;
            fireTime = 0.45f * duration;
            base.characterBody.SetAimTimer(2f);
            animator = GetModelAnimator();
            muzzleString = "LanceBase";
            Util.PlaySound("LungeStart", base.gameObject);
            PlayAnimation("FullBody, Override", "DragonLunge", "DragonLunge.playbackRate", duration * 0.975f);
            earlyExitTime /= attackSpeedStat;
            if ((bool)base.characterMotor && smallHopStrength != 0f)
            {
                Vector3 moveVector = base.inputBank.moveVector;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity = new Vector3(smallHopStrength * moveVector.x, Mathf.Max(base.characterMotor.velocity.y, smallHopStrength), smallHopStrength * moveVector.z);
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
            bool flag = IsKeyDownAuthority();
            Transform transform = base.characterBody.coreTransform;
            ChildLocator modelChildLocator = GetModelChildLocator();
            if ((bool)modelChildLocator)
            {
                transform = modelChildLocator.FindChild(muzzleString);
            }
            base.characterBody.AddSpreadBloom(1.5f);
            Util.PlaySound("LungeFire", base.gameObject);
            if (!base.isAuthority)
            {
                return;
            }
            EffectManager.SimpleEffect(Modules.Assets.dragonLungeEffect, transform.position, transform.rotation, transmit: true);
            Ray aimRay = GetAimRay();
            AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
            bool hitEnemy = false;
            List<GameObject> hitBodies = new List<GameObject>();
            if (Util.CharacterSpherecast(base.gameObject, aimRay, 1.5f, out var hitInfo2, range, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
            {
                hitWorld = true;
                hitPoint = hitInfo2.point;
            }
            BulletAttack bulletAttack = new BulletAttack
            {
                bulletCount = 1u,
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = damageCoefficient * damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Stun1s,
                falloffModel = BulletAttack.FalloffModel.None,
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
                radius = radius,
                sniper = false,
                stopperMask = LayerIndex.world.mask,
                weapon = null,
                tracerEffectPrefab = null,
                spreadPitchScale = 0f,
                spreadYawScale = 0f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = Modules.Assets.stabHitEffect
            };
            bulletAttack.hitCallback = delegate (BulletAttack bullet, ref BulletAttack.BulletHit hitInfo)
            {
                bool result = BulletAttack.defaultHitCallback(bullet, ref hitInfo);
                if (!hitWorld)
                {
                    hitPoint = hitInfo.point;
                }
                float magnitude = (base.transform.position - hitPoint).magnitude;
                Vector3 normalized = (hitPoint - base.transform.position).normalized;
                float num = Mathf.Lerp(Pull.minDuration, Pull.maxDuration, magnitude / Pull.maxDistance);
                if ((bool)hitInfo.hitHurtBox)
                {
                    HurtBox hitHurtBox = hitInfo.hitHurtBox;
                    if ((bool)hitHurtBox)
                    {
                        HealthComponent healthComponent = hitHurtBox.healthComponent;
                        if ((bool)healthComponent && (bool)healthComponent.body)
                        {
                            hitEnemy = true;
                            hitBodies.Add(healthComponent.body.gameObject);
                        }
                    }
                }
                return result;
            };
            bulletAttack.Fire();
            if (hitWorld || hitEnemy)
            {
                if (IsKeyDownAuthority())
                {
                    outer.SetNextState(new Pull
                    {
                        waitTime = duration - fireTime,
                        point = hitPoint,
                        hitWorld = hitWorld,
                        hitBodies = hitBodies
                    });
                }
                OnHitAnyAuthority();
            }
            if (hitWorld && !hitEnemy && !IsKeyDownAuthority())
            {
                base.activatorSkillSlot.rechargeStopwatch += base.activatorSkillSlot.CalculateFinalRechargeInterval() - cooldownOnMiss;
            }
            else if (!hitWorld && !hitEnemy)
            {
                base.activatorSkillSlot.rechargeStopwatch += base.activatorSkillSlot.CalculateFinalRechargeInterval() - cooldownOnMiss;
            }
            Vector3 vector = hitPoint - base.transform.position;
            if (hitPoint != Vector3.zero && vector.magnitude > 5f)
            {
                weaponAnimator.WeaponRotationOverride(vector * 500f + base.transform.position);
            }
            else
            {
                weaponAnimator.WeaponRotationOverride(aimRay.GetPoint(range));
            }
        }

        private void FireLollipop(Vector3 position)
        {
            bool flag = false;
            List<HealthComponent> list = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, popRadius, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox component = array[i].GetComponent<HurtBox>();
                if (!component)
                {
                    flag = true;
                    outer.SetNextState(new Pull2
                    {
                        waitTime = duration - fireTime,
                        point = array[i].transform.position,
                        hitWorld = true,
                        hitBodies = new List<GameObject>()
                    });
                    Debug.Log("popped dlunge");
                    OnHitAnyAuthority();
                    break;
                }
            }
        }

        private void OnHitAnyAuthority()
        {
            Util.PlaySound("LungeHit", base.gameObject);
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
            if (base.fixedAge >= duration - earlyExitTime && hasFired && base.isAuthority)
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