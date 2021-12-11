using System;
using System.Collections.Generic;
using EntityStates;
using RoR2;
using RoR2.Audio;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x02000009 RID: 9
	public class BaseM1 : BaseSkillState
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00002E38 File Offset: 0x00001038
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.animator.SetBool("attacking", true);
			base.characterDirection.forward = base.inputBank.aimDirection;
			base.characterBody.SetAimTimer(2f);
			this.duration = this.baseDuration / this.attackSpeedStat;
			Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = base.GetModelTransform();
			bool flag = modelTransform;
			if (flag)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
			}
			this.attack = new OverlapAttack();
			this.attack.damageType = this.damageType;
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = base.GetTeam();
			this.attack.damage = this.damageCoefficient * this.damageStat;
			this.attack.procCoefficient = this.procCoefficient;
			this.attack.hitEffectPrefab = this.hitEffectPrefab;
			this.attack.forceVector = this.bonusForce;
			this.attack.pushAwayForce = this.pushForce;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			this.attack.impactSound = this.impactSound;
			bool flag2 = !this.isAerial;
			if (flag2)
			{
				base.inputBank.moveVector = Vector3.zero;
				bool flag3 = this.isDash;
				if (flag3)
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
				else
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
			}
			bool flag4 = this.isDash;
			if (flag4)
			{
				bool isAuthority = base.isAuthority;
				if (isAuthority)
				{
					base.characterMotor.velocity *= 0.2f;
					this.slideVector = base.inputBank.aimDirection;
				}
			}
			this.PlayAttackAnimation();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003062 File Offset: 0x00001262
		private void PlayAttackAnimation()
		{
			base.PlayCrossfade("FullBody, Override", this.animString, "Slash.playbackRate", this.duration, 0.05f);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003088 File Offset: 0x00001288
		private void OnHitEnemyAuthority()
		{
			Util.PlaySound(this.hitSoundString, base.gameObject);
			bool flag = !this.hasHopped;
			if (flag)
			{
				bool flag2 = base.characterMotor && !base.characterMotor.isGrounded && this.hitHopVelocity > 0f;
				if (flag2)
				{
					base.SmallHop(base.characterMotor, this.hitHopVelocity);
				}
				this.hasHopped = true;
			}
			bool flag3 = !this.inHitPause && this.hitStopDuration > 0f;
			if (flag3)
			{
				this.storedVelocity = base.characterMotor.velocity;
				this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
				this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
				this.inHitPause = true;
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003167 File Offset: 0x00001367
		private void PlaySwingEffect()
		{
			EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003184 File Offset: 0x00001384
		private void FireAttack()
		{
			bool flag = !this.hasFired;
			if (flag)
			{
				this.hasFired = true;
				Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
				bool isAuthority = base.isAuthority;
				if (isAuthority)
				{
					this.PlaySwingEffect();
					base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
				}
			}
			bool isAuthority2 = base.isAuthority;
			if (isAuthority2)
			{
				List<HurtBox> list = new List<HurtBox>();
				bool flag2 = this.attack.Fire(list);
				if (flag2)
				{
					bool flag3 = this.isSus || this.isFlinch;
					if (flag3)
					{
						foreach (HurtBox hurtBox in list)
						{
							bool flag4 = this.isSus && hurtBox.healthComponent && hurtBox.healthComponent.body && hurtBox.healthComponent.body.characterMotor;
							if (flag4)
							{
								hurtBox.healthComponent.body.characterMotor.velocity.y = 0f;
							}
							bool flag5 = this.isFlinch && hurtBox.healthComponent && hurtBox.healthComponent.body;
							if (flag5)
							{
								this.ForceFlinch(hurtBox.healthComponent.body);
							}
						}
					}
					this.OnHitEnemyAuthority();
				}
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000334C File Offset: 0x0000154C
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

		// Token: 0x06000024 RID: 36 RVA: 0x00003388 File Offset: 0x00001588
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.hitPauseTimer -= Time.fixedDeltaTime;
			bool flag = this.hitPauseTimer <= 0f && this.inHitPause;
			if (flag)
			{
				base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
				this.inHitPause = false;
				base.characterMotor.velocity = this.storedVelocity;
			}
			bool flag2 = !this.inHitPause;
			if (flag2)
			{
				this.stopwatch += Time.fixedDeltaTime;
			}
			else
			{
				bool flag3 = base.characterMotor;
				if (flag3)
				{
					base.characterMotor.velocity = Vector3.zero;
				}
				bool flag4 = this.animator;
				if (flag4)
				{
					this.animator.SetFloat("Slash.playbackRate", 0f);
				}
			}
			bool flag5 = !this.isAerial;
			if (flag5)
			{
				base.inputBank.moveVector = Vector3.zero;
				bool flag6 = this.isDash;
				if (flag6)
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
				else
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
			}
			bool flag7 = this.isDash;
			if (flag7)
			{
				bool flag8 = base.characterMotor;
				if (flag8)
				{
					float num = this.dashSpeedCurve.Evaluate(base.fixedAge / this.duration);
					float num2 = (!this.hasHopped) ? 1f : 0.4f;
					base.characterMotor.rootMotion += 0.6f * num2 * (this.slideRotation * (num * this.moveSpeedStat * this.slideVector * Time.fixedDeltaTime));
					base.characterMotor.velocity.y = 0f;
				}
			}
			bool flag9 = this.stopwatch >= this.duration * this.attackStartTime && this.stopwatch <= this.duration * this.attackEndTime;
			if (flag9)
			{
				bool flag10 = this.isMultiHit;
				if (flag10)
				{
					this.attackResetStopwatch += Time.fixedDeltaTime;
					bool flag11 = this.attackResetStopwatch >= this.attackResetInterval;
					if (flag11)
					{
						Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
						this.attack.ResetIgnoredHealthComponents();
						this.attackResetStopwatch = 0f;
					}
				}
				this.FireAttack();
			}
			bool flag12 = this.isAerial && base.characterMotor.isGrounded;
			if (flag12)
			{
				base.PlayCrossfade("FullBody, Override", "LandAerial", "Slash.playbackRate", this.duration, 0.05f);
				this.outer.SetNextStateToMain();
			}
			else
			{
				bool flag13 = this.stopwatch >= this.duration - this.earlyExitTime && base.isAuthority && this.isCombo;
				if (flag13)
				{
					bool flag14 = base.inputBank.skill1.down && this.swingIndex != 2;
					if (flag14)
					{
						bool flag15 = !this.hasFired;
						if (flag15)
						{
							this.FireAttack();
						}
						this.SetNextState();
						return;
					}
				}
				bool flag16 = this.stopwatch >= this.duration && base.isAuthority;
				if (flag16)
				{
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003708 File Offset: 0x00001908
		protected virtual void SetNextState()
		{
			int num = this.swingIndex;
			bool flag = num == 0;
			if (flag)
			{
				num = 1;
				this.outer.SetNextState(new Jab2
				{
					swingIndex = num
				});
			}
			else
			{
				bool flag2 = num == 1;
				if (flag2)
				{
					num = 2;
					this.outer.SetNextState(new Jab3
					{
						swingIndex = num
					});
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003768 File Offset: 0x00001968
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000377B File Offset: 0x0000197B
		public override void OnExit()
		{
			this.animator.SetFloat("Slash.playbackRate", 1f);
			base.OnExit();
		}

		// Token: 0x0400005E RID: 94
		protected Vector3 slideVector;

		// Token: 0x0400005F RID: 95
		protected Quaternion slideRotation;

		// Token: 0x04000060 RID: 96
		protected bool isDash = false;

		// Token: 0x04000061 RID: 97
		protected bool isCombo = false;

		// Token: 0x04000062 RID: 98
		protected bool isMultiHit = false;

		// Token: 0x04000063 RID: 99
		protected bool isAerial = false;

		// Token: 0x04000064 RID: 100
		protected bool isSus = false;

		// Token: 0x04000065 RID: 101
		protected bool isFlinch = false;

		// Token: 0x04000066 RID: 102
		protected int stackGainAmount = 8;

		// Token: 0x04000067 RID: 103
		protected float attackResetInterval;

		// Token: 0x04000068 RID: 104
		private float attackResetStopwatch;

		// Token: 0x04000069 RID: 105
		protected AnimationCurve dashSpeedCurve;

		// Token: 0x0400006A RID: 106
		public int swingIndex;

		// Token: 0x0400006B RID: 107
		protected string animString = "Jab1";

		// Token: 0x0400006C RID: 108
		protected string hitboxName = "Jab";

		// Token: 0x0400006D RID: 109
		protected DamageType damageType = DamageType.Generic;

		// Token: 0x0400006E RID: 110
		protected float damageCoefficient = 2.5f;

		// Token: 0x0400006F RID: 111
		protected float procCoefficient = 1f;

		// Token: 0x04000070 RID: 112
		protected float pushForce = 300f;

		// Token: 0x04000071 RID: 113
		protected Vector3 bonusForce = Vector3.zero;

		// Token: 0x04000072 RID: 114
		protected float baseDuration = 0.3f;

		// Token: 0x04000073 RID: 115
		protected float attackStartTime = 0f;

		// Token: 0x04000074 RID: 116
		protected float attackEndTime = 0.4f;

		// Token: 0x04000075 RID: 117
		protected float hitStopDuration = 0.025f;

		// Token: 0x04000076 RID: 118
		protected float attackRecoil = 2f;

		// Token: 0x04000077 RID: 119
		protected float hitHopVelocity = 2f;

		// Token: 0x04000078 RID: 120
		protected bool cancelled = false;

		// Token: 0x04000079 RID: 121
		protected string swingSoundString = "";

		// Token: 0x0400007A RID: 122
		protected string hitSoundString = "";

		// Token: 0x0400007B RID: 123
		protected string muzzleString = "SwingCenter";

		// Token: 0x0400007C RID: 124
		protected GameObject swingEffectPrefab;

		// Token: 0x0400007D RID: 125
		protected GameObject hitEffectPrefab;

		// Token: 0x0400007E RID: 126
		protected NetworkSoundEventIndex impactSound;

		// Token: 0x0400007F RID: 127
		protected float earlyExitTime = 0.075f;

		// Token: 0x04000080 RID: 128
		public float duration;

		// Token: 0x04000081 RID: 129
		private bool hasFired;

		// Token: 0x04000082 RID: 130
		private float hitPauseTimer;

		// Token: 0x04000083 RID: 131
		protected OverlapAttack attack;

		// Token: 0x04000084 RID: 132
		protected bool inHitPause;

		// Token: 0x04000085 RID: 133
		private bool hasHopped;

		// Token: 0x04000086 RID: 134
		protected float stopwatch;

		// Token: 0x04000087 RID: 135
		protected Animator animator;

		// Token: 0x04000088 RID: 136
		private BaseState.HitStopCachedState hitStopCachedState;

		// Token: 0x04000089 RID: 137
		private Vector3 storedVelocity;
	}
}
