using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Dancer.SkillStates
{
	public class DownAir : BaseSkillState
	{
		public Vector3 launchTarget;
		public bool launchVectorOverride;
		public bool cancelledFromSprinting;
		public bool earlyExitJump;
		public string critHitSoundString;
		private bool crit;
		private List<HealthComponent> hits;
		protected float attackResetInterval = 0.115f;
		private float attackResetStopwatch;
		public int swingIndex;
		protected string animString = "DownAir";
		protected string hitboxName = "DownAir";
		protected DamageType damageType = DamageType.Generic;
		protected float damageCoefficient = 1.5f;
		protected float procCoefficient = 0.5f;
		protected float pushForce = 500f;
		protected float baseDuration = 1.5f;
		protected float attackStartTime = 0.0f;
		protected float attackEndTime = .9f;
		protected float hitStopDuration = 0.12f;
		protected float attackRecoil = 2f;
		protected float hitHopVelocity = 0f;
		protected bool cancelled = false;
		protected string swingSoundString = "";
		protected string hitSoundString = "";
		private float fallSpeed = 3.75f;

		protected string muzzleString = "SwingCenter";
		protected GameObject swingEffectPrefab;

		protected GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		public float duration;

		private bool hasFired;

		private float hitPauseTimer;

		protected OverlapAttack attack;

		protected bool inHitPause;

		private bool hasHopped;

		protected float stopwatch;

		protected Animator animator;

		private BaseState.HitStopCachedState hitStopCachedState;
		private Vector3 storedVelocity;
		public override void OnEnter()
		{
			base.OnEnter();

			this.animator = base.GetModelAnimator();

			base.characterMotor.velocity.y = 0f;

			this.AttackSetup();
			this.StartAttack(); //this.StartAttackServer();
		}

		protected float anim = 1f;

		private void AttackSetup()
		{
			this.hits = new List<HealthComponent>();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.attackResetInterval /= this.attackSpeedStat;

			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
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
			this.attack.forceVector = Vector3.down * this.pushForce;
			this.attack.pushAwayForce = 0f;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			this.attack.impactSound = this.impactSound;
		}
		private void StartAttack()
		{
			base.characterBody.SetAimTimer(this.duration);
			//Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
			this.animator.SetBool("attacking", true);
			base.characterDirection.forward = base.inputBank.aimDirection;
			base.PlayAnimation("FullBody, Override", "DAir", "Slash.playbackRate", 0.75f);
		}
		private void StartAttackServer()
		{
			if (NetworkServer.active)
			{
				base.characterBody.SetAimTimer(this.duration);
				Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
				this.animator.SetBool("attacking", true);
				base.characterDirection.forward = base.inputBank.aimDirection;

				base.characterMotor.velocity *= 0.2f;
				this.slideVector = Vector3.down;
				
			}

		}

		public virtual void PlayHitSound()
		{
			Util.PlaySound("SwordHit2", base.gameObject);
		}

		public virtual void OnHitEnemyAuthority(List<HurtBox> list)
		{
			this.PlayHitSound();
			if (!this.hasHopped)
			{
				if (base.characterMotor && !base.characterMotor.isGrounded && this.hitHopVelocity > 0f)
				{
					base.SmallHop(base.characterMotor, this.hitHopVelocity);
				}
				this.hasHopped = true;
			}
			if (!this.inHitPause && this.hitStopDuration > 0f)
			{
				this.storedVelocity = base.characterMotor.velocity;
				this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
				this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
				this.inHitPause = true;
			}
		}

		private void PlaySwingEffect()
		{
			EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
		}

		
		private void FireAttack()
		{
			if (!this.hasFired)
			{
				this.hasFired = true;

				if (Util.HasEffectiveAuthority(base.gameObject))
				{
					this.PlaySwingEffect();
					Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
					base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
				}
			}
			List<HurtBox> list = new List<HurtBox>();
			if (Util.HasEffectiveAuthority(base.gameObject))
			{
				if (this.attack.Fire(list))
				{
					this.OnHitEnemyAuthority(list);
				}
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (true)//NetworkServer.active)
			{
				this.hitPauseTimer -= Time.fixedDeltaTime;
				if (this.hitPauseTimer <= 0f && this.inHitPause)
				{
					this.animator.SetFloat("Slash.playbackRate", 2f);
					base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
					this.inHitPause = false;
					base.characterMotor.velocity = this.storedVelocity;
				}
				if (!this.inHitPause)
				{
					this.attackResetStopwatch += Time.fixedDeltaTime;
					this.stopwatch += Time.fixedDeltaTime;
				}
				else
				{
					if (base.characterMotor)
					{
						base.characterMotor.velocity = Vector3.zero;
					}
					if (this.animator)
					{
						this.animator.SetFloat("Slash.playbackRate", 0f);
					}
				}

				base.inputBank.moveVector /= 2f;


				if (base.characterMotor && !this.inHitPause)
				{
					base.characterMotor.rootMotion += (this.fallSpeed * this.moveSpeedStat * Vector3.down * Time.fixedDeltaTime);
				}

				if (this.stopwatch >= this.duration * this.attackStartTime && this.stopwatch <= this.duration * this.attackEndTime)
				{
					if (this.attackResetStopwatch >= this.attackResetInterval)
					{
						//Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
						this.attack.ResetIgnoredHealthComponents();
						this.attackResetStopwatch = 0f;
					}
					this.FireAttack(); //this.FireAttack()
				}
				if (base.characterMotor.isGrounded)
				{					
					this.outer.SetNextState(new DownAirLand());
				}
				else
				{
					if (this.stopwatch >= this.duration)
					{
						base.PlayCrossfade("FullBody, Override", "BufferEmpty", "Slash.playbackRate", this.duration * this.anim, 0.05f);
						this.outer.SetNextStateToMain();
					}
				}
			}

		}

		protected virtual void SetNextState()
		{
			int num = this.swingIndex;
			if (num == 0)
			{
				num = 1;
				this.outer.SetNextState(new Jab2
				{
					swingIndex = num
				});
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public override void OnExit()
		{
			if (this.cancelled)
				if (this.cancelled)
					PlayAnimation("FullBody, Override", "BufferEmpty");

			base.GetAimAnimator().enabled = true;
			this.animator.SetFloat("Slash.playbackRate", 1f);
			base.OnExit();
		}

		protected Vector3 slideVector;
		protected Quaternion slideRotation;

		
	}
}
