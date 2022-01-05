using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;

namespace Dancer.SkillStates
{
	public class EasyJab2 : BaseM1
	{
		public override void OnEnter()
		{

			this.anim = 1.1f;
			this.damageCoefficient = 2.5f;
			this.baseDuration = 0.55f;
			this.attackStartTime = 0.1f;
			this.attackEndTime = 0.6f;
			this.hitStopDuration = 0.025f;
			this.attackRecoil = 4f;
			this.hitHopVelocity = 8f;
			this.hitStopDuration = 0.06f;
			this.pushForce = 300f;
			this.swingSoundString = "Jab2";
			this.hitSoundString = "JabHit2";
			this.critHitSoundString = "JabHit3";
			this.muzzleString = "Jab1";
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab2HitSoundEvent.index;

			this.canMove = true;
			this.isCombo = true;
			this.nextState = new EasyJab3();
			this.animString = "Jab2";
			this.hitboxName = "Jab";
			base.OnEnter();
		}
		
	}
}
