using System;
using EntityStates;
using UnityEngine;
using RoR2;
using EntityStates;
namespace Dancer.SkillStates { 

	internal class SpikedState : BaseState
	{
		public float duration = 1f;
		private float stopwatch;
		private float wait = 0.2f;
		private bool hitPause;
		private float bounceForce = 1500f;
		private Vector3 storedVelocity;

		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			
			if(modelAnimator)
            {
				int layerIndex = modelAnimator.GetLayerIndex("Body");
				modelAnimator.enabled = false;
				modelAnimator.CrossFadeInFixedTime((UnityEngine.Random.Range(0, 2) == 0) ? "Hurt1" : "Hurt2", 0.1f);
				modelAnimator.Update(0f);
			}
			
			if(base.characterMotor)
            {
				this.storedVelocity = base.characterMotor.velocity;
				base.characterMotor.velocity = Vector3.zero;
			}
			else if(base.rigidbody)
            {
				this.storedVelocity = base.rigidbody.velocity;
				base.rigidbody.velocity = Vector3.zero;
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

		public void LaunchEnemy(CharacterBody body)
		{
			Vector3 launchVector = Vector3.up;
			launchVector *= this.bounceForce;
			float d = (this.duration - base.fixedAge) / this.duration;
			launchVector *= d;
			if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
			{
				body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}

			CharacterMotor m = body.characterMotor;

			float force = 0.25f;
			if (m)
			{
				float f = Mathf.Max(100f, m.mass);
				force = f / 100f;
				launchVector *= force;
				m.ApplyForce(launchVector);
			}
			else if (body.rigidbody)
			{
				float f = Mathf.Max(50f, body.rigidbody.mass);
				force = f / 200f;
				launchVector *= force;
				body.rigidbody.AddForce(launchVector, ForceMode.Impulse);
			}

		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator && base.fixedAge < this.wait)
			{
				this.hitPause = true;
				if (base.characterMotor)
				{
					base.characterMotor.velocity = Vector3.zero;
				}
				else if (base.rigidbody)
				{
					base.rigidbody.velocity = Vector3.zero;
				}
				modelAnimator.enabled = false;
			}
			if(this.hitPause && base.fixedAge > this.wait)
            {
				this.hitPause = false;
				if(modelAnimator)
					modelAnimator.enabled = true;
				if (base.characterMotor)
				{
					base.characterMotor.velocity = this.storedVelocity;
				}
				else if (base.rigidbody)
				{
					base.rigidbody.velocity = this.storedVelocity;
				}
			}
			if(base.isGrounded)
            {
				Util.PlaySound("JabHit22", base.gameObject);
				float d = (this.duration - base.fixedAge) / this.duration;

				GameObject effect = Resources.Load<GameObject>("prefabs/effects/impacteffects/beetleguardgroundslam");
				ShakeEmitter s = effect.GetComponent<ShakeEmitter>();
				GameObject.Destroy(effect.transform.Find("Spikes, Small"));
				GameObject.Destroy(effect.transform.Find("Spikes, Large"));
				EffectManager.SpawnEffect(effect, new EffectData
				{
					origin = base.transform.position,
					scale = 1.25f * d,
				}, true);
				this.LaunchEnemy(base.characterBody);
				if (base.GetComponent<SetStateOnHurt>().canBeStunned)
					this.outer.SetNextState(new StunState { stunDuration = 1f });
				else
					this.outer.SetNextStateToMain();
				return;
			}



			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}


	}
}
