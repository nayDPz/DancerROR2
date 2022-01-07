using EntityStates;
using RoR2;
using UnityEngine;
using Dancer.Modules.Components;
using Dancer.SkillStates;
using System.Collections.Generic;
using RoR2.Projectile;

namespace Dancer.SkillStates
{
    public class FireChainRibbons : BaseSkillState
    {
        public static float damageCoefficient = 1f;
        public static float procCoefficient = 0.5f;
        public static float baseDuration = 0.35f;
        public static float force = 0f;
        public static float recoil = 1f;
        public static float range = 62f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerCaptainShotgun");
        public static GameObject muzzleEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashHuntress");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/HitEffect/HitsparkCaptainShotgun");


        private float duration;
        private float fireTime;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();


            base.StartAimMode(2f);
            this.duration = DragonLunge.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.25f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "LanceBase";
            //base.PlayAnimation("FullBody, Override", "DragonLunge", "DragonLunge.playbackRate", this.duration * 0.975f);
            Util.PlaySound("Play_item_proc_whip", base.gameObject);
            

        }

        public override void OnExit()
        {
            base.OnExit();
        }



        private void Fire()
        {
            if (!this.hasFired)
            {

                this.hasFired = true;

                base.characterBody.AddSpreadBloom(1.5f);
                //EffectManager.SimpleMuzzleFlash(Modules.Assets.dragonLungeEffect, base.gameObject, this.muzzleString, false);


                if (base.isAuthority)
                {
                    GameObject projectilePrefab = Modules.Projectiles.dancerRibbonProjectile;

                    Ray aimRay = base.GetAimRay();
                    Vector3 position = aimRay.origin;
                    Vector3 direction = aimRay.direction;

                    ProjectileManager.instance.FireProjectile(projectilePrefab,
                    position,
                    Util.QuaternionSafeLookRotation(direction),
                    base.gameObject,
                    FireChainRibbons.damageCoefficient * this.damageStat,
                    0f,
                    base.RollCrit(),
                    DamageColorIndex.Default,
                    null);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime)
            {
                this.Fire();              
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}