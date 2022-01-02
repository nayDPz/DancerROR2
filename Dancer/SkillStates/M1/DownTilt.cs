using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;	
namespace Dancer.SkillStates
{
	public class DownTilt : BaseM1
	{
		public override void OnEnter()
		{
			this.anim = 1f;
			this.baseDuration = 0.4f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.damageCoefficient = 2f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.stackGainAmount = 7;
			this.hitStopDuration = 0.025f;
			this.bonusForce = Vector3.up * 1800f;
			this.pushForce = 1900f;
			this.launchVectorOverride = true;
			this.isFlinch = true;
			this.earlyExitJump = true;
			this.swingSoundString = "DownTilt";
			this.hitSoundString = "SwordHit";
			this.critHitSoundString = "SwordHit2";
			this.muzzleString = "DTilt";
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.animString = "DownTilt";
			this.hitboxName = "DownTilt";
			this.hitHopVelocity = 11f;
			base.OnEnter();
		}

		public override void LaunchEnemy(CharacterBody body)
		{
			//Vector3 launchVector = (Vector3.up * 15f + base.transform.position) - hurtBox.healthComponent.body.footPosition;
			//launchVector = launchVector.normalized;

			//Debug.Log("dtilt launch " + body.ToString());
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
