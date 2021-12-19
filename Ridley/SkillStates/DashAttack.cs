using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
namespace Ridley.SkillStates
{
	// Token: 0x0200000A RID: 10
	public class DashAttack : BaseM1
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000038B4 File Offset: 0x00001AB4
		public override void OnEnter()
		{
			this.baseDuration = 0.875f;
			this.attackStartTime = 0.16f;
			this.attackEndTime = 0.6f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 3.75f;
			this.damageType = RoR2.DamageType.BonusToLowHealth;
			this.hitStopDuration = 0.25f;
			this.pushForce = 2300f;
			this.launchVectorOverride = true;
			this.earlyExitJump = true;
			this.stackGainAmount = 12;
			this.swingSoundString = "DashAttack";
			this.hitSoundString = "Jab3Hit";
			this.critHitSoundString = "SwordHit3";
			this.muzzleString = "Mouth";
			this.swingEffectPrefab = Assets.biteEffect;
			this.hitEffectPrefab = Assets.biteEffect;
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

		public override void LaunchEnemy(HurtBox hurtBox)
		{
			Vector3 direction = base.characterDirection.forward * 15f + Vector3.up * 7.5f;
			Vector3 launchVector = (direction + base.transform.position) - hurtBox.healthComponent.body.transform.position;
			launchVector = launchVector.normalized;
			bool flag16 = hurtBox.healthComponent.gameObject.GetComponent<KinematicCharacterController.KinematicCharacterMotor>();
			if (flag16)
			{
				hurtBox.healthComponent.gameObject.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}
			CharacterMotor m = hurtBox.healthComponent.body.characterMotor;
			float force = 0.3f;
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
