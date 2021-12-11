using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x0200000B RID: 11
	public class DownTilt : BaseM1
	{
		// Token: 0x0600002C RID: 44 RVA: 0x000039DC File Offset: 0x00001BDC
		public override void OnEnter()
		{
			this.baseDuration = 0.3f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.stackGainAmount = 7;
			this.hitStopDuration = 0.75f;
			this.bonusForce = Vector3.up * 2200f;
			this.isFlinch = true;
			this.swingSoundString = "DownTilt";
			this.hitSoundString = "SwordHit";
			this.swingEffectPrefab = Assets.swordSwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.animString = "DownTilt";
			this.hitboxName = "DownTilt";
			this.hitHopVelocity = 11f;
			base.OnEnter();
		}
	}
}
