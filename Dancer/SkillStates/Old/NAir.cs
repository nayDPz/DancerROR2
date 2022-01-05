using System;
using EntityStates.Merc;
using Dancer.Modules;
using RoR2;
using UnityEngine;
namespace Dancer.SkillStates
{
	public class NAir : BaseM1
	{
		public override void OnEnter()
		{
			this.anim = 1.65f;
			this.baseDuration = 0.65f;
			this.attackStartTime = 0.2f;
			this.attackEndTime = 1f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 3.5f;
			this.stackGainAmount = 8;
			this.hitStopDuration = 0.15f;
			//this.bonusForce = Vector3.up * 2100f;
			this.pushForce = 2200f;
			this.launchVectorOverride = true;
			this.isAerial = true;
			this.isFlinch = true;
			this.swingSoundString = "NAir";
			this.hitSoundString = "SwordHit2";
			this.critHitSoundString = "SwordHit3";
			this.swingEffectPrefab = Assets.dashAttackEffect;
			//this.muzzleString = "Nair";
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword2HitSoundEvent.index;
			this.animString = "Nair";
			this.hitboxName = "NAir";
			base.OnEnter();
		}

        public override void LaunchEnemy(CharacterBody body)
        {
			//Vector3 launchVector = (Vector3.up * 15f + base.transform.position) - hurtBox.healthComponent.body.footPosition;
			//launchVector = launchVector.normalized;
			
			Vector3 direction = base.characterDirection.forward * 4f + Vector3.up * 10f;
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
				float f = Mathf.Max(140f, m.mass);
				force = f / 140f;
				launchVector *= force;
				m.ApplyForce(launchVector);
			}
			else if (body.rigidbody)
			{
				body.rigidbody.velocity = Vector3.zero;
				float f = Mathf.Max(50f, body.rigidbody.mass);
				force = f / 140f;
				launchVector *= force;
				body.rigidbody.AddForce(launchVector, ForceMode.Impulse);
			}

			DamageInfo info = new DamageInfo
			{
				attacker = base.gameObject,
				inflictor = base.gameObject,
				damage = 0,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.Generic,
				crit = false,
				dotIndex = DotController.DotIndex.None,
				force = launchVector,
				position = base.transform.position,
				procChainMask = default(ProcChainMask),
				procCoefficient = 0
			};
			//body.healthComponent.TakeDamageForce(info, true, true);
		}
    }
}
