using System;
using System.Collections.Generic;
using EntityStates;
using KinematicCharacterController;
using Ridley.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x02000015 RID: 21
	public class Skewer : BaseSkillState
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000045F4 File Offset: 0x000027F4
		private float attackRecoil = 7f;
		public override void OnEnter()
		{
			base.OnEnter();
			this.skewerTime /= this.attackSpeedStat;
			this.duration = Skewer.baseDuration / this.attackSpeedStat;
			this.fireTime = 0.4f * this.duration;
			base.characterBody.SetAimTimer(2f);
			base.characterDirection.forward = base.GetAimRay().direction;
			this.hitHurtBoxes = new List<HurtBox>();
			this.animator = base.GetModelAnimator();
			this.muzzleString = "Muzzle";
			base.PlayAnimation("FullBody, Override", "DownSpecial", "Slash.playbackrate", this.duration);
			Util.PlaySound("DSpecialStart", base.gameObject);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000046AD File Offset: 0x000028AD
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000046B8 File Offset: 0x000028B8
		private void Fire()
		{
			bool flag = !this.hasFired;
			if (flag)
			{
				this.hasFired = true;
				base.characterBody.AddSpreadBloom(1.5f);
				EffectManager.SimpleMuzzleFlash(Skewer.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
				bool isAuthority = base.isAuthority;
				if (isAuthority)
				{
					Ray aimRay = base.GetAimRay();
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
							bool flag2 = hitInfo.hitHurtBox.healthComponent && hitInfo.hitHurtBox.healthComponent.body;
							if (flag2)
							{
								EntityStateMachine component = hitInfo.hitHurtBox.healthComponent.body.GetComponent<EntityStateMachine>();
								bool flag3 = hitInfo.hitHurtBox.healthComponent.body.GetComponent<SetStateOnHurt>().canBeFrozen && component;
								if (flag3)
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

		// Token: 0x06000044 RID: 68 RVA: 0x000048F3 File Offset: 0x00002AF3
		private void OnHitEnemyAuthority()
		{
			base.characterMotor.velocity = Vector3.zero;
			base.AddRecoil(-1f * this.attackRecoil / 2, -2f * this.attackRecoil / 2, -0.5f * this.attackRecoil / 2, 0.5f * this.attackRecoil / 2);
			this.hasHit = true;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004910 File Offset: 0x00002B10
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			bool flag = this.subState == Skewer.SubState.Skewer;
			if (flag)
			{
				bool flag2 = this.stopwatch >= this.fireTime;
				if (flag2)
				{
					this.Fire();
				}
				bool flag3 = this.hasFired;
				if (flag3)
				{
					bool flag4 = this.hitHurtBoxes.Count > 0;
					if (flag4)
					{
						base.PlayAnimation("FullBody, Override", "DownSpecialHit", "Slash.playbackrate", this.duration * 0.5f);
						Util.PlaySound("DSpecialHit", base.gameObject);
						this.subState = Skewer.SubState.SkewerHit;
						this.stopwatch = 0f;
					}
					else
					{
						bool flag5 = !this.a;
						if (flag5)
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
				bool flag6 = this.subState == Skewer.SubState.SkewerHit;
				if (flag6)
				{
					base.GetModelAnimator().SetFloat("Slash.playbackRate", 0f);
					base.characterMotor.velocity.y = 0f;
					foreach (HurtBox hurtBox in this.hitHurtBoxes)
					{
						bool flag7 = hurtBox;
						if (flag7)
						{
							HealthComponent healthComponent = hurtBox.healthComponent;
							bool flag8 = healthComponent && healthComponent.body;
							if (flag8)
							{
								CharacterBody body = healthComponent.body;
								bool flag9 = body.characterMotor;
								if (flag9)
								{
									body.characterMotor.velocity = Vector3.zero;
								}
							}
						}
					}
					bool flag10 = this.stopwatch >= this.skewerTime;
					if (flag10)
					{
						/*
						bool crit = base.RollCrit();
						foreach(HurtBox hurtBox in this.hitHurtBoxes)
                        {
							if(hurtBox.healthComponent && hurtBox.healthComponent.body && hurtBox.healthComponent.alive)
                            {
								DamageInfo damageInfo = new DamageInfo
								{
									position = hurtBox.healthComponent.body.transform.position,
									attacker = base.gameObject,
									inflictor = base.gameObject,
									damage = Skewer.damageCoefficient * this.damageStat,
									damageColorIndex = DamageColorIndex.Default,
									damageType = DamageType.Stun1s,
									crit = crit,
									force = Vector3.zero,
									procChainMask = default(ProcChainMask),
									procCoefficient = 1f
								};
								hurtBox.healthComponent.TakeDamage(damageInfo);
							}
							
						}							
						*/
						base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
						base.GetModelAnimator().SetFloat("Slash.playbackRate", 1f);
						Util.PlaySound("DSpecialPull", base.gameObject);
						this.stopwatch = 0f;
						this.subState = Skewer.SubState.Pull;
					}
				}
				else
				{
					bool flag11 = this.subState == Skewer.SubState.Pull;
					if (flag11)
					{
						foreach (HurtBox hurtBox2 in this.hitHurtBoxes)
						{
							bool flag12 = hurtBox2;
							if (flag12)
							{
								HealthComponent healthComponent2 = hurtBox2.healthComponent;
								bool flag13 = healthComponent2 && healthComponent2.body;
								if (flag13)
								{
									bool flag14 = this.stopwatch < this.pullTime;
									if (flag14)
									{
										CharacterBody body2 = healthComponent2.body;
										float num = this.pullTime - this.stopwatch;
										Vector3 vector = this.pullPoint - body2.coreTransform.position;
										float num2 = vector.magnitude / num;
										Vector3 normalized = vector.normalized;
										float num3 = Mathf.Lerp(2f, 0f, this.stopwatch / this.pullTime);
										bool isChampion = body2.isChampion;
										if (isChampion)
										{
											num3 /= 2f;
										}
										num2 *= num3;
										bool flag15 = body2.characterMotor;
										if (flag15)
										{
											bool flag16 = body2.gameObject.GetComponent<KinematicCharacterMotor>();
											if (flag16)
											{
												body2.gameObject.GetComponent<KinematicCharacterMotor>().ForceUnground();
											}
											body2.characterMotor.rootMotion += normalized * num2 * Time.fixedDeltaTime;
											body2.characterMotor.velocity.y = 0f;
										}
										else
										{
											body2.transform.position += normalized * num2 * Time.fixedDeltaTime;
										}
									}
								}
							}
						}
						base.characterMotor.velocity.y = 0f;
						bool flag17 = this.stopwatch >= this.pullTime + this.skewerExitTime;
						if (flag17)
						{
							this.outer.SetNextStateToMain();
						}
					}
					else
					{
						bool flag18 = this.stopwatch >= this.exitTime;
						if (flag18)
						{
							this.outer.SetNextStateToMain();
						}
					}
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00004D94 File Offset: 0x00002F94
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400008C RID: 140
		public static float damageCoefficient = 5f;

		// Token: 0x0400008D RID: 141
		public static float procCoefficient = 1f;

		// Token: 0x0400008E RID: 142
		public static float baseDuration = 1.5f;

		// Token: 0x0400008F RID: 143
		public static float force = 0f;

		// Token: 0x04000090 RID: 144
		public static float recoil = 1f;

		// Token: 0x04000091 RID: 145
		public static float range = 43f;

		// Token: 0x04000092 RID: 146
		public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerHuntressSnipe");

		// Token: 0x04000093 RID: 147
		public static GameObject muzzleEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashHuntress");

		// Token: 0x04000094 RID: 148
		public static GameObject hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/HitEffect/HitsparkCaptainShotgun");

		// Token: 0x04000095 RID: 149
		private float exitTime = 0.25f;
		private float skewerExitTime = 0.3f;
		
		// Token: 0x04000096 RID: 150
		private float pullTime = 0.25f;

		// Token: 0x04000097 RID: 151
		private float skewerTime = 0.5f;

		// Token: 0x04000098 RID: 152
		private float stopwatch;

		// Token: 0x04000099 RID: 153
		private Vector3 pullPoint;

		// Token: 0x0400009A RID: 154
		private float duration;

		// Token: 0x0400009B RID: 155
		private float fireTime;

		// Token: 0x0400009C RID: 156
		private bool hasFired;

		// Token: 0x0400009D RID: 157
		private bool hasHit;

		// Token: 0x0400009E RID: 158
		private float hitTime;

		// Token: 0x0400009F RID: 159
		private Animator animator;

		// Token: 0x040000A0 RID: 160
		private string muzzleString;

		// Token: 0x040000A1 RID: 161
		private static float antigravityStrength;

		// Token: 0x040000A2 RID: 162
		private List<HurtBox> hitHurtBoxes;

		// Token: 0x040000A3 RID: 163
		private Skewer.SubState subState;

		// Token: 0x040000A4 RID: 164
		private bool a;

		// Token: 0x0200002B RID: 43
		private enum SubState
		{
			// Token: 0x04000158 RID: 344
			Skewer,
			// Token: 0x04000159 RID: 345
			SkewerHit,
			// Token: 0x0400015A RID: 346
			Pull,
			// Token: 0x0400015B RID: 347
			Exit
		}
	}
}
