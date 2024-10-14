using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates.InterruptStates
{

    internal class SuspendedState : BaseState
    {
        public float duration = 2f;

        public float pullDuration = 1f;

        private float stopwatch;

        public Vector3 destination;

        private float wait = 0.075f;

        public override void OnEnter()
        {
            base.OnEnter();
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
            if ((bool)rigidbodyMotor)
            {
                rigidbodyMotor.moveVector = Vector3.zero;
            }
            if ((bool)rigidbody)
            {
                rigidbody.velocity = Vector3.zero;
            }
            if (fixedAge >= duration)
            {
                if (GetComponent<SetStateOnHurt>().canBeStunned)
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
            writer.Write((double)duration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            duration = (float)reader.ReadDouble();
        }
    }
}