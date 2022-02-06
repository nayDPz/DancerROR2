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
        public static float popRadius = 4f;
        public static float cooldownOnMiss = 1f;
        public static float smallHopStrength = 12f;
        public static float antiGravityStrength = 30f;
        public static float pullForce = 3f;
        public static float damageCoefficient = Modules.StaticValues.dragonLungeDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.7f;
        public static float force = 0f;
        public static float recoil = 1f;
        public static float range = 62f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerCaptainShotgun");
        public static GameObject muzzleEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashHuntress");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/HitEffect/HitsparkCaptainShotgun");

        private DancerComponent weaponAnimator;

        private float earlyExitTime = 0.25f;
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
                Vector3 direction = base.inputBank.moveVector;

                characterMotor.Motor.ForceUnground();
                characterMotor.velocity = new Vector3(smallHopStrength * direction.x, Mathf.Max(characterMotor.velocity.y, smallHopStrength), smallHopStrength * direction.z);
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

                bool isHeld = base.IsKeyDownAuthority();

                

                base.characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(Modules.Assets.dragonLungeEffect, base.gameObject, this.muzzleString, false);
                Util.PlaySound("LungeFire", base.gameObject);

                if (base.isAuthority)
                {


                    Ray aimRay = base.GetAimRay();
                    base.AddRecoil(-1f * DragonLunge2.recoil, -2f * DragonLunge2.recoil, -0.5f * DragonLunge2.recoil, 0.5f * DragonLunge2.recoil);
                    bool hitEnemy = false;
                    List<GameObject> hitBodies = new List<GameObject>();

                    RaycastHit raycastHit;
                    if (Util.CharacterSpherecast(base.gameObject, aimRay, 1.5f, out raycastHit, DragonLunge2.range, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        hitWorld = true;
                        this.hitPoint = raycastHit.point;
                    }

                    var bulletAttack = new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = DragonLunge2.damageCoefficient * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Stun1s,
                        falloffModel = BulletAttack.FalloffModel.None,
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
                        stopperMask = LayerIndex.world.mask, //LayerIndex.CommonMasks.bullet
                        weapon = null,
                        tracerEffectPrefab = null,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = Modules.Assets.stabHitEffect,
                    };


                    
                    bulletAttack.hitCallback = (ref BulletAttack.BulletHit hitInfo) =>
                    {
                        var result = bulletAttack.DefaultHitCallback(ref hitInfo);

                        if (!hitWorld)
                            this.hitPoint = hitInfo.point;

                        float distance = (base.transform.position - this.hitPoint).magnitude;
                        Vector3 direction = (this.hitPoint - base.transform.position).normalized;
                        float duration = Mathf.Lerp(Pull.minDuration, Pull.maxDuration, distance / Pull.maxDistance);
                        if (hitInfo.hitHurtBox)
                        {
                            HurtBox hurtBox = hitInfo.hitHurtBox;
                            if (hurtBox)
                            {
                                HealthComponent h = hurtBox.healthComponent;
                                if (h && h.body)
                                {
                                    hitEnemy = true;
                                    hitBodies.Add(h.body.gameObject);
                                    
                                }
                            }
                        }

                        return result;
                    };
                    bulletAttack.Fire();

                    if (this.hitWorld || hitEnemy)
                    {
                        this.hitPoint = this.hitWorld ? this.hitPoint : aimRay.GetPoint(range);
                        if(base.IsKeyDownAuthority())
                        {
                            this.outer.SetNextState(new Pull2
                            {
                                waitTime = this.duration - this.fireTime,
                                point = hitPoint,
                                hitWorld = hitWorld,
                                hitBodies = hitBodies,
                            });
                        }
                        
                        this.OnHitAnyAuthority();
                    }

                    if (this.hitWorld && !hitEnemy && !base.IsKeyDownAuthority())
                        base.activatorSkillSlot.rechargeStopwatch += base.activatorSkillSlot.CalculateFinalRechargeInterval() - DragonLunge2.cooldownOnMiss;
                    else if (!this.hitWorld && !hitEnemy)
                        base.activatorSkillSlot.rechargeStopwatch += base.activatorSkillSlot.CalculateFinalRechargeInterval() - DragonLunge2.cooldownOnMiss;

                    Vector3 between = this.hitPoint - base.transform.position;
                    if (this.hitPoint != Vector3.zero && between.magnitude > 5f)
                    {
                        this.weaponAnimator.RotationOverride(between * 500f + base.transform.position);
                    }
                    else
                        this.weaponAnimator.RotationOverride(aimRay.GetPoint(range));
                }
            }
        }

        private void FireLollipop(Vector3 position)
        {
            bool hitz = false;
            List<HealthComponent> hits = new List<HealthComponent>();
            Collider[] hit = Physics.OverlapSphere(position, DragonLunge2.popRadius, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal);
            for (int i = 0; i < hit.Length; i++)
            {
                HurtBox hurtBox = hit[i].GetComponent<HurtBox>();
                if (!hurtBox)
                {
                    hitz = true;
                    this.outer.SetNextState(new Pull2
                    {
                        waitTime = this.duration - this.fireTime,
                        point = hit[i].transform.position,
                        hitWorld = true,
                        hitBodies = new List<GameObject>(),
                    });
                    Debug.Log("popped dlunge");
                    this.OnHitAnyAuthority();
                    return;
                }

            }
                
        }

        private void OnHitAnyAuthority()
        {

            Util.PlaySound("LungeHit", base.gameObject);

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


            if (base.fixedAge >= this.duration - this.earlyExitTime && this.hasFired && base.isAuthority)
            {
                this.weaponAnimator.StopRotationOverride();
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