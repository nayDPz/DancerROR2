using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
namespace Dancer.SkillStates
{
	public class AttackSprint : BaseDirectionalM1
	{
		public override void OnEnter()
		{
			this.anim = 1f;
			this.baseDuration = 0.6f;
			this.attackStartTime = 0.07f;
			this.attackEndTime = 0.25f;
			this.earlyExitTime = 0.375f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = StaticValues.directionalSprintDamageCoefficient;
			this.damageType = RoR2.DamageType.Generic;
			this.hitStopDuration = 0.08f;
			this.pushForce = 800f;
			this.launchVectorOverride = true;
			this.swingSoundString = "SwordSwing3";
			this.hitSoundString = "WhipHit2";
			this.muzzleString = "eDashAttack";
			this.swingEffectPrefab = Assets.dashAttackEffect;
			this.hitEffectPrefab = Assets.stabHitEffect;
			this.impactSound = Assets.jab3HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 7f),
				new Keyframe(0.5f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isDash = true;
			this.isFlinch = true;
			this.animString = "AttackForward";
			this.hitboxName = "Jab";
			base.OnEnter();
		}

        public override void SetSlideVector()
        {
			this.slideVector = base.inputBank.aimDirection;
        }

        public override void FixedUpdate()
		{
			base.FixedUpdate();

		}

		public override void LaunchEnemy(CharacterBody body)
		{

			Vector3 direction = base.characterDirection.forward * 10f;
			Vector3 launchVector = (direction + base.transform.position) - body.transform.position;
			launchVector = launchVector.normalized;

			if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
			{
				body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}
			launchVector *= this.pushForce;
			CharacterMotor m = body.characterMotor;
			float force = 0.3f;
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
