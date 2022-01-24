using System;
using EntityStates.Merc;
using Dancer.Modules;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
namespace Dancer.SkillStates
{
	public class AttackRight : BaseDirectionalM1
	{
		public override void OnEnter()
		{
			this.anim = 1.1f;
			this.damageCoefficient = StaticValues.directionalRightDamageCoefficient;
			this.baseDuration = 0.55f;
			this.attackStartTime = 0.17f;
			this.attackEndTime = 0.26f;
			this.earlyExitTime = 0.8f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.hitStopDuration = 0.06f;
			this.swingSoundString = "SwordSwing2";
			this.hitSoundString = "JabHit1";
			this.muzzleString = "eJab2";
			this.swingEffectPrefab = Assets.swingEffect;
			this.hitEffectPrefab = Assets.hitEffect;
			this.impactSound = Assets.jab1HitSoundEvent.index;
			this.isSus = true;
			this.isDash = true;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.15f, 6f),
				new Keyframe(0.75f, 0f),
				new Keyframe(1f, 0f)
			});
			this.animString = "AttackRight";
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
				this.slideVector = (new Vector3(forward.z, 0, -forward.x));
			}
		}

	}
}

