using System;
using System.Collections.Generic;
using System.Linq;
using EntityStates;
using EntityStates.Commando;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace Ridley.SkillStates
{
	// Token: 0x02000016 RID: 22
	public class SpacePirateRush : BaseSkillState
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00004E4C File Offset: 0x0000304C
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.aimDirection = base.GetAimRay().direction;
			this.aimDirection.y = Mathf.Clamp(this.aimDirection.y, -0.5f, 0.5f);
			this.stopwatch = 0f;
			this.grabController = new List<GrabController>();
			base.PlayAnimation("FullBody, Override", "SSpecStart", "Slash.playbackRate", this.grabDuration);
			Util.PlaySound("GrabEnter", base.gameObject);
			if (base.characterBody) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
			Transform modelTransform = base.GetModelTransform();
			HitBoxGroup hitBoxGroup = null;
			bool flag = modelTransform;
			if (flag)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "NAir");
			}
			ChildLocator component = this.animator.GetComponent<ChildLocator>();
			bool flag2 = component;
			if (flag2)
			{
				this.sphereCheckTransform = component.FindChild("SphereCheck");
			}
			this.attack = new OverlapAttack();
			this.attack.damageType = DamageType.Generic;
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = base.GetTeam();
			this.attack.damage = this.chargeDamageCoefficient * this.damageStat;
			this.attack.procCoefficient = 1f;
			this.attack.hitEffectPrefab = this.hitEffectPrefab;
			this.attack.forceVector = this.bonusForce;
			this.attack.pushAwayForce = this.chargeImpactForce;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			this.attack.impactSound = this.impactSound;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 14f),
				new Keyframe(0.8f, 0f),
				new Keyframe(1f, 0f)
			});
			this.dragSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.35f, 1f),
				new Keyframe(0.9f, 5f),
				new Keyframe(1f, 5f)
			});
			this.dropSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.9f, 25f),
				new Keyframe(1f, 25f)
			});
			this.subState = SpacePirateRush.SubState.Windup;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000050EC File Offset: 0x000032EC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			bool flag = this.subState == SpacePirateRush.SubState.Windup;
			if (flag)
			{
				bool flag2 = this.stopwatch >= this.windupDuration;
				if (flag2)
				{
					this.stopwatch = 0f;
					this.subState = SpacePirateRush.SubState.DashGrab;
				}
			}
			else
			{
				bool flag3 = this.subState == SpacePirateRush.SubState.DashGrab;
				if (flag3)
				{
					float num = this.dashSpeedCurve.Evaluate(this.stopwatch / this.grabDuration);
					base.characterMotor.rootMotion += this.aimDirection * (num * this.moveSpeedStat * Time.fixedDeltaTime);
					base.characterMotor.velocity.y = 0f;
					bool flag4 = !this.hasGrabbed;
					if (flag4)
					{
						this.AttemptGrab(this.grabRadius);
					}
					bool flag5 = this.hasGrabbed;
					if (flag5)
					{
						this.stopwatch = 0f;
						bool isGrounded = base.isGrounded;
						this.subState = SpacePirateRush.SubState.AirGrabbed;
					}
					else
					{
						bool flag6 = this.stopwatch >= this.grabDuration;
						if (flag6)
						{
							this.stopwatch = 0f;
							this.outer.SetNextStateToMain();
							this.subState = SpacePirateRush.SubState.MissedGrab;
						}
					}
				}
				else
				{
					bool flag7 = this.subState == SpacePirateRush.SubState.AirGrabbed;
					if (flag7)
					{
						bool isGrounded2 = base.isGrounded;
						if (isGrounded2 && this.stopwatch >= this.minDropTime)
						{
							this.dragEffect = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.groundDragEffect, base.FindModelChild("HandL").position, Util.QuaternionSafeLookRotation(Vector3.up));
							//this.dragEffect.transform.parent = base.FindModelChild("HandL");
							float c = ((this.stopwatch / this.maxAirTime) + 1) * this.airTimeDamageCoefficient;
							base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
							EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/beetleguardgroundslam"), new EffectData
							{
								origin = base.transform.position,
								scale = this.groundSlamRadius * c,
							}, true);
							new BlastAttack
							{
								attacker = base.gameObject,
								procChainMask = default(ProcChainMask),
								impactEffect = EffectIndex.Invalid,
								losType = BlastAttack.LoSType.NearestHit,
								damageColorIndex = DamageColorIndex.Default,
								damageType = DamageType.Stun1s,
								procCoefficient = 1f,
								bonusForce = SpacePirateRush.upForce * Vector3.up,
								baseForce = SpacePirateRush.launchForce,
								baseDamage = c * this.groundSlamDamageCoefficient * (this.airTimeDamageCoefficient * this.stopwatch) * this.damageStat,
								falloffModel = BlastAttack.FalloffModel.None,
								radius = this.groundSlamRadius,
								position = base.FindModelChild("HandL").position,
								attackerFiltering = AttackerFiltering.NeverHit,
								teamIndex = base.GetTeam(),
								inflictor = base.gameObject,
								crit = base.RollCrit()
							}.Fire();


							base.modelLocator.normalizeToFloor = true;
							this.subState = SpacePirateRush.SubState.Dragging;
							this.stopwatch = 0f;
						}
						else
						{
							float d = this.dropSpeedCurve.Evaluate(this.stopwatch / this.maxAirTime);
							base.characterMotor.rootMotion += 1.5f * Vector3.down * d * Time.fixedDeltaTime + 0.5f * base.inputBank.moveVector * d * Time.fixedDeltaTime;
						}
					}
					else
					{
						bool flag8 = this.subState == SpacePirateRush.SubState.Dragging;
						if (flag8)
						{
							if (base.characterMotor.lastGroundedTime.timeSince >= 0.125f)
							{
								if(this.dragEffect)
                                {
									EntityState.Destroy(this.dragEffect);
                                }
								this.subState = SubState.AirGrabbed;
								this.stopwatch = 0f;
								this.dragDamageStopwatch = 0f;
								this.animator.SetBool("dragGround", false);
								base.PlayAnimation("FullBody, Override", "SSpecGrabAir", "Slash.playbackRate", this.grabDuration);
								AkSoundEngine.StopPlayingID(this.soundID);
								this.sound = false;
								return;
							}
							RaycastHit raycastHit = default(RaycastHit);
							Vector3 position = base.FindModelChild("HandL").position;
							position.y += 1f;
							Debug.DrawRay(position, Vector3.down);
							if (Physics.Raycast(new Ray(position, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask | LayerIndex.water.mask, QueryTriggerInteraction.Collide))
								this.dragEffect.transform.position = raycastHit.point;
							else
								this.dragEffect.transform.position = raycastHit.point = base.FindModelChild("HandL").position;

							this.dragStopwatch += Time.fixedDeltaTime;
							bool flag9 = !this.sound;
							if (flag9)
							{
								base.PlayAnimation("FullBody, Override", "SSpecGrab", "Slash.playbackRate", this.grabDuration);
								this.sound = true;
								Util.PlaySound("GrabHitGround", base.gameObject);
								this.soundID = Util.PlaySound("DragLoop", base.gameObject);
							}
							this.animator.SetBool("dragGround", true);
							
							this.dragDamageStopwatch += Time.fixedDeltaTime;
							bool flag10 = this.dragDamageStopwatch >= this.dragDamageInterval;
							if (flag10)
							{
								this.DamageTargets();
								this.dragDamageStopwatch = 0f;
							}
							float d2 = this.dragSpeedCurve.Evaluate(this.stopwatch / this.dragMaxSpeedTime);							
							this.targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(this.targetMoveVector, base.inputBank.aimDirection, ref this.targetMoveVectorVelocity, SpacePirateRush.turnSmoothTime, SpacePirateRush.turnSpeed), Vector3.up).normalized;
							base.characterDirection.moveVector = this.targetMoveVector;
							Vector3 forward = base.characterDirection.forward;
							base.characterMotor.moveDirection = forward * d2;
							List<HurtBox> list = new List<HurtBox>();
							foreach(GrabController controller in this.grabController)
                            {
								if(controller.body)
                                {
									HealthComponent healthComponent = controller.body.healthComponent;
									if(healthComponent)
                                    {
										this.attack.ignoredHealthComponentList.Add(healthComponent);
                                    }
								}
								
                            }
							bool flag11 = this.attack.Fire(list);
							if (flag11)
							{
								foreach (HurtBox hurtBox in list)
								{
									bool flag12 = (hurtBox.healthComponent && hurtBox.healthComponent.body && hurtBox.healthComponent.body.isChampion) || hurtBox.healthComponent.body.isBoss;
									if (flag12)
									{
										if (this.dragEffect)
										{
											EntityState.Destroy(this.dragEffect);
										}
										this.subState = SpacePirateRush.SubState.GrabWall;
										this.stopwatch = 0f;
										return;
									}
								}
							}
							bool flag13 = this.dragStopwatch >= this.dragDuration || base.inputBank.jump.justPressed;
							if (flag13)
							{
								if (this.dragEffect)
								{
									EntityState.Destroy(this.dragEffect);
								}
								this.subState = SpacePirateRush.SubState.DragLaunch;
								AkSoundEngine.StopPlayingID(this.soundID);
								Util.PlaySound("DragLaunch", base.gameObject);
								Util.PlaySound("DragLaunchVoice", base.gameObject);
								base.PlayAnimation("FullBody, Override", "DragEnd", "Slash.playbackRate", this.grabDuration);
								foreach (GrabController grabController in this.grabController)
								{
									bool flag14 = grabController;
									if (flag14)
									{
										grabController.Launch(base.characterMotor.moveDirection.normalized * SpacePirateRush.launchForce + Vector3.up * SpacePirateRush.upForce);
										base.modelLocator.normalizeToFloor = true;
									}
								}
								this.stopwatch = 0f;
							}
						}
						else
						{
							bool flag15 = this.subState == SpacePirateRush.SubState.GrabWall;
							if (flag15)
							{
								float bonusDamage = this.velocityDamageCoefficient * base.characterMotor.velocity.magnitude * this.damageStat;
								base.characterMotor.moveDirection = Vector3.zero;
								Util.PlaySound("JabHit3", base.gameObject);
								AkSoundEngine.StopPlayingID(this.soundID);
								base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
								base.PlayAnimation("FullBody, Override", "DragWall", "Slash.playbackRate", this.grabDuration);
								EffectManager.SpawnEffect(GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab, new EffectData
								{
									origin = base.transform.position,
									scale = this.wallBlastRadius
								}, true);
								new BlastAttack
								{
									attacker = base.gameObject,
									procChainMask = default(ProcChainMask),
									impactEffect = EffectIndex.Invalid,
									losType = BlastAttack.LoSType.NearestHit,
									damageColorIndex = DamageColorIndex.Default,
									damageType = DamageType.Stun1s,
									procCoefficient = 1f,
									bonusForce = SpacePirateRush.upForce * Vector3.up,
									baseForce = SpacePirateRush.launchForce,
									baseDamage = this.wallDamageCoefficient * this.damageStat + bonusDamage,
									falloffModel = BlastAttack.FalloffModel.None,
									radius = this.wallBlastRadius,
									position = base.FindModelChild("HandL").position,
									attackerFiltering = AttackerFiltering.NeverHit,
									teamIndex = base.GetTeam(),
									inflictor = base.gameObject,
									crit = base.RollCrit()
								}.Fire();
								this.subState = SpacePirateRush.SubState.Exit;
							}
							else
							{
								base.characterMotor.velocity /= 3f;
								base.characterMotor.moveDirection = Vector3.zero;
								bool flag16 = this.stopwatch >= this.exitDuration;
								if (flag16)
								{
									this.outer.SetNextStateToMain();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000057B4 File Offset: 0x000039B4
		private void DamageTargets()
		{
			foreach (GrabController grabController in this.grabController)
			{
				bool flag = grabController;
				if (flag)
				{
					DamageInfo damageInfo = new DamageInfo
					{
						position = grabController.body.transform.position,
						attacker = base.gameObject,
						inflictor = base.gameObject,
						damage = this.dragDamageCoefficient * this.damageStat,
						damageColorIndex = DamageColorIndex.Default,
						damageType = DamageType.Generic,
						crit = base.RollCrit(),
						force = Vector3.zero,
						procChainMask = default(ProcChainMask),
						procCoefficient = 0f
					};
					bool flag2 = grabController.body && grabController.body.healthComponent;
					if (flag2)
					{
						grabController.body.healthComponent.TakeDamage(damageInfo);
						this.ForceFlinch(grabController.body);
					}
				}
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000058E4 File Offset: 0x00003AE4
		public override void OnExit()
		{
			base.OnExit();
			if (this.dragEffect)
			{
				EntityState.Destroy(this.dragEffect);
			}
			AkSoundEngine.StopPlayingID(this.soundID);
			base.modelLocator.normalizeToFloor = false;
			this.animator.SetBool("dragGround", false);
			bool flag = base.cameraTargetParams;
			if (flag)
			{
				base.cameraTargetParams.fovOverride = -1f;
			}
			bool flag2 = this.grabController.Count > 0;
			if (flag2)
			{
				foreach (GrabController grabController in this.grabController)
				{
					bool flag3 = grabController;
					if (flag3)
					{
						grabController.Release();
					}
				}
			}
			bool active = NetworkServer.active;
			bool flag4 = active;
			if (flag4)
			{
				base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000059B8 File Offset: 0x00003BB8
		protected virtual void ForceFlinch(CharacterBody body)
		{
			SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
			bool flag = component == null;
			if (!flag)
			{
				bool canBeHitStunned = component.canBeHitStunned;
				if (canBeHitStunned)
				{
					component.SetPain();
				}
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000059F4 File Offset: 0x00003BF4
		public void AttemptGrab(float grabRadius)
		{
			Ray aimRay = base.GetAimRay();
			BullseyeSearch bullseyeSearch = new BullseyeSearch
			{
				teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam()),
				filterByLoS = false,
				searchOrigin = base.transform.position,
				searchDirection = UnityEngine.Random.onUnitSphere,
				sortMode = BullseyeSearch.SortMode.Distance,
				maxDistanceFilter = grabRadius,
				maxAngleFilter = 360f
			};
			bullseyeSearch.RefreshCandidates();
			bullseyeSearch.FilterOutGameObject(base.gameObject);
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			foreach (HurtBox hurtBox in list)
			{
				bool flag = hurtBox;
				if (flag)
				{
					bool flag2 = hurtBox.healthComponent && hurtBox.healthComponent.body;
					if (flag2)
					{
						bool flag3 = !hurtBox.healthComponent.body.isChampion;
						if (flag3)
						{
							bool flag4 = !this.playedGrabSound;
							if (flag4)
							{
								Util.PlaySound("HenrySwordSwing", base.gameObject);
								this.playedGrabSound = true;
							}
							GrabController grabController = hurtBox.healthComponent.body.gameObject.AddComponent<GrabController>();
							grabController.pivotTransform = base.FindModelChild("HandL");
							this.grabController.Add(grabController);
							this.ForceFlinch(hurtBox.healthComponent.body);
							bool isGrounded = base.isGrounded;
							base.PlayAnimation("FullBody, Override", "SSpecGrabAir", "Slash.playbackRate", this.grabDuration);
							this.hasGrabbed = true;
							Util.PlaySound("GrabHit", base.gameObject);
							base.SmallHop(base.characterMotor, this.smallHopVelocity);
						}
					}
				}
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00005C14 File Offset: 0x00003E14
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		private float airTimeDamageCoefficient = 1.5f;

		private GameObject dragEffect;

		private float minDropTime = 0.35f;

		private float attackRecoil = 7f;
		// Token: 0x040000A5 RID: 165
		protected AnimationCurve dashSpeedCurve;

		protected AnimationCurve dragSpeedCurve;
		// Token: 0x040000A6 RID: 166
		protected AnimationCurve dropSpeedCurve;

		// Token: 0x040000A7 RID: 167
		private float grabDuration = 0.4f;

		// Token: 0x040000A8 RID: 168
		private Vector3 targetMoveVector;

		// Token: 0x040000A9 RID: 169
		private Vector3 targetMoveVectorVelocity;

		private float velocityDamageCoefficient = 0.5f;
		// Token: 0x040000AA RID: 170
		private float wallDamageCoefficient = 12f;

		// Token: 0x040000AB RID: 171
		private float wallBlastRadius = 12f;

		// Token: 0x040000AC RID: 172
		private Vector3 wallHitPoint;

		// Token: 0x040000AD RID: 173
		public static float upForce = 3000f;

		// Token: 0x040000AE RID: 174
		public static float launchForce = 1750f;

		// Token: 0x040000AF RID: 175
		public static float turnSmoothTime = 0.01f;

		// Token: 0x040000B0 RID: 176
		public static float turnSpeed = 20f;

		// Token: 0x040000B1 RID: 177
		public static float dragMaxSpeedCoefficient = 5f;

		// Token: 0x040000B2 RID: 178
		private float dragDamageCoefficient = 0.75f;

		// Token: 0x040000B3 RID: 179
		private float dragDamageInterval = 0.1f;

		// Token: 0x040000B4 RID: 180
		private float dragDamageStopwatch;

		private float dragStopwatch;
		// Token: 0x040000B5 RID: 181
		private float dragDuration = 1.5f;

		// Token: 0x040000B6 RID: 182
		private float dragMaxSpeedTime = 0.8f;

		// Token: 0x040000B7 RID: 183
		private Transform sphereCheckTransform;

		// Token: 0x040000B8 RID: 184
		private float maxAirTime = 0.67f;

		// Token: 0x040000B9 RID: 185
		private float airStopwatch;

		// Token: 0x040000BA RID: 186
		private float smallHopVelocity = 12f;

		// Token: 0x040000BB RID: 187
		private float windupDuration = 0.3f;

		// Token: 0x040000BC RID: 188
		private float exitDuration = 0.5f;

		// Token: 0x040000BD RID: 189
		protected GameObject swingEffectPrefab;

		// Token: 0x040000BE RID: 190
		protected GameObject hitEffectPrefab;

		// Token: 0x040000BF RID: 191
		protected NetworkSoundEventIndex impactSound;

		private float groundSlamDamageCoefficient = 3.75f;
		// Token: 0x040000C0 RID: 192
		private float chargeDamageCoefficient = 2.5f;

		// Token: 0x040000C1 RID: 193
		private float chargeImpactForce = 2000f;

		// Token: 0x040000C2 RID: 194
		private Vector3 bonusForce = Vector3.up * 2000f;

		// Token: 0x040000C3 RID: 195
		private Vector3 aimDirection;

		// Token: 0x040000C4 RID: 196
		private List<GrabController> grabController;

		// Token: 0x040000C5 RID: 197
		private float stopwatch;

		// Token: 0x040000C6 RID: 198
		private Animator animator;

		// Token: 0x040000C7 RID: 199
		private bool hasGrabbed;

		// Token: 0x040000C8 RID: 200
		private OverlapAttack attack;

		// Token: 0x040000C9 RID: 201
		private float grabRadius = 8f;
		private float groundSlamRadius = 4f;
		// Token: 0x040000CA RID: 202
		private bool playedGrabSound = false;

		// Token: 0x040000CB RID: 203
		private SpacePirateRush.SubState subState;

		// Token: 0x040000CC RID: 204
		public static float dodgeFOV = DodgeState.dodgeFOV;

		// Token: 0x040000CD RID: 205
		private bool sound;

		// Token: 0x040000CE RID: 206
		private uint soundID;

		// Token: 0x040000CF RID: 207
		private bool s;

		// Token: 0x0200002D RID: 45
		private enum SubState
		{
			// Token: 0x0400015F RID: 351
			Windup,
			// Token: 0x04000160 RID: 352
			DashGrab,
			// Token: 0x04000161 RID: 353
			MissedGrab,
			// Token: 0x04000162 RID: 354
			AirGrabbed,
			// Token: 0x04000163 RID: 355
			Dragging,
			// Token: 0x04000164 RID: 356
			GrabWall,
			// Token: 0x04000165 RID: 357
			DragLaunch,
			// Token: 0x04000166 RID: 358
			Exit
		}
	}
}
