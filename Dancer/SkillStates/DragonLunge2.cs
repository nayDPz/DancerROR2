using EntityStates;
using RoR2;
using UnityEngine;
using Dancer.Modules.Components;
using Dancer.SkillStates;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Dancer.SkillStates
{
    public class DragonLunge2 : BaseSkillState
    {
        public static float smallHopStrength = 12f;
        public static float antiGravityStrength = 30f;
        public static float pullForce = 3f;
        public static float damageCoefficient = 5f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.8f;
        public static float force = 0f;
        public static float recoil = 1f;
        public static float range = 62f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerCaptainShotgun");
        public static GameObject muzzleEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashHuntress");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/HitEffect/HitsparkCaptainShotgun");

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

        public override void OnEnter()
        {
            base.OnEnter();
            this.weaponAnimator = base.GetComponent<DancerComponent>();

            base.StartAimMode(2f);
            this.duration = DragonLunge2.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.4f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "LanceBase";
            Util.PlaySound("LungeStart", base.gameObject);
            base.PlayAnimation("FullBody, Override", "DragonLunge", "DragonLunge.playbackRate", this.duration * 0.975f);
            if (base.characterMotor && DragonLunge2.smallHopStrength != 0f)
            {
                base.characterMotor.velocity.y = DragonLunge2.smallHopStrength;
            }

            base.characterDirection.forward = base.inputBank.aimDirection;

        }

        public override void OnExit()
        {
            this.weaponAnimator.StopRotationOverride();
            this.animator.SetFloat("DragonLunge.playbackRate", 1f);
            base.OnExit();
        }



        private void Fire()
        {
            if (!this.hasFired)
            {

                this.hasFired = true;

                base.characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(Modules.Assets.dragonLungeEffect, base.gameObject, this.muzzleString, false);
                Util.PlaySound("LungeFire", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    base.AddRecoil(-1f * DragonLunge2.recoil, -2f * DragonLunge2.recoil, -0.5f * DragonLunge2.recoil, 0.5f * DragonLunge2.recoil);

                    RaycastHit raycastHit;

                    var bulletAttack = new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = DragonLunge2.damageCoefficient * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Stun1s,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        maxDistance = DragonLunge2.range,
                        force = DragonLunge2.force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = base.RollCrit(),
                        owner = base.gameObject,
                        muzzleName = muzzleString,
                        smartCollision = false,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = procCoefficient,
                        radius = 2f,
                        sniper = false,
                        stopperMask = LayerIndex.CommonMasks.bullet, //LayerIndex.CommonMasks.bullet
                        weapon = null,
                        tracerEffectPrefab = null,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = muzzleEffectPrefab,
                    };

                    bulletAttack.hitCallback = (ref BulletAttack.BulletHit hitInfo) =>
                    {
                        var result = bulletAttack.DefaultHitCallback(ref hitInfo);

                        this.hitPoint = hitInfo.point;
                        if (hitInfo.hitHurtBox && hitInfo.hitHurtBox.healthComponent && hitInfo.hitHurtBox.healthComponent.body)
                        {
                            this.hitEnemy = true;
                        }
                        else
                        {
                            this.hitWorld = true;
                        }


                        return result;
                    };
                    bulletAttack.Fire();

                    if (this.hitWorld || hitEnemy)
                        this.OnHitAnyAuthority();

                    Vector3 between = this.hitPoint - base.transform.position;
                    if (this.hitPoint != Vector3.zero && between.magnitude > 0f)
                    {
                        this.weaponAnimator.RotationOverride(between * 500f + base.transform.position);
                    }
                    else
                        this.weaponAnimator.RotationOverride(aimRay.GetPoint(range));
                }
            }
        }

        private void OnHitAnyAuthority()
        {

            Util.PlaySound("LungeHit", base.gameObject);

            base.characterMotor.velocity = Vector3.zero;

            this.hasHit = true;

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;
            if (base.fixedAge < this.fireTime)
            {
                base.characterDirection.forward = base.inputBank.aimDirection;
                CharacterMotor characterMotor = base.characterMotor;
                characterMotor.velocity.y = characterMotor.velocity.y + DragonLunge2.antigravityStrength * Time.fixedDeltaTime * (1f - this.stopwatch / this.fireTime);

            }
            if (base.fixedAge >= this.fireTime * 0.85f && !this.hasFired) this.weaponAnimator.RotationOverride(base.GetAimRay().GetPoint(range));
            if (base.fixedAge >= this.fireTime)
            {
                this.Fire();
                base.characterDirection.forward = this.hitPoint != Vector3.zero ? this.hitPoint - base.transform.position : base.characterDirection.forward;
            }

            if (this.hasHit)
            {
                base.characterMotor.velocity = Vector3.zero;
                this.animator.SetFloat("DragonLunge.playbackRate", 0f);
            }


            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                if (this.hasHit)
                {
                    Util.PlaySound("LungeDash", base.gameObject);
                    if (this.hitWorld)
                    {
                        float distance = Mathf.Max((this.hitPoint - base.transform.position).magnitude - 2f, 0);
                        Vector3 direction = (this.hitPoint - base.transform.position).normalized;
                        Vector3 point = distance * direction + base.transform.position;
                        this.outer.SetNextState(new Pull { point = point, hitWorld = this.hitWorld });
                        return;
                    }
                    else
                        this.outer.SetNextStateToMain();


                    return;
                }
                else
                {
                    this.weaponAnimator.StopRotationOverride();
                    this.outer.SetNextStateToMain();
                }

                return;
            }
        }

        GameObject netTargetObject;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }


    }
}