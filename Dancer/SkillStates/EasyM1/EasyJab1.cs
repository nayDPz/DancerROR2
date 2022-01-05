using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
namespace Dancer.SkillStates
{
	public class EasyJab1 : BaseM1
	{
		public override void OnEnter()
		{
			this.anim = 1.1f;
			this.damageCoefficient = 2.5f;
			this.baseDuration = 0.55f;
			this.attackStartTime = 0.1f;
			this.attackEndTime = 0.6f;
			this.hitStopDuration = 0.04f;
			this.attackRecoil = 4f;
			this.hitHopVelocity = 8f;
			this.hitStopDuration = 0.06f;
			this.pushForce = 300f;
			this.swingSoundString = "Jab1";
			this.hitSoundString = "JabHit1";
			this.critHitSoundString = "JabHit2";
			this.muzzleString = "Jab2";
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.jab1HitSoundEvent.index;

			this.canMove = true;
			this.isCombo = true;
			this.nextState = new EasyJab2();
			this.animString = "Jab1";
			this.hitboxName = "Jab";
			base.OnEnter();
		}


	}
}

