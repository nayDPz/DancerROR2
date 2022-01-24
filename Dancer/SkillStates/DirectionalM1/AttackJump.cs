using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
namespace Dancer.SkillStates
{
	public class AttackJump : BaseDirectionalM1
	{
		public override void OnEnter()
		{
			this.hitHopVelocity = 0f;
			if (base.characterMotor.jumpCount <= base.characterBody.maxJumpCount)
            {
				base.characterMotor.Jump(1f, 1f, false);
				base.characterMotor.jumpCount++;
			}
			else
            {
				this.hitHopVelocity = base.characterBody.jumpPower;
			}
				

			this.anim = 1.1f;
			this.damageCoefficient = StaticValues.directionalJumpDamageCoefficient;
			this.baseDuration = 0.65f;
			this.attackStartTime = 0.07f;
			this.attackEndTime = 0.2f;
			this.earlyExitTime = 0.67f;
			this.attackRecoil = 2f;

			this.canRecieveInput = false;
			this.hitStopDuration = 0.06f;
			this.pushForce = 2200f;
			this.swingSoundString = "SwordSwing2";
			this.hitSoundString = "JabHit1";
			this.muzzleString = "JumpAttack";
			this.launchVectorOverride = true;
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = Assets.hitEffect;
			this.impactSound = Assets.jab1HitSoundEvent.index;
			this.animString = "AttackJump";
			this.hitboxName = "FAir";
			base.OnEnter();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void LaunchEnemy(CharacterBody body)
		{
			Vector3 direction = Vector3.up * 10f;
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
				float f = Mathf.Max(150f, m.mass);
				force = f / 150f;
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

	}
}

