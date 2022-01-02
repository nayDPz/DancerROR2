using System;
using EntityStates;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates
{
	public class DancerMain : GenericCharacterMain
	{
		private GameObject sprintEffect1;
		private GameObject sprintEffect2;
		private Transform footL;
		private Transform footR;
		private EntityStateMachine weapon;
		private float baseAcceleration;
		private float sprintAcceleration = 300f;
		public LocalUser localUser;
		private bool bufferJump = true;
		private float landingTime = 0.2f;
		private uint sprintSoundID;

		private bool sprintSoundOn;
		public override void OnEnter()
        {
			this.localUser = LocalUserManager.readOnlyLocalUsersList[0];
			this.footL = base.FindModelChild("SprintFootL");
			this.footR = base.FindModelChild("SprintFootR");
			this.baseAcceleration = base.characterBody.acceleration;
			this.weapon = base.gameObject.GetComponents<EntityStateMachine>()[1];
			base.OnEnter();
        }
        public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.characterBody.isSprinting && !this.sprintSoundOn && base.isGrounded)
			{
				//this.sprintEffect1 = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.footDragEffect, footL.position, Util.QuaternionSafeLookRotation(Vector3.up));
				//this.sprintEffect2 = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.footDragEffect, footR.position, Util.QuaternionSafeLookRotation(Vector3.up));
				this.sprintSoundOn = true;
				base.characterBody.acceleration = this.sprintAcceleration;
				AkSoundEngine.StopPlayingID(this.sprintSoundID);
				this.sprintSoundID = Util.PlaySound("DancerSprintStart", base.gameObject);
			}
			//if(this.sprintEffect1)
			//	this.sprintEffect1.transform.position = footL.position;
			//if(this.sprintEffect2)
			//	this.sprintEffect2.transform.position = footR.position;
			if (!base.characterBody.isSprinting || !base.isGrounded)
			{
				if (this.sprintSoundOn)
				{
					base.characterBody.acceleration = this.baseAcceleration;
					this.sprintSoundOn = false;
					AkSoundEngine.StopPlayingID(this.sprintSoundID);
				}
			}


			if (Util.HasEffectiveAuthority(base.gameObject) && base.characterMotor.isGrounded && !this.localUser.isUIFocused)
			{
				if (Input.GetKeyDown(Modules.Config.emote1Keybind.Value))
				{
					this.outer.SetInterruptState(new Emotes.Emote1(), InterruptPriority.Any);
					return;
				}
				else if (Input.GetKeyDown(Modules.Config.emote2Keybind.Value))
				{
					this.outer.SetInterruptState(new Emotes.Emote2(), InterruptPriority.Any);
					return;
				}
				else if (Input.GetKeyDown(Modules.Config.standKeybind.Value))
				{
					this.outer.SetInterruptState(new Emotes.Stand(), InterruptPriority.Any);
					return;
				}
			}
		}

        public override void OnExit()
        {
			//if (this.sprintEffect1)
			//	EntityState.Destroy(this.sprintEffect1);
			//if (this.sprintEffect2)
			//	EntityState.Destroy(this.sprintEffect2);
			base.characterBody.acceleration = this.baseAcceleration;
			AkSoundEngine.StopPlayingID(this.sprintSoundID);
			base.OnExit();
        }

        public override void ProcessJump()
		{
			if (this.hasCharacterMotor)
			{
				if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
				{
					bool flag = false;
					bool flag2 = false;
					int itemCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
					float horizontalBonus = 1f;
					float verticalBonus = 1f;
					if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
					{
						flag = true;
						horizontalBonus = 1.5f;
						verticalBonus = 1.5f;
					}
					else
					{
						if ((float)itemCount > 0f && base.characterBody.isSprinting)
						{
							float num = base.characterBody.acceleration * base.characterMotor.airControl;
							if (base.characterBody.moveSpeed > 0f && num > 0f)
							{
								flag2 = true;
								float num2 = Mathf.Sqrt(10f * (float)itemCount / num);
								float num3 = base.characterBody.moveSpeed / num;
								horizontalBonus = (num2 + num3) / num3;
							}
						}
					}
					GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);
					bool hasModelAnimator = this.hasModelAnimator;
					if (hasModelAnimator)
					{
						int layerIndex = base.modelAnimator.GetLayerIndex("Body");
						if (layerIndex >= 0)
						{
							if (base.characterMotor.jumpCount == 0 || base.characterBody.baseJumpCount == 1)
							{
								base.modelAnimator.CrossFadeInFixedTime("Jump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								Util.PlaySound("DancerJump", base.gameObject);
							}
							else
							{
								base.modelAnimator.CrossFadeInFixedTime("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								Util.PlaySound("DancerJumpAir", base.gameObject);
							}
						}
					}
					if (flag)
					{
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
						{
							origin = base.characterBody.footPosition
						}, true);
					}
					else
					{
						if (base.characterMotor.jumpCount > 0)
						{
							EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
							{
								origin = base.characterBody.footPosition,
								scale = base.characterBody.radius
							}, true);
						}
					}
					if (flag2)
					{
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
						{
							origin = base.characterBody.footPosition,
							rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
						}, true);
					}
					base.characterMotor.jumpCount++;
				}
			}
		}
		
	}
}
