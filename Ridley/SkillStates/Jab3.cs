using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x0200000F RID: 15
	public class Jab3 : BaseM1
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00003E00 File Offset: 0x00002000
		public override void OnEnter()
		{
			this.baseDuration = 0.6f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 3.8f;
			this.stackGainAmount = 9;
			this.hitStopDuration = 0.15f;
			this.swingSoundString = "Jab3";
			this.hitSoundString = "JabHit3";
			this.swingEffectPrefab = Assets.swordSwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab3HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.15f, 10f),
				new Keyframe(0.5f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isCombo = true;
			this.isDash = true;
			this.isFlinch = true;
			this.animString = "Jab3";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
	}
}
