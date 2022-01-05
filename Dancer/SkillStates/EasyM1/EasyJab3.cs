using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
namespace Dancer.SkillStates
{
	public class EasyJab3 : BaseM1
	{
		public override void OnEnter()
		{
			this.anim = 1f;
			this.baseDuration = 1f;
			this.attackStartTime = 0.15f;
			this.attackEndTime = 0.4f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 8f;
			this.damageCoefficient = 3.75f;
			this.damageType = RoR2.DamageType.Generic;
			this.hitStopDuration = 0.25f;
			this.pushForce = 300f;
			this.swingSoundString = "DashAttack";
			this.hitSoundString = "Jab3Hit";
			this.critHitSoundString = "SwordHit3";
			this.muzzleString = "Mouth";
			this.swingEffectPrefab = Assets.dashAttackEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab3HitSoundEvent.index;

			this.canMove = true;
			this.animString = "DashAttack";
			this.hitboxName = "Jab";
			base.OnEnter();
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

			Vector3 direction = base.characterDirection.forward * 15f + Vector3.up * 7.5f;
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
