using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x0200000E RID: 14
	public class Jab2 : BaseM1
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00003CD0 File Offset: 0x00001ED0
		public override void OnEnter()
		{
			this.baseDuration = 0.4f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.stackGainAmount = 7;
			this.hitStopDuration = 0.06f;
			this.swingSoundString = "Jab2";
			this.hitSoundString = "JabHit2";
			this.swingEffectPrefab = Assets.swordSwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab2HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.15f, 9f),
				new Keyframe(0.75f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isCombo = true;
			this.isDash = true;
			this.animString = "Jab2";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
	}
}
