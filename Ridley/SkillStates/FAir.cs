using System;
using EntityStates.Merc;
using Ridley.Modules;

namespace Ridley.SkillStates
{
	// Token: 0x0200000C RID: 12
	public class FAir : BaseM1
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00003ABC File Offset: 0x00001CBC
		public override void OnEnter()
		{
			this.anim = 1.45f;
			this.baseDuration = 0.7f;
			this.attackStartTime = 0.225f;
			this.attackEndTime = 0.65f;
			this.hitStopDuration = 0.03f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 2.4f;
			this.attackResetInterval = 0.11f;
			this.hitStopDuration = 0.08f;
			this.stackGainAmount = 3;
			this.isMultiHit = true;
			this.isAerial = true;
			this.isSus = true;
			this.swingSoundString = "FAir";
			this.hitSoundString = "SwordHit";
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.animString = "FAir";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
	}
}
