using System;
using EntityStates.Merc;
using Ridley.Modules;

namespace Ridley.SkillStates
{
	// Token: 0x02000010 RID: 16
	public class Flurry : BaseM1
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00003F44 File Offset: 0x00002144
		public override void OnEnter()
		{
			this.baseDuration = 0.3f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.swingSoundString = "Jab2";
			this.hitSoundString = "JabHit2";
			this.swingEffectPrefab = Assets.swordSwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.isCombo = true;
			this.isDash = true;
			this.animString = "Jab1";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
	}
}
