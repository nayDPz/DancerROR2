using System;
using EntityStates.Merc;
using Ridley.Modules;
using RoR2;
using UnityEngine;
namespace Ridley.SkillStates
{
	// Token: 0x02000011 RID: 17
	public class NAir : BaseM1
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003FF4 File Offset: 0x000021F4
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
			this.pushForce = 2500f;
			this.launchVectorOverride = true;
			this.isAerial = true;
			this.isFlinch = true;
			this.swingSoundString = "NAir";
			this.hitSoundString = "SwordHit2";
			this.swingEffectPrefab = Assets.nairSwingEffect;
			//this.muzzleString = "Nair";
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword2HitSoundEvent.index;
			this.animString = "Nair";
			this.hitboxName = "NAir";
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
				float f = Mathf.Max(140f, m.mass);
				force = f / 140f;
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
