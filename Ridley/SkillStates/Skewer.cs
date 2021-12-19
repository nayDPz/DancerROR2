using System;
using System.Collections.Generic;
using EntityStates;
using KinematicCharacterController;
using Ridley.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace Ridley.SkillStates
{
	public class Skewer : BaseSkillState
	{
		public static float damageCoefficient = 5f;
		public static float procCoefficient = 1f;
		public static float baseDuration = 1.5f;
		public static float force = 0f;
		public static float recoil = 1f;
		public static float range = 43f;
		public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerHuntressSnipe");
		public static GameObject muzzleEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashHuntress");
		public static GameObject hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/HitEffect/HitsparkCaptainShotgun");
		private float exitTime = 0.15f;
		private float skewerExitTime = 0.15f;
		private float pullTime = 0.25f;
		private float skewerTime = 0.375f;
		private float stopwatch;
		private Vector3 pullPoint;
		private float duration;
		private float fireTime;
		private bool hasFired;
		private float hitTime;
		private Animator animator;
		private string muzzleString;
		private static float antigravityStrength;
		private List<HurtBox> hitHurtBoxes;
		private Skewer.SubState subState;
		private bool a;
		private enum SubState
		{
			Skewer,
			SkewerHit,
			Pull,
			Exit
		}

		private Ray aimRay;
		private float attackRecoil = 7f;
		private List<Transform> tailTransforms;
		public override void OnEnter()
		{
			base.OnEnter();
			this.tailTransforms = new List<Transform>();
			this.skewerTime /= this.attackSpeedStat;
			this.duration = Skewer.baseDuration / this.attackSpeedStat;
			this.fireTime = 0.4f * this.duration;
			if(base.modelLocator)
            {
				//GetTailTransforms();
            }
			base.characterBody.SetAimTimer(2f);
			base.characterDirection.forward = base.GetAimRay().direction;
			this.hitHurtBoxes = new List<HurtBox>();
			this.animator = base.GetModelAnimator();
			this.muzzleString = "Muzzle";
			base.PlayAnimation("FullBody, Override", "DownSpecial", "Slash.playbackrate", this.duration);
			Util.PlaySound("DSpecialStart", base.gameObject);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		private void Fire()
		{
			if (!this.hasFired)
			{
				this.hasFired = true;
				base.characterBody.AddSpreadBloom(1.5f);
				EffectManager.SimpleMuzzleFlash(Skewer.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
				if (base.isAuthority)
				{
					this.aimRay = base.GetAimRay();
					this.pullPoint = aimRay.GetPoint(3f);
					this.pullPoint.y = base.transform.position.y + 1f;
					Vector3 direction = aimRay.direction;
					//direction.y = Mathf.Clamp(aimRay.direction.y, -0.25f, 0.25f);
					aimRay.direction = direction;
					base.AddRecoil(-1f * Skewer.recoil, -2f * Skewer.recoil, -0.5f * Skewer.recoil, 0.5f * Skewer.recoil);
					BulletAttack bulletAttack = new BulletAttack
					{
						bulletCount = 1U,
						aimVector = aimRay.direction,
						origin = aimRay.origin,
						damage = this.damageStat * Skewer.damageCoefficient,
						damageColorIndex = DamageColorIndex.Default,
						damageType = DamageType.Stun1s,
						falloffModel = BulletAttack.FalloffModel.DefaultBullet,
						maxDistance = Skewer.range,
						force = Skewer.force,
						hitMask = LayerIndex.CommonMasks.bullet,
						minSpread = 0f,
						maxSpread = 0f,
						isCrit = base.RollCrit(),
						owner = base.gameObject,
						muzzleName = this.muzzleString,
						smartCollision = true,
						procChainMask = default(ProcChainMask),
						procCoefficient = Skewer.procCoefficient,
						radius = 2f,
						sniper = false,
						stopperMask = LayerIndex.world.mask,
						weapon = null,
						tracerEffectPrefab = Skewer.tracerEffectPrefab,
						spreadPitchScale = 0f,
						spreadYawScale = 0f,
						queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
						hitEffectPrefab = Skewer.muzzleEffectPrefab
					};
					bulletAttack.hitCallback = delegate (ref BulletAttack.BulletHit hitInfo)
					{
						bool result = bulletAttack.DefaultHitCallback(ref hitInfo);
						if(hitInfo.hitHurtBox && result)
                        {
							this.hitHurtBoxes.Add(hitInfo.hitHurtBox);
							if (hitInfo.hitHurtBox.healthComponent && hitInfo.hitHurtBox.healthComponent.body)
							{
								EntityStateMachine component = hitInfo.hitHurtBox.healthComponent.body.GetComponent<EntityStateMachine>();
								if (hitInfo.hitHurtBox.healthComponent.body.GetComponent<SetStateOnHurt>().canBeFrozen && component)
								{
									SkeweredState newNextState = new SkeweredState
									{
										duration = this.skewerTime
									};
									component.SetInterruptState(newNextState, InterruptPriority.Frozen);
								}
							}
							this.OnHitEnemyAuthority();
						}
						
						return result;
					};
					bulletAttack.Fire();
				}
			}
		}

		private void OnHitEnemyAuthority()
		{
			base.characterMotor.velocity = Vector3.zero;
			base.AddRecoil(-1f * this.attackRecoil / 2, -2f * this.attackRecoil / 2, -0.5f * this.attackRecoil / 2, 0.5f * this.attackRecoil / 2);
		}


		private void GetTailTransforms()
        {
			if (base.modelLocator)
			{
				Transform modelTransform = modelLocator.modelTransform;
				if (modelTransform) // lol
				{
					//Transform tail = modelTransform.Find("model-armature/Trans/Rot/Hip/Tail/Tail1/Tail2/Tail3/");
					//if(tail) tailTransforms.Add(tail);
					Transform tail = modelTransform.Find("model-armature/Trans/Rot/Hip/Tail/Tail1/Tail2/Tail3/Tail4");
					if (tail) tailTransforms.Add(tail);
					tail = modelTransform.Find("model-armature/Trans/Rot/Hip/Tail/Tail1/Tail2/Tail3/Tail4/Tail5");
					if (tail) tailTransforms.Add(tail);
					tail = modelTransform.Find("model-armature/Trans/Rot/Hip/Tail/Tail1/Tail2/Tail3/Tail4/Tail5/Tail6");
					if (tail) tailTransforms.Add(tail);
					tail = modelTransform.Find("model-armature/Trans/Rot/Hip/Tail/Tail1/Tail2/Tail3/Tail4/Tail5/Tail6/Tail7");
					if (tail) tailTransforms.Add(tail);
					tail = modelTransform.Find("model-armature/Trans/Rot/Hip/Tail/Tail1/Tail2/Tail3/Tail4/Tail5/Tail6/Tail7/Tail8");
					if (tail) tailTransforms.Add(tail);

				}
			}
		}
        public override void Update()
        {
            base.Update();
			//if (this.subState == Skewer.SubState.SkewerHit)
				//SetTailPosition();
        }
        private void SetTailPosition()
        {
			Vector3 vector = this.aimRay.direction * Skewer.range;
			float distanceBetweenBones = vector.magnitude / tailTransforms.Count;
			for(int i = 0; i < tailTransforms.Count; i++)
            {
				if(tailTransforms[i])
					tailTransforms[i].position = this.aimRay.GetPoint(distanceBetweenBones * (i + 1));
            }
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.subState == Skewer.SubState.Skewer)
			{
				if (this.stopwatch >= this.fireTime)
				{
					this.Fire();
				}
				if (this.hasFired)
				{
					if (this.hitHurtBoxes.Count > 0)
					{
						base.PlayAnimation("FullBody, Override", "DownSpecialHit", "Slash.playbackrate", this.duration * 0.5f);
						Util.PlaySound("DSpecialHit", base.gameObject);
						this.subState = Skewer.SubState.SkewerHit;
						this.stopwatch = 0f;
					}
					else
					{
						if (!this.a)
						{
							Util.PlaySound("DSpecialSwing", base.gameObject);
						}
						this.a = true;
						this.subState = Skewer.SubState.Exit;
						this.stopwatch = 0f;
					}
				}
			}
			else
			{
				if (this.subState == Skewer.SubState.SkewerHit)
				{
					base.GetModelAnimator().SetFloat("Slash.playbackRate", 0f);
					base.characterMotor.velocity.y = 0f;
					foreach (HurtBox hurtBox in this.hitHurtBoxes)
					{
						if (hurtBox)
						{
							HealthComponent healthComponent = hurtBox.healthComponent;
							if (healthComponent && healthComponent.body)
							{
								CharacterBody body = healthComponent.body;
								if (body.characterMotor)
								{
									body.characterMotor.velocity = Vector3.zero;
								}
							}
						}
					}
					if (this.stopwatch >= this.skewerTime)
					{
						base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
						base.GetModelAnimator().SetFloat("Slash.playbackRate", 1f);
						Util.PlaySound("DSpecialPull", base.gameObject);
						this.stopwatch = 0f;
						this.subState = Skewer.SubState.Pull;
					}
				}
				else
				{
					if (this.subState == Skewer.SubState.Pull)
					{
						foreach (HurtBox hurtBox in this.hitHurtBoxes)
						{
							if (hurtBox)
							{
								HealthComponent healthComponent2 = hurtBox.healthComponent;
								if (healthComponent2 && healthComponent2.body)
								{
									if (this.stopwatch < this.pullTime)
									{
										CharacterBody body = healthComponent2.body;
										float num = this.pullTime - this.stopwatch;
										Vector3 vector = this.pullPoint - body.coreTransform.position;
										float num2 = vector.magnitude / num;
										Vector3 normalized = vector.normalized;
										float num3 = Mathf.Lerp(2f, 0f, this.stopwatch / this.pullTime);
										if (body.isChampion)
										{
											num3 /= 2f;
										}
										num2 *= num3;
										if (body.characterMotor)
										{
											if (body.gameObject.GetComponent<KinematicCharacterMotor>())
											{
												body.gameObject.GetComponent<KinematicCharacterMotor>().ForceUnground();
											}
											body.characterMotor.rootMotion += normalized * num2 * Time.fixedDeltaTime;
											body.characterMotor.velocity.y = 0f;
										}
										else
										{
											body.transform.position += normalized * num2 * Time.fixedDeltaTime;
										}
									}
								}
							}
						}
						base.characterMotor.velocity.y = 0f;
						if (this.stopwatch >= this.pullTime + this.skewerExitTime)
						{
							this.outer.SetNextStateToMain();
						}
					}
					else
					{
						if (this.stopwatch >= this.exitTime)
						{
							this.outer.SetNextStateToMain();
						}
					}
				}
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
		
	}
}
