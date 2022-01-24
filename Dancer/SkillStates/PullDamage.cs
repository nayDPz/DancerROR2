using EntityStates;
using RoR2;
using UnityEngine;
using Dancer.Modules.Components;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

namespace Dancer.SkillStates
{
    public class PullDamage : BaseSkillState
    {
        public bool isFirst = true;
        private OverlapAttack attack;
        public CharacterBody target;
        private GameObject nextTarget;

        private Vector3 lastKnownPosition;

        private CharacterModel characterModel;
        private Transform modelTransform;

        private HurtBoxGroup hurtboxGroup;

        public Vector3 point;
        private Vector3 direction;
        private float distance;
        private float duration;
        private float speed;
        private float startSpeed;
        private float endSpeed;
        private float fireTime;
        private float exitHopVelocity = 15f;
        public int swingIndex;
        private bool hasFired;

        private float attackRadius = 3.5f;
        private float procCoefficient = 0.75f;
        private float damageCoefficient = 3f;

        public static float minDuration = 0.2f;
        public static float maxDuration = 0.6f;
        public static float maxDistance = 80f;
        public static float minVelocity = 0.7f;
        public static float velocityMultiplier = 1.3f;

        private float maxAngle = 60f;
        private Animator animator;
        private float stopwatch;

        private DancerComponent weaponAnimator;

        public override void OnEnter()
        {
            base.OnEnter();

            if (base.characterBody && NetworkServer.active) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;          

            this.animator = base.GetModelAnimator();
            this.weaponAnimator = base.GetComponent<DancerComponent>();

            this.distance = (base.transform.position - this.target.coreTransform.position).magnitude;
            this.direction = (this.target.coreTransform.position - base.transform.position).normalized;
            this.duration = Mathf.Lerp(minDuration, maxDuration, this.distance / maxDistance);
            this.duration /= this.attackSpeedStat;
            this.speed = this.distance / this.duration;
            this.startSpeed = this.speed * 2f;
            this.endSpeed = this.speed * 0.5f;
            this.fireTime = 0.4f * this.duration;

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "NAir");
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Generic;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = this.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = EntityStates.Merc.GroundLight.finisherHitEffectPrefab;
            this.attack.forceVector = Vector3.zero;
            this.attack.pushAwayForce = 0f;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = Modules.Assets.sword2HitSoundEvent.index;

            if (base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }

            Vector3 direction = this.target.coreTransform.position - base.transform.position;
            this.weaponAnimator.WeaponRotationOverride(direction.normalized * 500f + base.transform.position);



            //EffectManager.SimpleMuzzleFlash(Modules.Assets.dragonLungePullEffect, base.gameObject, "LanceBase", false);
            if(this.isFirst)
                base.PlayAnimation("FullBody, Override", "DragonLungePull", "Slash.playbackRate", this.duration * 3f);

            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

        }

        public override void OnExit()
        {
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount--;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (NetworkServer.active)
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            this.weaponAnimator.StopWeaponOverride();
            base.OnExit();
        }

        private void Fire()
        {
            if(!this.hasFired)
            {
                this.hasFired = true;
                base.PlayAnimation("FullBody, Override", "Jab" + (this.swingIndex + 1), "Slash.playbackRate", this.duration * 1f);
                if (Util.HasEffectiveAuthority(base.gameObject))
                {
                    this.PlaySwingEffect();

                }
            }
            if(base.isAuthority)
            {
                if (this.attack.Fire())
                {
                    Util.PlayAttackSpeedSound("Jab" + (swingIndex + 1), base.gameObject, base.attackSpeedStat);
                }
            }
            
                     

        }

        private void PlaySwingEffect()
        {
            
            EffectManager.SimpleMuzzleFlash(Modules.Assets.swingEffect, base.gameObject, "eJab" + (swingIndex + 1), false);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 target;
            if (this.target)
            {
                target = this.target.coreTransform.position;
                this.lastKnownPosition = target;
            }
            else
                target = this.lastKnownPosition;
            this.direction = (target - base.transform.position).normalized;
            base.characterDirection.forward = this.direction;           
            this.speed = Mathf.Lerp(this.startSpeed, this.endSpeed, base.fixedAge / (this.duration));               
            base.characterMotor.velocity = this.direction * this.speed;      

            if(base.fixedAge >= this.duration * fireTime)
            {
                this.Fire();
            }
            if (base.fixedAge >= duration)
            {
                if (this.nextTarget && base.inputBank.skill3.down)
                {
                    if (this.swingIndex == 0) this.swingIndex = 1;
                    else this.swingIndex = 0;
                    this.outer.SetNextState(new PullDamage { target = this.nextTarget.GetComponent<CharacterBody>(), swingIndex = this.swingIndex, isFirst = false });
                    return;
                }
                this.outer.SetNextStateToMain();
                base.characterMotor.velocity = Vector3.zero;
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }
}