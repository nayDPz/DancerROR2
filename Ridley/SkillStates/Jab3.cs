using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;
using RoR2;
namespace Ridley.SkillStates
{
	// Token: 0x0200000F RID: 15
	public class Jab3 : BaseM1
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00003E00 File Offset: 0x00002000
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

		public override void LaunchEnemy(HurtBox hurtBox)
		{
			Vector3 direction = base.characterDirection.forward * 10f + Vector3.up * 3f;
			Vector3 launchVector = (direction + base.transform.position) - hurtBox.healthComponent.body.transform.position;
			launchVector = launchVector.normalized;
			bool flag16 = hurtBox.healthComponent.gameObject.GetComponent<KinematicCharacterController.KinematicCharacterMotor>();
			if (flag16)
			{
				hurtBox.healthComponent.gameObject.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}
			CharacterMotor m = hurtBox.healthComponent.body.characterMotor;
			float force = 0.25f;
			if (m)
			{
				float f = Mathf.Max(150f, m.mass);
				force = f / 150f;
			}
			float fz = this.pushForce * force;
			DamageInfo damageInfo = new DamageInfo
			{
				position = hurtBox.healthComponent.body.transform.position,
				attacker = null,
				inflictor = null,
				damage = 0f,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.Generic,
				crit = false,
				force = launchVector * fz,
				procChainMask = default(ProcChainMask),
				procCoefficient = 0f
			};
			hurtBox.healthComponent.TakeDamageForce(damageInfo, false, false);
		}
	}
}
