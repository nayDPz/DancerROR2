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
			if(base.isAuthority)
				Util.PlayAttackSpeedSound("ForwardAirStart", base.gameObject, this.attackSpeedStat);

			base.SmallHop(base.characterMotor, 4.5f);
			this.anim = 1.1f;
			this.baseDuration = 0.9f;
			this.attackStartTime = 0.375f;
			this.attackEndTime = 0.7f;
			this.hitStopDuration = 0.15f;
			this.attackRecoil = 6f;
			this.hitHopVelocity = 12f;
			this.damageCoefficient = StaticValues.forwardAirDamageCoefficient;
			this.hitStopDuration = 0.08f;
			this.stackGainAmount = 3;
			this.pushForce = 6000f;
			this.isAerial = true;
			this.isSus = true;
			this.isFlinch = true;
			this.launchVectorOverride = true;
			this.swingSoundString = "SwordSwing3";
			this.hitSoundString = "SwordHit3";
			this.critHitSoundString = "SwordHit3";
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = Assets.bigHitEffect;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.animString = "FAir";
			this.hitboxName = "FAir";
			this.muzzleString = "eFAir";
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
						SpikedState newNextState = new SpikedState{ inflictor = base.gameObject };
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
