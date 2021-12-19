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
	public class SpacePirateRush : BaseSkillState
	{
		private float finalAirTime;
		private Vector3 lastSafeFootPosition;
		private float airTimeDamageCoefficient = 2.5f;
		private GameObject fireEffect;
		private GameObject dragEffect;
		private float minDropTime = 0.35f;
		private float attackRecoil = 7f;
		protected AnimationCurve dashSpeedCurve;
		protected AnimationCurve dragSpeedCurve;
		protected AnimationCurve dropSpeedCurve;
		private float grabDuration = 0.4f;
		private Vector3 targetMoveVector;
		private Vector3 targetMoveVectorVelocity;
		private float velocityDamageCoefficient = 0.7f;
		private float wallDamageCoefficient = 9f;
		private float wallBlastRadius = 12f;
		private Vector3 wallHitPoint;
		public static float upForce = 2000f;
		public static float launchForce = 1750f;
		public static float turnSmoothTime = 0.01f;
		public static float turnSpeed = 20f;
		public static float dragMaxSpeedCoefficient = 5f;
		private float dragDamageCoefficient = 0.75f;
		private float dragDamageInterval = 0.1f;
		private float dragDamageStopwatch;
		private float dragStopwatch;
		private float dragDuration = 2f;
		private float dragMaxSpeedTime = 0.8f;
		private Transform sphereCheckTransform;
		private float maxAirTime = 0.67f;
		private float airStopwatch;
		private float smallHopVelocity = 12f;
		private float windupDuration = 0.3f;
		private float exitDuration = 0.5f;
		protected GameObject swingEffectPrefab;
		protected GameObject hitEffectPrefab;
		protected NetworkSoundEventIndex impactSound;
		private float groundSlamDamageCoefficient = 2.9f;
		private float chargeDamageCoefficient = 2.2f;
		private float chargeImpactForce = 2000f;
		private Vector3 bonusForce = Vector3.up * 2000f;
		private Vector3 aimDirection;
		private List<GrabController> grabController;
		private float stopwatch;
		private Animator animator;
		private bool hasGrabbed;
		private OverlapAttack attack;
		private float grabRadius = 8f;
		private float groundSlamRadius = 4f;
		private bool playedGrabSound = false;
		private SpacePirateRush.SubState subState;
		public static float dodgeFOV = DodgeState.dodgeFOV;
		private bool sound;
		private uint soundID;
		private bool s;

		private enum SubState
		{
			Windup,
			DashGrab,
			MissedGrab,
			AirGrabbed,
			Dragging,
			GrabWall,
			DragLaunch,
			Exit
		}
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.aimDirection = base.GetAimRay().direction;
			this.aimDirection.y = Mathf.Clamp(this.aimDirection.y, -0.5f, 0.5f);
			this.stopwatch = 0f;
			this.grabController = new List<GrabController>();

			//this.fireEffect = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.grabFireEffect, base.FindModelChild("HandL2"));
			base.PlayAnimation("FullBody, Override", "SSpecStart", "Slash.playbackRate", this.grabDuration);
			Util.PlaySound("GrabEnter", base.gameObject);
			if (base.characterBody) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
			Transform modelTransform = base.GetModelTransform();
			HitBoxGroup hitBoxGroup = null;
			if (modelTransform)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "NAir");
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

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.subState == SpacePirateRush.SubState.Windup)
			{
				if (this.stopwatch >= this.windupDuration)
				{
					this.stopwatch = 0f;
					this.subState = SpacePirateRush.SubState.DashGrab;
				}
			}
			else
			{
				if (this.subState == SpacePirateRush.SubState.DashGrab)
				{
					float num = this.dashSpeedCurve.Evaluate(this.stopwatch / this.grabDuration);
					base.characterMotor.rootMotion += this.aimDirection * (num * this.moveSpeedStat * Time.fixedDeltaTime);
					base.characterMotor.velocity.y = 0f;
					if (!this.hasGrabbed)
					{
						this.AttemptGrab(this.grabRadius);
					}
					if (this.hasGrabbed)
					{
						this.stopwatch = 0f;
						this.subState = SpacePirateRush.SubState.AirGrabbed;						
					}
					else
					{
						if (this.stopwatch >= this.grabDuration)
						{
							if (this.fireEffect) EntityState.Destroy(this.fireEffect);
							this.stopwatch = 0f;
							this.outer.SetNextStateToMain();
							this.subState = SpacePirateRush.SubState.MissedGrab;
						}
					}
				}
				else
				{
					if (this.subState == SpacePirateRush.SubState.AirGrabbed)
					{
						if ((base.isGrounded || base.inputBank.jump.justPressed) && this.stopwatch >= this.minDropTime)
						{
							this.targetMoveVector = Vector3.zero;
							this.dragEffect = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.groundDragEffect, base.FindModelChild("HandL").position, Util.QuaternionSafeLookRotation(Vector3.up));
							//this.dragEffect.transform.parent = base.FindModelChild("HandL");
							this.finalAirTime = (this.stopwatch / this.maxAirTime);
							float c = (finalAirTime + 1) * this.airTimeDamageCoefficient;
							float attackRecoil = this.attackRecoil;
							base.AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
							EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/beetleguardgroundslam"), new EffectData
							{
								origin = base.transform.position,
								scale = this.groundSlamRadius * c,
							}, true);
							
							BlastAttack.Result result = new BlastAttack
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
								baseDamage = c * this.groundSlamDamageCoefficient * this.damageStat,
								falloffModel = BlastAttack.FalloffModel.SweetSpot,
								radius = this.groundSlamRadius * c,
								position = base.FindModelChild("HandL").position,
								attackerFiltering = AttackerFiltering.NeverHit,
								teamIndex = base.GetTeam(),
								inflictor = base.gameObject,
								crit = base.RollCrit()
							}.Fire();
							base.modelLocator.normalizeToFloor = true;
							this.subState = SpacePirateRush.SubState.Dragging;
							this.stopwatch = 0f;

							base.PlayAnimation("FullBody, Override", "SSpecGrab", "Slash.playbackRate", this.grabDuration);
							this.sound = true;
							Util.PlaySound("GrabHitGround", base.gameObject);
							this.soundID = Util.PlaySound("DragLoop", base.gameObject);
							this.animator.SetBool("dragGround", true);
						}
						else
						{
							float d = this.dropSpeedCurve.Evaluate(this.stopwatch / this.maxAirTime);
							base.characterMotor.rootMotion += 1.5f * Vector3.down * d * Time.fixedDeltaTime + 0.5f * base.inputBank.moveVector * d * Time.fixedDeltaTime;
						}
					}
					else
					{
						if (this.subState == SpacePirateRush.SubState.Dragging)
						{
							if (base.characterMotor.lastGroundedTime.timeSince >= 0.15f)
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
							if(this.dragEffect)
                            {
								if (Physics.Raycast(new Ray(position, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
									this.dragEffect.transform.position = raycastHit.point;
								else
									this.dragEffect.transform.position = raycastHit.point = base.FindModelChild("HandL").position;
							}							
							this.dragStopwatch += Time.fixedDeltaTime;						
							
							this.dragDamageStopwatch += Time.fixedDeltaTime;
							if (this.dragDamageStopwatch >= this.dragDamageInterval)
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

							if (this.attack.Fire(list))
							{
								foreach (HurtBox hurtBox in list)
								{
									if ((hurtBox.healthComponent && hurtBox.healthComponent.body && hurtBox.healthComponent.body.isChampion) || hurtBox.healthComponent.body.isBoss)
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

							if (this.dragStopwatch >= this.dragDuration || base.inputBank.jump.justPressed)
							{
								if (this.dragEffect)
								{
									EntityState.Destroy(this.dragEffect);
								}
								this.exitSpeed = d2;
								this.subState = SpacePirateRush.SubState.DragLaunch;
								AkSoundEngine.StopPlayingID(this.soundID);
								Util.PlaySound("DragLaunch", base.gameObject);
								Util.PlaySound("DragLaunchVoice", base.gameObject);
								this.lastSafeFootPosition = base.characterBody.footPosition;
								base.PlayAnimation("FullBody, Override", "DragEnd", "Slash.playbackRate", this.grabDuration);
								foreach (GrabController grabController in this.grabController)
								{
									if (grabController)
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
							if (this.subState == SpacePirateRush.SubState.GrabWall)
							{
								float f = Mathf.Max(this.velocityDamageCoefficient * base.characterMotor.velocity.magnitude, (finalAirTime + 1) * this.airTimeDamageCoefficient);
								float bonusDamage = f * this.damageStat;
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
								if (this.subState == SubState.DragLaunch)
									base.characterMotor.moveDirection = base.characterDirection.forward * this.exitSpeed * Mathf.Lerp(1f, 0f, this.stopwatch / this.exitDuration);
								else
                                {
									base.characterMotor.velocity = Vector3.zero;
									base.characterMotor.moveDirection = Vector3.zero;
								}
								base.characterMotor.velocity = Vector3.zero; ////////delet
								base.characterMotor.moveDirection = Vector3.zero;

								if (this.stopwatch >= this.exitDuration)
								{
									this.outer.SetNextStateToMain();
								}
							}
						}
					}
				}
			}
		}

		private float exitSpeed;
		private void DamageTargets()
		{
			foreach (GrabController grabController in this.grabController)
			{
				if (grabController)
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
					if (grabController.body && grabController.body.healthComponent)
					{
						grabController.body.healthComponent.TakeDamage(damageInfo);
						this.ForceFlinch(grabController.body);
					}
				}
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			if (this.dragEffect)
			{
				EntityState.Destroy(this.dragEffect);
			}
			if (this.fireEffect) EntityState.Destroy(this.fireEffect);

			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(base.characterBody.footPosition, Vector3.down), out raycastHit, 100f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
				base.transform.position = this.lastSafeFootPosition + Vector3.up * 5;
			AkSoundEngine.StopPlayingID(this.soundID);
			base.modelLocator.normalizeToFloor = false;
			this.animator.SetBool("dragGround", false);
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = -1f;
			}
			if (this.grabController.Count > 0)
			{
				foreach (GrabController grabController in this.grabController)
				{
					if (grabController)
					{
						grabController.Release();
					}
				}
			}
			if (NetworkServer.active)
			{
				base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
			}
		}
		protected virtual void ForceFlinch(CharacterBody body)
		{
			SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
			if (component)
			{
				if (component.canBeHitStunned)
				{
					component.SetPain();
				}
				else if(component.canBeStunned)
                {
					component.SetStun(1f);					
                }
				foreach (EntityStateMachine e in body.gameObject.GetComponents<EntityStateMachine>())
				{
					if (e && e.customName.Equals("Weapon"))
					{
						e.SetNextStateToMain();
					}
				}
			}
		}
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
				if (hurtBox)
				{
					if (hurtBox.healthComponent && hurtBox.healthComponent.body)
					{
						if (!hurtBox.healthComponent.body.isChampion || (hurtBox.healthComponent.gameObject.name.Contains("Brother") && hurtBox.healthComponent.gameObject.name.Contains("Body")))
						{
							if (this.playedGrabSound)
							{
								Util.PlaySound("HenrySwordSwing", base.gameObject);
								this.playedGrabSound = true;
							}
							Vector3 between = hurtBox.healthComponent.transform.position - base.transform.position;
							Vector3 v = between / 4f;
							v.y = Math.Max(v.y, between.y);
							base.characterMotor.AddDisplacement(v);
							GrabController grabController = hurtBox.healthComponent.body.gameObject.AddComponent<GrabController>();
							grabController.pivotTransform = base.FindModelChild("HandL");
							if (this.fireEffect) EntityState.Destroy(this.fireEffect);
							this.grabController.Add(grabController);
							this.ForceFlinch(hurtBox.healthComponent.body);
							base.PlayAnimation("FullBody, Override", "SSpecGrabAir", "Slash.playbackRate", this.grabDuration);
							this.hasGrabbed = true;
							Util.PlaySound("GrabHit", base.gameObject);
							base.SmallHop(base.characterMotor, this.smallHopVelocity);
						}
					}
				}
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		
	}
}
