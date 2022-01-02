using EntityStates;
using RoR2;
using UnityEngine;
using Dancer.Modules.Components;

namespace Dancer.SkillStates
{
    public class Pull : BaseSkillState
    {
        public Vector3 point;
        private Vector3 direction;
        private float distance;
        private float duration;
        private float speed;
        private float startSpeed;
        private float endSpeed;

        public bool hitWorld;
        private float exitHopVelocity = 15f;

        public static float minDuration = 0.2f;
        public static float maxDuration = 0.8f;
        public static float maxDistance = 80f;
        public static float minVelocity = 0.7f;
        public static float velocityMultiplier = 1.3f;

        private float maxAngle = 60f;
        private Animator animator;
        private float stopwatch;

        private WeaponAnimator weaponAnimator;

        public override void OnEnter()
        {
            base.OnEnter();

            this.animator = base.GetModelAnimator();
            this.weaponAnimator = base.GetComponent<WeaponAnimator>();

            this.distance = (base.transform.position - this.point).magnitude;
            this.direction = (this.point - base.transform.position).normalized;
            this.duration = Mathf.Lerp(minDuration, maxDuration, this.distance / maxDistance);
            this.speed = this.distance / this.duration;
            this.startSpeed = this.speed * 2f;
            this.endSpeed = this.speed * 0.0f;

            if (base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }

            float distance = (this.point - base.transform.position).magnitude;
            Vector3 direction = (this.point - base.transform.position).normalized;
            Vector3 point = (distance + 2) * direction + base.transform.position;

            this.weaponAnimator.RotationOverride(point);

            base.PlayCrossfade("FullBody, Override", "DragonLungePull", "Slash.playbackRate", this.duration * 1.125f, 0.05f);
        }

        public override void OnExit()
        {
            
            this.weaponAnimator.StopRotationOverride();
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.OnExit();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.speed = Mathf.Lerp(this.startSpeed, this.endSpeed, base.fixedAge / this.duration);
            base.characterDirection.forward = this.direction;
            base.characterMotor.velocity = this.direction * this.speed;
            if (base.fixedAge >= duration)
            {
                if(base.inputBank.jump.justPressed)
                {
                    base.PlayAnimation("Body", "Jump");
                    base.SmallHop(base.characterMotor, base.characterBody.jumpPower);
                    this.outer.SetNextStateToMain();
                    return;
                }
                else if(!this.hitWorld)
                    this.outer.SetNextStateToMain();
                else if(!base.inputBank.skill3.down)
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