using System;
using EntityStates.Merc;
using Ridley.Modules;

namespace Ridley.SkillStates
{
	// Token: 0x02000012 RID: 18
	public class UpAir : BaseM1
	{
		// Token: 0x0600003A RID: 58 RVA: 0x000040D0 File Offset: 0x000022D0
		public override void OnEnter()
		{
			this.baseDuration = 0.5f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 3.5f;
			this.stackGainAmount = 7;
			this.isAerial = true;
			this.swingSoundString = "UpAir";
			this.hitSoundString = "SwordHit2";
			this.swingEffectPrefab = Assets.swordSwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword2HitSoundEvent.index;
			this.animString = "UpAir";
			this.hitboxName = "UpTilt";
			base.OnEnter();
		}
	}
}
