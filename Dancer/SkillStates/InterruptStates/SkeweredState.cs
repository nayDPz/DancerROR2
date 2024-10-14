using EntityStates;
using KinematicCharacterController;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates.InterruptStates
{

    internal class SkeweredState : BaseState
    {
        public float skewerDuration = 2f;

        public float pullDuration = 1f;

        private float stopwatch;

        public Vector3 destination;

        public bool hitWorld;

        private float wait = 0.075f;

        public override void OnEnter()
        {
            base.OnEnter();
            if ((bool)characterBody && NetworkServer.active)
            {
                characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            Animator modelAnimator = GetModelAnimator();
            int layerIndex = modelAnimator.GetLayerIndex("Body");
            modelAnimator.CrossFadeInFixedTime(Random.Range(0, 2) == 0 ? "Hurt1" : "Hurt2", 0.1f);
            modelAnimator.Update(0f);
            if ((bool)rigidbody && !rigidbody.isKinematic)
            {
                rigidbody.velocity = Vector3.zero;
                if ((bool)rigidbodyMotor)
                {
                    rigidbodyMotor.moveVector = Vector3.zero;
                }
            }
            EntityStateMachine[] components = gameObject.GetComponents<EntityStateMachine>();
            foreach (EntityStateMachine entityStateMachine in components)
            {
                if ((bool)entityStateMachine && entityStateMachine.customName.Equals("Weapon"))
                {
                    entityStateMachine.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            Animator modelAnimator = GetModelAnimator();
            if ((bool)modelAnimator)
            {
                modelAnimator.enabled = true;
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Animator modelAnimator = GetModelAnimator();
            if ((bool)modelAnimator && fixedAge > wait)
            {
                modelAnimator.enabled = false;
            }
            if ((bool)characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
            }
            if (fixedAge >= skewerDuration && fixedAge < skewerDuration + pullDuration)
            {
                stopwatch += Time.fixedDeltaTime;
                float num = pullDuration - stopwatch;
                Vector3 vector = destination - characterBody.coreTransform.position;
                if (num > 0f)
                {
                    float num2 = vector.magnitude / num;
                    Vector3 normalized = vector.normalized;
                    float num3 = Mathf.Lerp(2f, 0f, stopwatch / pullDuration);
                    if (characterBody.isChampion)
                    {
                        num3 /= 2f;
                    }
                    num2 *= num3;
                    if ((bool)characterMotor)
                    {
                        if ((bool)GetComponent<KinematicCharacterMotor>())
                        {
                            GetComponent<KinematicCharacterMotor>().ForceUnground();
                        }
                        characterMotor.velocity = normalized * num2;
                    }
                    else
                    {
                        transform.position += normalized * num2 * Time.fixedDeltaTime;
                    }
                }
            }
            if (fixedAge >= skewerDuration + pullDuration)
            {
                if (GetComponent<SetStateOnHurt>().canBeStunned && isAuthority)
                {
                    outer.SetNextState(new StunState
                    {
                        stunDuration = 1f
                    });
                }
                else
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(destination);
            writer.Write((double)pullDuration);
            writer.Write((double)skewerDuration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            destination = reader.ReadVector3();
            pullDuration = (float)reader.ReadDouble();
            skewerDuration = (float)reader.ReadDouble();
        }
    }
}