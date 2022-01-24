using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using EntityStates;

namespace Dancer.SkillStates
{
	public class AttackDown : BaseDirectionalM1
	{
		public override void OnEnter()
		{
			this.anim = 1.1f;
			this.baseDuration = 0.67f;
			this.attackStartTime = 0.11f;
			this.attackEndTime = 0.6f;
			this.earlyExitTime = 0.7f;
			this.hitStopDuration = 0.05f;
			this.attackRecoil = 6f;
			this.hitHopVelocity = 3f;
			this.damageCoefficient = StaticValues.directionalDownDamageCoefficient;
			this.pushForce = 2250f;
			this.isFlinch = true;
			this.swingSoundString = "SwordSwing3";
			this.hitSoundString = "SwordHit3";
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = Assets.bigHitEffect;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.3f, 4f),
				new Keyframe(0.75f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isDash = true;
			this.launchVectorOverride = true;
			this.animString = "AttackDown";
			this.hitboxName = "FAir";
			this.muzzleString = "eFAir";
			this.canCombo = false;
			base.OnEnter();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (!this.canCombo && !(this.inputVector.x < -0.5f))
				this.canCombo = true;
		}

		public override void SetSlideVector()
		{
			this.slideVector = Vector3.down;
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

				if (body)
				{
					EntityStateMachine component = body.GetComponent<EntityStateMachine>();
					if (body.GetComponent<SetStateOnHurt>() && component)
					{
						SpikedState newNextState = new SpikedState { inflictor = base.gameObject };
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
