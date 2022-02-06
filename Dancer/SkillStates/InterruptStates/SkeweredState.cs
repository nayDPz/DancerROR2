using System;
using EntityStates;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace Dancer.SkillStates
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

			if (base.characterBody && NetworkServer.active) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;


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

			if (NetworkServer.active)
			{
				base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
			}

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
			if (base.fixedAge >= this.skewerDuration && base.fixedAge < this.skewerDuration + this.pullDuration)
			{
				this.stopwatch += Time.fixedDeltaTime;
				float num = this.pullDuration - this.stopwatch;

				Vector3 vector = this.destination - base.characterBody.coreTransform.position;
				if (num > 0)
				{
					float num2 = vector.magnitude / num;
					Vector3 normalized = vector.normalized;
					float num3 = Mathf.Lerp(1.5f, .5f, this.stopwatch / this.pullDuration);
					if (base.characterBody.isChampion)
					{
						num3 /= 2f;
					}
					num2 *= num3;
					if (base.characterMotor)
					{
						if (base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
						{
							base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
						}
						base.characterMotor.velocity = normalized * num2;
						//base.characterMotor.velocity.y = 0f;
					}
					else
					{
						base.transform.position += normalized * num2 * Time.fixedDeltaTime;
					}
				}

			}
			if (base.fixedAge >= this.skewerDuration + this.pullDuration)
			{
				if (base.GetComponent<SetStateOnHurt>().canBeStunned && base.isAuthority)
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
			writer.Write(this.destination);
			writer.Write((double)this.pullDuration);
			writer.Write((double)this.skewerDuration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
			this.destination = reader.ReadVector3();
			this.pullDuration = (float)reader.ReadDouble();
			this.skewerDuration = (float)reader.ReadDouble();
        }
    }
}
