using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x0200000A RID: 10
	public class DashAttack : BaseM1
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000038B4 File Offset: 0x00001AB4
		public override void OnEnter()
		{
			this.baseDuration = 0.875f;
			this.attackStartTime = 0f;
			this.attackEndTime = 0.6f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 5f;
			this.hitStopDuration = 0.25f;
			this.stackGainAmount = 12;
			this.swingSoundString = "DashAttack";
			this.hitSoundString = "Jab3Hit";
			this.swingEffectPrefab = Assets.swordSwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab3HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 10f),
				new Keyframe(0.5f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isDash = true;
			this.isFlinch = true;
			this.animString = "DashAttack";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
	}
}
