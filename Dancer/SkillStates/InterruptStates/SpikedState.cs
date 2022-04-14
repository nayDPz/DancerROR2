using System;
using EntityStates;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
namespace Dancer.SkillStates { 

	internal class SpikedState : BaseState
	{
		public float duration = 1f;
		private float stopwatch;
		private float wait = 0.2f;
		private bool hitPause;
		private float bounceForce = 5000f;
		private Vector3 storedVelocity;
		public static float damageCoefficient = Modules.StaticValues.forwardAirSpikeDamageCoefficient;
		public GameObject inflictor;
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();

			if (base.characterBody && NetworkServer.active) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

			if (modelAnimator)
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

			if (NetworkServer.active) base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

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
			float d = Mathf.Max(base.fixedAge / this.duration, this.wait);
			launchVector *= d;
			if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
			{
				body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}

			CharacterMotor m = body.characterMotor;

			float force = 0.25f;
			if (m)
			{
				m.velocity = Vector3.zero;
				float f = Mathf.Max(100f, m.mass);
				force = f / 100f;
				launchVector *= force;
				m.ApplyForce(launchVector);
			}
			else if (body.rigidbody)
			{
				body.rigidbody.velocity = Vector3.zero;
				float f = Mathf.Max(50f, body.rigidbody.mass);
				force = f / 300f;
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

			if (base.isGrounded)
            {
				Util.PlaySound("Hit2", base.gameObject);
				if(base.isAuthority)
                {
					float d = (this.duration - base.fixedAge) / this.duration;

					/*
					GameObject effect = Addressables.LoadAssetAsync<GameObject>("prefabs/effects/impacteffects/beetleguardgroundslam");
					ShakeEmitter s = effect.GetComponent<ShakeEmitter>();
					GameObject.Destroy(effect.transform.Find("Spikes, Small"));
					GameObject.Destroy(effect.transform.Find("Spikes, Large"));
					EffectManager.SpawnEffect(effect, new EffectData
					{
						origin = base.transform.position,
						scale = 1.25f * d,
						//networkSoundEventIndex = Modules.Assets.grabGroundSoundEvent.index
					}, true);
					*/

					float f = Mathf.Max(base.fixedAge / this.duration, this.wait);
					

					if (this.inflictor)
                    {
						DamageInfo damageInfo = new DamageInfo
						{
							position = base.transform.position,
							attacker = this.inflictor,
							inflictor = this.inflictor,
							damage = damageCoefficient * this.damageStat * f,
							damageColorIndex = DamageColorIndex.Default,
							damageType = DamageType.Stun1s,
							crit = base.RollCrit(),
							force = Vector3.zero,
							procChainMask = default(ProcChainMask),
							procCoefficient = 0f
						};
						base.healthComponent.TakeDamage(damageInfo);
						GlobalEventManager.instance.OnHitEnemy(damageInfo, base.gameObject);
						GlobalEventManager.instance.OnHitAll(damageInfo, base.gameObject);
					};
					this.LaunchEnemy(base.characterBody);

					if(base.characterBody.GetComponent<SetStateOnHurt>() && base.characterBody.GetComponent<SetStateOnHurt>().canBeStunned)
						this.outer.SetNextState(new StunState { duration = 1f });
					else
						this.outer.SetNextStateToMain();
				}
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
			return InterruptPriority.Pain;
		}


	}
}
