using System;
using EntityStates.Merc;
using Ridley.Modules;

namespace Ridley.SkillStates
{
	// Token: 0x02000011 RID: 17
	public class NAir : BaseM1
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003FF4 File Offset: 0x000021F4
		public override void OnEnter()
		{
			this.baseDuration = 0.6f;
			this.attackStartTime = 0.2f;
			this.attackEndTime = 1f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 3.5f;
			this.stackGainAmount = 8;
			this.hitStopDuration = 0.15f;
			this.isAerial = true;
			this.isSus = true;
			this.isFlinch = true;
			this.swingSoundString = "NAir";
			this.hitSoundString = "SwordHit2";
			this.swingEffectPrefab = Assets.swordSwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword2HitSoundEvent.index;
			this.animString = "Nair";
			this.hitboxName = "NAir";
			base.OnEnter();
		}
	}
}
