using System;
using EntityStates.Merc;
using Ridley.Modules;

namespace Ridley.SkillStates
{
	public class Flurry : BaseM1
	{
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
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.isCombo = true;
			this.isDash = true;
			this.animString = "Jab1";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
	}
}
