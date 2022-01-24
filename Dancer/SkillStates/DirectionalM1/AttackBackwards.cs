using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
namespace Dancer.SkillStates
{
	public class AttackBackwards : BaseDirectionalM1
	{
		public override void OnEnter()
		{
			this.anim = 1f;
			this.baseDuration = 0.7f;
			this.attackStartTime = 0.18f;
			this.attackEndTime = 0.32f;
			this.earlyExitTime = 0.8f;
			this.hitStopDuration = 0.05f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = StaticValues.directionalBackDamageCoefficient;
			this.damageType = RoR2.DamageType.Generic;
			this.pushForce = 800f;
			this.swingSoundString = "SwordSwing3";
			this.hitSoundString = "WhipHit2";
			this.muzzleString = "eDashAttack";
			this.swingEffectPrefab = Assets.dashAttackEffect;
			this.hitEffectPrefab = Assets.stabHitEffect;
			this.impactSound = Assets.jab3HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 7f),
				new Keyframe(0.5f, 0f),
				new Keyframe(1f, 0f)
			});
			this.isDash = true;
			this.isFlinch = true;
			this.animString = "AttackBack";
			this.hitboxName = "Jab";
			this.canRecieveInput = false;

			base.OnEnter();
		}

		public override void SetSlideVector()
		{
			Vector3 forward = base.inputBank.moveVector;
			this.slideVector = forward;
			if (forward == Vector3.zero)
			{
				forward = base.inputBank.aimDirection;
				this.slideVector = -forward;
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (!this.canRecieveInput && !(this.inputVector.x < -0.5f))
				this.canRecieveInput = true;
		}

		
	}
}
