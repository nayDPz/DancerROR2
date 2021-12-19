using System;
using EntityStates;
using RoR2;
using UnityEngine;

namespace Ridley.SkillStates
{
	public class RidleyMain : GenericCharacterMain
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
			bool flag = base.characterBody.isSprinting && !this.sprintSoundOn && base.isGrounded;
			if (flag)
			{
				//this.sprintEffect1 = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.footDragEffect, footL.position, Util.QuaternionSafeLookRotation(Vector3.up));
				//this.sprintEffect2 = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.footDragEffect, footR.position, Util.QuaternionSafeLookRotation(Vector3.up));
				this.sprintSoundOn = true;
				base.characterBody.acceleration = this.sprintAcceleration;
				AkSoundEngine.StopPlayingID(this.sprintSoundID);
				this.sprintSoundID = Util.PlaySound("RidleySprintStart", base.gameObject);
			}
			//if(this.sprintEffect1)
			//	this.sprintEffect1.transform.position = footL.position;
			//if(this.sprintEffect2)
			//	this.sprintEffect2.transform.position = footR.position;
			bool flag2 = !base.characterBody.isSprinting || !base.isGrounded;
			if (flag2)
			{
				bool flag3 = this.sprintSoundOn;
				if (flag3)
				{
					//if (this.sprintEffect1)
					//	EntityState.Destroy(this.sprintEffect1);
					//if (this.sprintEffect2)
					//	EntityState.Destroy(this.sprintEffect2);
					base.characterBody.acceleration = this.baseAcceleration;
					this.sprintSoundOn = false;
					AkSoundEngine.StopPlayingID(this.sprintSoundID);
				}
			}


			if (base.isAuthority && base.characterMotor.isGrounded && !this.localUser.isUIFocused)
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

        // Token: 0x0600003F RID: 63 RVA: 0x000042F4 File Offset: 0x000024F4
        public override void ProcessJump()
		{
			bool hasCharacterMotor = this.hasCharacterMotor;
			if (hasCharacterMotor)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount;
				if (flag3)
				{
					int itemCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
					float horizontalBonus = 1f;
					float verticalBonus = 1f;
					bool flag4 = base.characterMotor.jumpCount >= base.characterBody.baseJumpCount;
					if (flag4)
					{
						flag = true;
						horizontalBonus = 1.5f;
						verticalBonus = 1.5f;
					}
					else
					{
						bool flag5 = (float)itemCount > 0f && base.characterBody.isSprinting;
						if (flag5)
						{
							float num = base.characterBody.acceleration * base.characterMotor.airControl;
							bool flag6 = base.characterBody.moveSpeed > 0f && num > 0f;
							if (flag6)
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
						bool flag7 = layerIndex >= 0;
						if (flag7)
						{
							bool flag8 = base.characterMotor.jumpCount == 0 || base.characterBody.baseJumpCount == 1;
							if (flag8)
							{
								base.modelAnimator.CrossFadeInFixedTime("Jump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								Util.PlaySound("RidleyJump", base.gameObject);
							}
							else
							{
								base.modelAnimator.CrossFadeInFixedTime("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								Util.PlaySound("RidleyJumpAir", base.gameObject);
							}
						}
					}
					bool flag9 = flag;
					if (flag9)
					{
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
						{
							origin = base.characterBody.footPosition
						}, true);
					}
					else
					{
						bool flag10 = base.characterMotor.jumpCount > 0;
						if (flag10)
						{
							EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
							{
								origin = base.characterBody.footPosition,
								scale = base.characterBody.radius
							}, true);
						}
					}
					bool flag11 = flag2;
					if (flag11)
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

		// Token: 0x0400008A RID: 138
		private uint sprintSoundID;

		// Token: 0x0400008B RID: 139
		private bool sprintSoundOn;
	}
}
