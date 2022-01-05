using System;
using EntityStates.Merc;
using Dancer.Modules;

namespace Dancer.SkillStates
{
	// Token: 0x02000013 RID: 19
	public class UpTilt : BaseM1
	{
		// Token: 0x0600003C RID: 60 RVA: 0x0000419C File Offset: 0x0000239C
		public override void OnEnter()
		{
			this.baseDuration = 0.5f;
			this.attackStartTime = 0.2f;
			this.attackEndTime = 1f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 2.5f;
			this.stackGainAmount = 8;
			this.swingSoundString = "PunchSwing";
			this.hitSoundString = "JabHit1";
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab1HitSoundEvent.index;
			this.animString = "UpTilt";
			this.hitboxName = "UpTilt";
			base.OnEnter();
		}
	}
}
