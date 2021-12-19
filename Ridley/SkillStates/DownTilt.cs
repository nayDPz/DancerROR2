using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;
using RoR2;	
namespace Ridley.SkillStates
{
	// Token: 0x0200000B RID: 11
	public class DownTilt : BaseM1
	{
		// Token: 0x0600002C RID: 44 RVA: 0x000039DC File Offset: 0x00001BDC
		public override void OnEnter()
		{
			this.anim = 1.65f;
			this.baseDuration = 0.4f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.stackGainAmount = 7;
			this.hitStopDuration = 0.025f;
			this.bonusForce = Vector3.up * 1800f;
			this.pushForce = 1800f;
			this.launchVectorOverride = true;
			this.isFlinch = true;
			this.earlyExitJump = true;
			this.swingSoundString = "DownTilt";
			this.hitSoundString = "SwordHit";
			this.muzzleString = "DTilt";
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.animString = "DownTilt";
			this.hitboxName = "DownTilt";
			this.hitHopVelocity = 11f;
			base.OnEnter();
		}

		public override void LaunchEnemy(HurtBox hurtBox)
		{
			//Vector3 launchVector = (Vector3.up * 15f + base.transform.position) - hurtBox.healthComponent.body.footPosition;
			//launchVector = launchVector.normalized;

			Vector3 direction = base.characterDirection.forward * 4f + Vector3.up * 10f;
			Vector3 launchVector = (direction + base.transform.position) - hurtBox.healthComponent.body.transform.position;
			launchVector = launchVector.normalized;
			CharacterMotor m = hurtBox.healthComponent.body.characterMotor;
			float force = 0.25f;
			if (m)
			{
				float f = Mathf.Max(165f, m.mass);
				force = f / 165f;
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
