using EntityStates;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates
{

    public class Riposte : BaseSkillState
    {
        public float charge;

        public static float radius = 5f;

        public static float perfectDamageCoefficient = 24f;

        public static float maxDamageCoefficient = 18f;

        public static float minDamageCoefficient = 12f;

        private float damageCoefficient;

        public static Vector3 force = Vector3.up * 2000f;

        public static float procCoefficient = 1f;

        private float duration = 0.67f;

        private bool hasFired;

        private Ray aimRay;

        private float fireTime;

        public override void OnEnter()
        {
            base.OnEnter();
            duration /= attackSpeedStat;
            if (charge >= 1f)
            {
                damageCoefficient = perfectDamageCoefficient;
            }
            else
            {
                damageCoefficient = Mathf.Lerp(maxDamageCoefficient, minDamageCoefficient, charge);
            }
            fireTime = 0.74626863f * duration;
            StartAimMode(duration);
            GetModelAnimator();
            aimRay = GetAimRay();
            PlayAnimation("FullBody, Override", "FAir", "Slash.playbackRate", 1f);
        }

        private void Fire()
        {
            if (!hasFired)
            {
                hasFired = true;
                Util.PlaySound("FireFireball", base.gameObject);
                Vector3 point = GetAimRay().GetPoint(3f);
                point.y = 0f;
                if (new BlastAttack
                {
                    attacker = base.gameObject,
                    procChainMask = default(ProcChainMask),
                    impactEffect = EffectIndex.Invalid,
                    losType = BlastAttack.LoSType.NearestHit,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    procCoefficient = procCoefficient,
                    bonusForce = force,
                    baseForce = 0f,
                    baseDamage = damageCoefficient * damageStat,
                    falloffModel = BlastAttack.FalloffModel.None,
                    radius = radius,
                    position = point,
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
        }

        private void OnHitEnemyAuthority()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.fixedAge += Time.fixedDeltaTime;
            if (base.fixedAge >= fireTime)
            {
                Fire();
            }
            if (base.fixedAge >= duration && Util.HasEffectiveAuthority(base.gameObject))
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}