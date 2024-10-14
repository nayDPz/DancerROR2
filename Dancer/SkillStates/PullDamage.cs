using EntityStates;
using EntityStates.Merc;
using KinematicCharacterController;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

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
            if ((bool)base.characterBody && NetworkServer.active)
            {
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            animator = GetModelAnimator();
            weaponAnimator = GetComponent<DancerComponent>();
            distance = (base.transform.position - target.coreTransform.position).magnitude;
            direction = (target.coreTransform.position - base.transform.position).normalized;
            duration = Mathf.Lerp(minDuration, maxDuration, distance / maxDistance);
            duration /= attackSpeedStat;
            speed = distance / duration;
            startSpeed = speed * 2f;
            endSpeed = speed * 0.5f;
            fireTime = 0.4f * duration;
            HitBoxGroup hitBoxGroup = null;
            Transform transform = GetModelTransform();
            if ((bool)transform)
            {
                hitBoxGroup = Array.Find(transform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "NAir");
            }
            attack = new OverlapAttack();
            attack.damageType = DamageType.Generic;
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 0f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            attack.impactSound = Modules.Assets.sword2HitSoundEvent.index;
            if ((bool)GetComponent<KinematicCharacterMotor>())
            {
                GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            Vector3 vector = target.coreTransform.position - base.transform.position;
            weaponAnimator.WeaponRotationOverride(vector.normalized * 500f + base.transform.position);
            if (isFirst)
            {
                PlayAnimation("FullBody, Override", "DragonLungePull", "Slash.playbackRate", duration * 3f);
            }
            modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }
            if ((bool)characterModel)
            {
                characterModel.invisibilityCount++;
            }
            if ((bool)hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter++;
            }
        }

        public override void OnExit()
        {
            if ((bool)characterModel)
            {
                characterModel.invisibilityCount--;
            }
            if ((bool)hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter--;
            }
            if (NetworkServer.active)
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            weaponAnimator.StopWeaponOverride();
            base.OnExit();
        }

        private void Fire()
        {
            if (!hasFired)
            {
                hasFired = true;
                PlayAnimation("FullBody, Override", "Jab" + (swingIndex + 1), "Slash.playbackRate", duration * 1f);
                if (Util.HasEffectiveAuthority(base.gameObject))
                {
                    PlaySwingEffect();
                }
            }
            if (base.isAuthority && attack.Fire())
            {
                Util.PlayAttackSpeedSound("Jab" + (swingIndex + 1), base.gameObject, attackSpeedStat);
            }
        }

        private void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(Modules.Assets.swingEffect, base.gameObject, "eJab" + (swingIndex + 1), transmit: false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 vector = ((!target) ? lastKnownPosition : (lastKnownPosition = target.coreTransform.position));
            direction = (vector - base.transform.position).normalized;
            base.characterDirection.forward = direction;
            speed = Mathf.Lerp(startSpeed, endSpeed, base.fixedAge / duration);
            base.characterMotor.velocity = direction * speed;
            if (base.fixedAge >= duration * fireTime)
            {
                Fire();
            }
            if (!(base.fixedAge >= duration))
            {
                return;
            }
            if ((bool)nextTarget && base.inputBank.skill3.down)
            {
                if (swingIndex == 0)
                {
                    swingIndex = 1;
                }
                else
                {
                    swingIndex = 0;
                }
                outer.SetNextState(new PullDamage
                {
                    target = nextTarget.GetComponent<CharacterBody>(),
                    swingIndex = swingIndex,
                    isFirst = false
                });
            }
            else
            {
                outer.SetNextStateToMain();
                base.characterMotor.velocity = Vector3.zero;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}