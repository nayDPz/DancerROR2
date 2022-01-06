using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
namespace Dancer.SkillStates
{
	public class DashAttack : BaseM1
	{
		public override void OnEnter()
		{
			this.anim = 1f;
			this.baseDuration = 0.625f;
			this.attackStartTime = 0.21f;
			this.attackEndTime = 0.6f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 3.75f;
			this.damageType = RoR2.DamageType.BonusToLowHealth;
			this.hitStopDuration = 0.0f;
			this.pushForce = 1800f;
			this.launchVectorOverride = true;
			this.earlyExitJump = true;
			this.stackGainAmount = 12;
			this.swingSoundString = "SwordSwing3";
			this.hitSoundString = "WhipHit2";
			this.critHitSoundString = "SwordHit3";
			this.muzzleString = "eDashAttack";
			this.swingEffectPrefab = Assets.dashAttackEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab3HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 10f),
				new Keyframe(0.5f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isDash = true;
			this.isFlinch = true;
			this.animString = "DashAttack";
			this.hitboxName = "Jab";
			base.OnEnter();
		}

        public override void FixedUpdate()
        {
            base.FixedUpdate();

			base.characterDirection.forward = this.slideVector;
        }
        public override void OnHitEnemyAuthority(List<HurtBox> list)
		{
			foreach (HurtBox hurtBox in list)
			{
				HealthComponent h = hurtBox.healthComponent;
				if (h && h.combinedHealthFraction < 0.45f)
				{
					this.hitSoundString = "SwordHit3";
					break;
				}

			}
			base.OnHitEnemyAuthority(list);
		}

		public override void LaunchEnemy(CharacterBody body)
		{
			
			Vector3 direction = base.characterDirection.forward * 8f + Vector3.up * 10f;
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
