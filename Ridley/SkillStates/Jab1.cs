using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;
using RoR2;
namespace Ridley.SkillStates
{
	// Token: 0x0200000D RID: 13
	public class Jab1 : BaseM1
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00003BA0 File Offset: 0x00001DA0
		public override void OnEnter()
		{
			//UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.grabFireEffect, base.FindModelChild("HandL2"));
			this.anim = 1.2f;
			this.baseDuration = 0.4f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.stackGainAmount = 6;
			this.hitStopDuration = 0.06f;
			this.pushForce = 1300f;
			this.launchVectorOverride = true;
			this.swingSoundString = "Jab1";
			this.hitSoundString = "JabHit1";
			this.critHitSoundString = "JabHit2"; 
			this.muzzleString = "Jab1";
			this.cancelledFromSprinting = true;
			this.earlyExitJump = true;
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab1HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.15f, 9f),
				new Keyframe(0.75f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isCombo = true;
			this.isDash = true;
			this.animString = "Jab1";
			this.hitboxName = "Jab";
			base.OnEnter();
		}

		public override void LaunchEnemy(HurtBox hurtBox)
		{
			Vector3 direction = base.characterDirection.forward * 10f;
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
				float f = Mathf.Max(100f, m.mass);
				force = f / 100f;
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
