using System;
using EntityStates;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking; 
namespace Dancer.SkillStates
{
	internal class RibbonedState : BaseState
	{
		public float duration = 2f;
		public float pullDuration = 1f;
		public float timer;
		public Vector3 destination;
		public float deceleration = 0.5f;
		private GameObject ribbonVfxInstance;
		private float wait = 0.075f;
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			int layerIndex = modelAnimator.GetLayerIndex("Body");
			modelAnimator.CrossFadeInFixedTime((UnityEngine.Random.Range(0, 2) == 0) ? "Hurt1" : "Hurt2", 0.1f);
			modelAnimator.Update(0f);

			this.timer = this.duration;

			float time = Modules.Buffs.ribbonDebuffDuration;
			foreach (CharacterBody.TimedBuff buff in base.characterBody.timedBuffs)
			{
				if (buff.buffIndex == Modules.Buffs.ribbonDebuff.buffIndex)
					time = buff.timer;
			}
			if (this.timer < time)
				this.timer = time;


			if (this.timer > Modules.Buffs.ribbonBossCCDuration && base.characterBody.isChampion)
				this.timer = Modules.Buffs.ribbonBossCCDuration;

			
			if (base.rigidbody && !base.rigidbody.isKinematic)
			{
				base.rigidbody.velocity = Vector3.zero;
				if (base.rigidbodyMotor)
				{
					base.rigidbodyMotor.moveVector = Vector3.zero;
				}
			}
			foreach (EntityStateMachine e in base.gameObject.GetComponents<EntityStateMachine>())
			{
				if (e && e.customName.Equals("Weapon"))
				{
					e.SetNextStateToMain();
				}
			}
			// base.characterBody.coreTransform);
		}

		public void SetNewTimer(float newDuration)
        {
			this.timer = newDuration;
			if (this.timer > Modules.Buffs.ribbonBossCCDuration && base.characterBody.isChampion)
				this.timer = Modules.Buffs.ribbonBossCCDuration;

			float time = 0f;
			foreach (CharacterBody.TimedBuff buff in base.characterBody.timedBuffs)
			{
				if (buff.buffIndex == Modules.Buffs.ribbonDebuff.buffIndex)
					time = buff.timer;
			}
			if (this.timer > time)
				this.timer = time;
		}

		public override void OnExit()
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				modelAnimator.enabled = true;
			}		
			base.OnExit();
		}

		

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.timer -= Time.fixedDeltaTime;

			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator && base.fixedAge > this.wait)
			{
				modelAnimator.enabled = false;
			}
			if (base.characterMotor)
			{
				float magnitude = base.characterMotor.velocity.magnitude;
				magnitude -= this.deceleration * Time.fixedDeltaTime;
				base.characterMotor.velocity = base.characterMotor.velocity.normalized * magnitude;
				if (base.characterMotor.velocity.x == 0 && base.characterMotor.velocity.z == 0)
					base.characterMotor.velocity.y = 0f;
			}
			else if (base.rigidbody)
			{
				float magnitude = base.rigidbody.velocity.magnitude;
				magnitude -= this.deceleration * Time.fixedDeltaTime;
				base.rigidbody.velocity = base.rigidbody.velocity.normalized * magnitude;
				if (base.rigidbody.velocity.x == 0 && base.rigidbody.velocity.z == 0)
					base.rigidbody.velocity = Vector3.zero;
			}



			if (this.timer <= 0f || (NetworkServer.active && !this.characterBody.HasBuff(Modules.Buffs.ribbonDebuff)))
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{	
			return InterruptPriority.Frozen;
		}


	}
}
