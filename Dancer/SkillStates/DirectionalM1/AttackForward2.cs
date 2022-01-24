using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
namespace Dancer.SkillStates
{
	public class AttackForward2 : BaseDirectionalM1
	{
		public override void OnEnter()
		{
			this.anim = 1.1f;
			this.damageCoefficient = StaticValues.directionalForwardDamageCoefficient;
			this.baseDuration = 0.55f;
			this.attackStartTime = 0.1f;
			this.attackEndTime = 0.3f;
			this.earlyExitTime = 0.8f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.hitStopDuration = 0.06f;
			this.pushForce = 1400f;
			this.launchVectorOverride = true;
			this.swingSoundString = "SwordSwing2";
			this.hitSoundString = "JabHit1";
			this.muzzleString = "eJab1";
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = Assets.hitEffect;
			this.impactSound = Assets.jab1HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.15f, 7f),
				new Keyframe(0.75f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isDash = true;
			this.animString = "Jab2";
			this.hitboxName = "Jab";

			base.OnEnter();
		}

		public override void SetSlideVector()
		{
			this.slideVector = base.inputBank.aimDirection;
		}

        public override void LaunchEnemy(CharacterBody body)
		{
			Vector3 direction = base.characterDirection.forward * 10f;
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
				float f = Mathf.Max(100f, m.mass);
				force = f / 100f;
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
