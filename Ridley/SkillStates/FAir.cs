using System;
using EntityStates.Merc;
using Ridley.Modules;
using UnityEngine;
using RoR2;
namespace Ridley.SkillStates
{
	public class FAir : BaseM1
	{
		public override void OnEnter()
		{
			this.anim = 1.45f;
			this.baseDuration = 0.7f;
			this.attackStartTime = 0.225f;
			this.attackEndTime = 0.65f;
			this.hitStopDuration = 0.03f;
			this.attackRecoil = 2f;
			this.hitHopVelocity = 2f;
			this.damageCoefficient = 2.4f;
			this.attackResetInterval = 0.11f;
			this.hitStopDuration = 0.08f;
			this.stackGainAmount = 3;
			this.isMultiHit = true;
			this.isAerial = true;
			this.isSus = true;
			//this.launchVectorOverride = true;
			this.swingSoundString = "FAir";
			this.hitSoundString = "SwordHit";
			this.critHitSoundString = "SwordHit2";
			this.swingEffectPrefab = Assets.ridleySwingEffect;
			this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
			this.impactSound = Assets.sword1HitSoundEvent.index;
			this.animString = "FAir";
			this.hitboxName = "Jab";
			base.OnEnter();
		}

		public override void LaunchEnemy(HurtBox hurtBox)
		{
			Vector3 direction = base.characterDirection.forward * 2f + Vector3.up;
			Vector3 launchVector = (direction + base.transform.position) - hurtBox.healthComponent.body.transform.position;
			launchVector = launchVector.normalized;
			if (hurtBox.healthComponent.gameObject.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
			{
				hurtBox.healthComponent.gameObject.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}
			if(hurtBox.healthComponent && hurtBox.healthComponent.body && !hurtBox.healthComponent.body.isChampion)
            {
				if(hurtBox.healthComponent.body.characterMotor)
                {
					hurtBox.healthComponent.body.characterMotor.AddDisplacement(launchVector);
					hurtBox.healthComponent.body.characterMotor.velocity.y = 0;
				}
				else if(hurtBox.healthComponent.body.rigidbody)
                {
					hurtBox.healthComponent.body.rigidbody.MovePosition(launchVector);
					Vector3 v = hurtBox.healthComponent.body.rigidbody.velocity;
					v.y = 0;
					hurtBox.healthComponent.body.rigidbody.velocity = v;
				}
            }
		}
	}
}
