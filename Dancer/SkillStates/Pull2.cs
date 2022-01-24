using EntityStates;
using RoR2;
using UnityEngine;
using Dancer.Modules.Components;
using UnityEngine.Networking;
using System.Collections.Generic;
namespace Dancer.SkillStates
{
    public class Pull2 : BaseSkillState
    {
        public List<GameObject> hitBodies;
        public float waitTime;
        public Vector3 point;
        private Vector3 direction;
        private bool pullStarted;
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

        private DancerComponent weaponAnimator;

        public override void OnEnter()
        {
            base.OnEnter();

            if (base.characterBody && NetworkServer.active) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            this.animator = base.GetModelAnimator();
            this.weaponAnimator = base.GetComponent<DancerComponent>();

            float distance = Mathf.Max((this.point - base.transform.position).magnitude - 3f, 0);
            Vector3 direction = (this.point - base.transform.position).normalized;
            Vector3 point = distance * direction + base.transform.position;
            this.distance = (base.transform.position - point).magnitude;
            this.direction = (point - base.transform.position).normalized;
            this.duration = Mathf.Lerp(minDuration, maxDuration, this.distance / maxDistance);
            this.speed = this.distance / this.duration;
            this.startSpeed = this.speed * 2f;
            this.endSpeed = this.speed * 0.0f;


            foreach(GameObject body in this.hitBodies)
            {
                if(body && body.GetComponent<NetworkIdentity>())
                {
                    EntityStateMachine component = body.GetComponent<EntityStateMachine>();
                    if (component && body.GetComponent<SetStateOnHurt>() && body.GetComponent<SetStateOnHurt>().canBeFrozen)
                {
                        if (!hitWorld)
                        {
                            SuspendedState newNextState = new SuspendedState
                            {
                                duration = this.duration,
                            };
                            component.SetInterruptState(newNextState, InterruptPriority.Vehicle);
                        }
                        else
                        {
                            SkeweredState newNextState = new SkeweredState
                            {
                                skewerDuration = (this.waitTime),
                                pullDuration = this.duration,
                                destination = this.point,
                            };
                            component.SetInterruptState(newNextState, InterruptPriority.Vehicle);
                        }

                    }
                }
                
            }

            if (base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }

            this.weaponAnimator.RotationOverride(direction.normalized * 500f + base.transform.position);

            
        }

        public override void OnExit()
        {
            this.animator.SetFloat("DragonLunge.playbackRate", 1f);
            if (NetworkServer.active)
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            this.weaponAnimator.StopRotationOverride();
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.waitTime)
            {
                if(!this.pullStarted)
                {
                    this.animator.SetFloat("DragonLunge.playbackRate", 1f);
                    this.pullStarted = true;
                    //EffectManager.SimpleMuzzleFlash(Modules.Assets.dragonLungePullEffect, base.gameObject, "LanceBase", false);
                    base.PlayAnimation("FullBody, Override", "DragonLungePull", "Slash.playbackRate", this.duration * 1f);
                    Util.PlaySound("LungeDash", base.gameObject);
                }
                this.stopwatch += Time.fixedDeltaTime;
                this.speed = Mathf.Lerp(this.startSpeed, this.endSpeed, this.stopwatch / this.duration);
                base.characterDirection.forward = this.direction;
                base.characterMotor.velocity = this.direction * this.speed;
                if (this.stopwatch >= duration)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    if (base.inputBank.jump.justPressed)
                    {
                        base.PlayAnimation("Body", "Jump");
                        base.SmallHop(base.characterMotor, base.characterBody.jumpPower);
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    else if (!this.hitWorld)
                        this.outer.SetNextStateToMain();
                    else if (!base.inputBank.skill3.down)
                        this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                base.characterMotor.velocity = Vector3.zero;
                this.animator.SetFloat("DragonLunge.playbackRate", 0f);
            }
                
            
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            int count = 0;
            foreach (GameObject body in this.hitBodies)
            {
                if (body && body.GetComponent<NetworkIdentity>())
                    count++;
            }
            writer.Write(count);
            writer.Write((double)this.waitTime);
            writer.Write(this.hitWorld);
            writer.Write(this.point);           
            foreach(GameObject body in this.hitBodies)
            {
                if (body && body.GetComponent<NetworkIdentity>()) 
                    writer.Write(body.GetComponent<NetworkIdentity>().netId);
            }
            
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            this.hitBodies = new List<GameObject>();
            base.OnDeserialize(reader);
            this.waitTime = (float)reader.ReadDouble();
            this.hitWorld = reader.ReadBoolean();
            this.point = reader.ReadVector3();
            int count = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                this.hitBodies.Add(NetworkServer.FindLocalObject(reader.ReadNetworkId()));
            }
        }
    }
}