using System;
using EntityStates;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
namespace Dancer.SkillStates
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
			Animator modelAnimator = base.GetModelAnimator();
			int layerIndex = modelAnimator.GetLayerIndex("Body");
			modelAnimator.CrossFadeInFixedTime((UnityEngine.Random.Range(0, 2) == 0) ? "Hurt1" : "Hurt2", 0.1f);
			modelAnimator.Update(0f);
			if (base.rigidbody && !base.rigidbody.isKinematic)
			{
				base.rigidbody.velocity = Vector3.zero;
				if (base.rigidbodyMotor)
				{
					base.rigidbodyMotor.moveVector = Vector3.zero;
				}
			}

			foreach (EntityStateMachine e in base.gameObject.GetComponents<EntityStateMachine>())
			{
				if (e && e.customName.Equals("Weapon"))
				{
					e.SetNextStateToMain();
				}
			}
		}
		public override void OnExit()
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				modelAnimator.enabled = true;
			}
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator && base.fixedAge > this.wait)
			{
				modelAnimator.enabled = false;
			}
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if(base.rigidbodyMotor)
            {
				base.rigidbodyMotor.moveVector = Vector3.zero;
            }
			if(base.rigidbody)
            {
				base.rigidbody.velocity = Vector3.zero;
            }

			
			if(base.fixedAge >= this.duration)
            {
				if (base.GetComponent<SetStateOnHurt>().canBeStunned)
					this.outer.SetNextState(new StunState { stunDuration = 1f });
				else
					this.outer.SetNextStateToMain();
				return;
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((double)this.duration);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.duration = (float)reader.ReadDouble();
		}
	}
}
