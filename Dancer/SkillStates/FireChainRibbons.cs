using Dancer.Modules;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Dancer.SkillStates
{

    public class FireChainRibbons : BaseSkillState
    {
        public static float damageCoefficient = 0.75f;

        public static float procCoefficient = 0.5f;

        public static float baseDuration = 0.35f;

        public static float force = 0f;

        public static float recoil = 1f;

        public static float range = 62f;

        private float duration;

        private float fireTime;

        private bool hasFired;

        private Animator animator;

        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            StartAimMode();
            duration = DragonLunge.baseDuration / attackSpeedStat;
            fireTime = 0.25f * duration;
            base.characterBody.SetAimTimer(2f);
            animator = GetModelAnimator();
            muzzleString = "LanceBase";
            Util.PlaySound("Play_item_proc_whip", base.gameObject);
        }

        public override void OnExit()
        {
            if (!hasFired)
            {
                Fire();
            }
            base.OnExit();
        }

        private void Fire()
        {
            if (!hasFired)
            {
                hasFired = true;
                base.characterBody.AddSpreadBloom(1.5f);
                if (base.isAuthority)
                {
                    GameObject dancerRibbonProjectile = Projectiles.dancerRibbonProjectile;
                    Ray aimRay = GetAimRay();
                    Vector3 origin = aimRay.origin;
                    Vector3 direction = aimRay.direction;
                    ProjectileManager.instance.FireProjectile(dancerRibbonProjectile, origin, Util.QuaternionSafeLookRotation(direction), base.gameObject, damageCoefficient * damageStat, 0f, RollCrit());
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= fireTime)
            {
                Fire();
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}