using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using EntityStates;

namespace Dancer.SkillStates
{
	public class FAir : BaseM1
	{
		public bool hop;
		public override void OnEnter()
		{
			Util.PlayAttackSpeedSound("FAir", base.gameObject, this.attackSpeedStat);
			base.SmallHop(base.characterMotor, 5f);
			this.anim = 1.15f;
			this.baseDuration = 1f;
			this.attackStartTime = 0.35f;
			this.attackEndTime = 0.65f;
			this.hitStopDuration = 0.15f;
			this.attackRecoil = 6f;
			this.hitHopVelocity = 12f;
			this.damageCoefficient = 3.2f;
			this.hitStopDuration = 0.08f;
			this.stackGainAmount = 3;
			this.pushForce = 3500f;
			this.isAerial = true;
			this.isSus = true;
			this.isFlinch = true;
			this.launchVectorOverride = true;
			this.swingSoundString = "Jab1";
			this.hitSoundString = "SwordHit3";
			this.critHitSoundString = "SwordHit3";
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.animString = "FAir";
			this.hitboxName = "FAir";
			base.OnEnter();
		}



		public override void LaunchEnemy(CharacterBody body)
		{
			Vector3 direction = base.characterDirection.forward * 10f + Vector3.down * 15f;
			Vector3 launchVector = (direction + base.transform.position) - body.transform.position;
			launchVector = launchVector.normalized;
			launchVector *= this.pushForce;

			if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
			{
				body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}

			CharacterMotor m = body.characterMotor;

			float force = 0.25f;
			if (m)
			{
				m.velocity = Vector3.zero;
				float f = Mathf.Max(150f, m.mass);
				force = f / 150f;
				launchVector *= force;
				m.ApplyForce(launchVector);

				if (body) // wisps dont like this :(
				{
					EntityStateMachine component = body.GetComponent<EntityStateMachine>();
					if (body.GetComponent<SetStateOnHurt>() && component)
					{
						SpikedState newNextState = new SpikedState();
						component.SetInterruptState(newNextState, InterruptPriority.Death);
					}
				}
			}
			else if (body.rigidbody)
			{
				body.rigidbody.velocity = Vector3.zero;
				float f = Mathf.Max(50f, body.rigidbody.mass);
				force = f / 200f;
				launchVector *= force;
				body.rigidbody.AddForce(launchVector, ForceMode.Impulse);
			}

			
		}
	}
}
