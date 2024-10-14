using Dancer.SkillStates.InterruptStates;
using EntityStates;
using KinematicCharacterController;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
            if ((bool)base.characterBody && NetworkServer.active)
            {
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            animator = GetModelAnimator();
            weaponAnimator = GetComponent<DancerComponent>();
            float num = Mathf.Max((point - base.transform.position).magnitude - 3f, 0f);
            Vector3 normalized = (point - base.transform.position).normalized;
            Vector3 vector = num * normalized + base.transform.position;
            distance = (base.transform.position - vector).magnitude;
            direction = (vector - base.transform.position).normalized;
            duration = Mathf.Lerp(minDuration, maxDuration, distance / maxDistance);
            speed = distance / duration;
            startSpeed = speed * 1.5f;
            endSpeed = speed * 0.5f;
            foreach (GameObject hitBody in hitBodies)
            {
                if ((bool)hitBody && (bool)hitBody.GetComponent<NetworkIdentity>())
                {
                    EntityStateMachine component = hitBody.GetComponent<EntityStateMachine>();
                    if ((bool)component && (bool)hitBody.GetComponent<SetStateOnHurt>() && hitBody.GetComponent<SetStateOnHurt>().canBeFrozen)
                    {
                        bool flag = false;
                        SkeweredState newNextState = new SkeweredState
                        {
                            skewerDuration = waitTime,
                            pullDuration = duration,
                            destination = point
                        };
                        component.SetInterruptState(newNextState, InterruptPriority.Vehicle);
                    }
                }
            }
            if ((bool)GetComponent<KinematicCharacterMotor>())
            {
                GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            weaponAnimator.WeaponRotationOverride(normalized.normalized * 500f + base.transform.position);
            PlayAnimation("FullBody, Override", "DragonLungePull", "Slash.playbackRate", duration * 0.8f);
            animator.SetFloat("DragonLunge.playbackRate", 1f);
        }

        public override void OnExit()
        {
            base.PlayAnimation("Body", "Jump");
            animator.SetFloat("DragonLunge.playbackRate", 1f);
            if (NetworkServer.active)
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            weaponAnimator.StopWeaponOverride();
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= waitTime)
            {
                if (!pullStarted)
                {
                    animator.SetFloat("DragonLunge.playbackRate", 1f);
                    pullStarted = true;
                    Util.PlaySound("LungeDash", base.gameObject);
                }
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch < duration)
                {
                    speed = Mathf.Lerp(startSpeed, endSpeed, stopwatch / duration);
                    base.characterDirection.forward = direction;
                    base.characterMotor.velocity = direction * speed;
                }
                if (!(stopwatch >= duration))
                {
                    return;
                }
                animator.SetFloat("DragonLunge.playbackRate", 0f);
                if (base.inputBank.skill3.down)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    if (base.inputBank.jump.justPressed)
                    {
                        base.PlayAnimation("FullBody, Override", "BufferEmpty");
                        SmallHop(base.characterMotor, base.characterBody.jumpPower);
                        outer.SetNextStateToMain();
                        return;
                    }
                }
                if (!hitWorld)
                {
                    outer.SetNextStateToMain();
                }
                else if (!base.inputBank.skill3.down)
                {
                    outer.SetNextStateToMain();
                }
            }
            else
            {
                base.characterMotor.velocity = Vector3.zero;
                animator.SetFloat("DragonLunge.playbackRate", 0f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            int num = 0;
            foreach (GameObject hitBody in hitBodies)
            {
                if ((bool)hitBody && (bool)hitBody.GetComponent<NetworkIdentity>())
                {
                    num++;
                }
            }
            writer.Write(num);
            writer.Write((double)waitTime);
            writer.Write(hitWorld);
            writer.Write(point);
            foreach (GameObject hitBody2 in hitBodies)
            {
                if ((bool)hitBody2 && (bool)hitBody2.GetComponent<NetworkIdentity>())
                {
                    writer.Write(hitBody2.GetComponent<NetworkIdentity>().netId);
                }
            }
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            hitBodies = new List<GameObject>();
            base.OnDeserialize(reader);
            waitTime = (float)reader.ReadDouble();
            hitWorld = reader.ReadBoolean();
            point = reader.ReadVector3();
            int num = reader.ReadInt32();
            for (int i = 0; i < num; i++)
            {
                hitBodies.Add(NetworkServer.FindLocalObject(reader.ReadNetworkId()));
            }
        }
    }
}