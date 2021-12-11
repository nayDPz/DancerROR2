using System;
using EntityStates;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace Ridley.SkillStates.BaseStates
{
	// Token: 0x02000018 RID: 24
	public class BaseMeleeAttack : BaseSkillState
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00005EA8 File Offset: 0x000040A8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.animDuration = this.duration;
			this.earlyExitTime = this.baseEarlyExitTime / this.attackSpeedStat;
			this.hasFired = false;
			this.animator = base.GetModelAnimator();
			base.StartAimMode(0.5f + this.duration, false);
			base.characterBody.outOfCombatStopwatch = 0f;
			this.animator.SetBool("attacking", true);
			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = base.GetModelTransform();
			bool flag = modelTransform;
			if (flag)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
			}
			this.PlayAttackAnimation();
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
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006048 File Offset: 0x00004248
		protected virtual void PlayAttackAnimation()
		{
			base.PlayCrossfade("Gesture, Override", "Slash" + (1 + this.swingIndex).ToString(), "Slash.playbackRate", this.animDuration, 0.05f);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000608C File Offset: 0x0000428C
		public override void OnExit()
		{
			bool flag = !this.hasFired && !this.cancelled;
			if (flag)
			{
				this.FireAttack();
			}
			base.OnExit();
			this.animator.SetBool("attacking", false);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000060D2 File Offset: 0x000042D2
		protected virtual void PlaySwingEffect()
		{
			EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000060F0 File Offset: 0x000042F0
		protected virtual void OnHitEnemyAuthority()
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

		// Token: 0x0600005C RID: 92 RVA: 0x000061D0 File Offset: 0x000043D0
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
				bool flag2 = this.attack.Fire(null);
				if (flag2)
				{
					this.OnHitEnemyAuthority();
				}
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000627C File Offset: 0x0000447C
		protected virtual void SetNextState()
		{
			int num = this.swingIndex;
			bool flag = num == 0;
			if (flag)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			this.outer.SetNextState(new BaseMeleeAttack
			{
				swingIndex = num
			});
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000062B8 File Offset: 0x000044B8
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
					this.animator.SetFloat("Swing.playbackRate", 0f);
				}
			}
			bool flag5 = this.stopwatch >= this.duration * this.attackStartTime && this.stopwatch <= this.duration * this.attackEndTime;
			if (flag5)
			{
				this.FireAttack();
			}
			bool flag6 = this.stopwatch >= this.duration - this.earlyExitTime && base.isAuthority;
			if (flag6)
			{
				bool down = base.inputBank.skill1.down;
				if (down)
				{
					bool flag7 = !this.hasFired;
					if (flag7)
					{
						this.FireAttack();
					}
					this.SetNextState();
					return;
				}
			}
			bool flag8 = this.stopwatch >= this.duration && base.isAuthority;
			if (flag8)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000645C File Offset: 0x0000465C
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000646F File Offset: 0x0000466F
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.swingIndex);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00006487 File Offset: 0x00004687
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.swingIndex = reader.ReadInt32();
		}

		// Token: 0x040000D1 RID: 209
		public int swingIndex;

		// Token: 0x040000D2 RID: 210
		protected string hitboxName = "Sword";

		// Token: 0x040000D3 RID: 211
		protected DamageType damageType = DamageType.Generic;

		// Token: 0x040000D4 RID: 212
		protected float damageCoefficient = 3.5f;

		// Token: 0x040000D5 RID: 213
		protected float procCoefficient = 1f;

		// Token: 0x040000D6 RID: 214
		protected float pushForce = 300f;

		// Token: 0x040000D7 RID: 215
		protected Vector3 bonusForce = Vector3.zero;

		// Token: 0x040000D8 RID: 216
		protected float baseDuration = 1f;

		// Token: 0x040000D9 RID: 217
		protected float attackStartTime = 0.2f;

		// Token: 0x040000DA RID: 218
		protected float attackEndTime = 0.4f;

		// Token: 0x040000DB RID: 219
		protected float baseEarlyExitTime = 0.4f;

		// Token: 0x040000DC RID: 220
		protected float hitStopDuration = 0.012f;

		// Token: 0x040000DD RID: 221
		protected float attackRecoil = 0.75f;

		// Token: 0x040000DE RID: 222
		protected float hitHopVelocity = 4f;

		// Token: 0x040000DF RID: 223
		protected bool cancelled = false;

		// Token: 0x040000E0 RID: 224
		protected float animDuration;

		// Token: 0x040000E1 RID: 225
		protected string swingSoundString = "";

		// Token: 0x040000E2 RID: 226
		protected string hitSoundString = "";

		// Token: 0x040000E3 RID: 227
		protected string muzzleString = "SwingCenter";

		// Token: 0x040000E4 RID: 228
		protected GameObject swingEffectPrefab;

		// Token: 0x040000E5 RID: 229
		protected GameObject hitEffectPrefab;

		// Token: 0x040000E6 RID: 230
		protected NetworkSoundEventIndex impactSound;

		// Token: 0x040000E7 RID: 231
		private float earlyExitTime;

		// Token: 0x040000E8 RID: 232
		public float duration;

		// Token: 0x040000E9 RID: 233
		private bool hasFired;

		// Token: 0x040000EA RID: 234
		private float hitPauseTimer;

		// Token: 0x040000EB RID: 235
		private OverlapAttack attack;

		// Token: 0x040000EC RID: 236
		protected bool inHitPause;

		// Token: 0x040000ED RID: 237
		private bool hasHopped;

		// Token: 0x040000EE RID: 238
		protected float stopwatch;

		// Token: 0x040000EF RID: 239
		protected Animator animator;

		// Token: 0x040000F0 RID: 240
		private BaseState.HitStopCachedState hitStopCachedState;

		// Token: 0x040000F1 RID: 241
		private Vector3 storedVelocity;
	}
}
