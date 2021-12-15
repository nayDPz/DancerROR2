using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x0200000D RID: 13
	public class Jab1 : BaseM1
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00003BA0 File Offset: 0x00001DA0
		public override void OnEnter()
		{
			//UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.grabFireEffect, base.FindModelChild("HandL2"));
			this.anim = 1.2f;
			this.baseDuration = 0.4f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.4f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.stackGainAmount = 6;
			this.hitStopDuration = 0.06f;
			this.pushForce = 650f;
			this.swingSoundString = "Jab1";
			this.hitSoundString = "JabHit1";
			this.muzzleString = "Jab1";
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab1HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.15f, 9f),
				new Keyframe(0.75f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isCombo = true;
			this.isDash = true;
			this.animString = "Jab1";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
	}
}
