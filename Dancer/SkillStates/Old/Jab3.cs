using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
namespace Dancer.SkillStates
{
	public class Jab3 : BaseM1
	{
		public override void OnEnter()
		{
			this.anim = 1.2f;
			this.baseDuration = 0.6f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 3.8f;
			this.stackGainAmount = 9;
			this.hitStopDuration = 0.15f;
			this.pushForce = 1900f;
			this.launchVectorOverride = true;
			this.swingSoundString = "Jab3";
			this.hitSoundString = "JabHit3";
			this.critHitSoundString = "JabHit22";
			this.muzzleString = "Mouth";
			this.cancelledFromSprinting = true;
			this.earlyExitJump = true;
			this.swingEffectPrefab = Assets.biteEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab3HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.15f, 10f),
				new Keyframe(0.5f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isCombo = true;
			this.isDash = true;
			this.isFlinch = true;
			this.animString = "Jab3";
			this.hitboxName = "Jab";
			base.OnEnter();
		}

		public override void LaunchEnemy(CharacterBody body)
		{

			Vector3 direction = base.characterDirection.forward * 10f + Vector3.up * 3f;
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
				force = f / 150f;
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
