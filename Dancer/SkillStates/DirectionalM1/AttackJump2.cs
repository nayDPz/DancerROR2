using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Dancer.Modules;
namespace Dancer.SkillStates
{
	public class AttackJump2 : BaseInputEvaluation
	{
		public Vector3 launchTarget;
		public string critHitSoundString;
		private bool crit;
		private bool secondAttack;
		private List<HealthComponent> hits;

		protected string animString = "UpAir";
		protected string hitboxName = "UpAir";
		protected DamageType damageType = DamageType.Generic;
		protected float damageCoefficient = StaticValues.upAir1DamageCoefficient;
		protected float procCoefficient = 1f;
		protected float pushForce = 2200f;
		protected Vector3 bonusForce = Vector3.zero;
		protected float baseDuration = 0.8f;
		protected float attackStartTime = 0.1f;
		protected float attackEndTime = 0.8f;
		protected float attackResetTime = 0.575f;
		protected float hitStopDuration = 0.06f;
		protected float attackRecoil = 2f;
		protected float hitHopVelocity = 2f;
		protected bool cancelled = false;
		protected string swingSoundString = "";
		protected string hitSoundString = "";
		private float earlyExitTime = .95f;

		protected string muzzleString = "eUpAir1";
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

			base.SmallHop(base.characterMotor, 9f);

			this.animator = base.GetModelAnimator();

			this.AttackSetup();
			this.StartAttack(); //this.StartAttackServer();
		}

		protected float anim = 1f;

		private void AttackSetup()
		{
			this.hits = new List<HealthComponent>();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.crit = base.RollCrit();

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

			this.attack.forceVector = Vector3.zero;
			this.attack.pushAwayForce = 0f;

			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = this.crit;
			this.attack.impactSound = Modules.Assets.sword2HitSoundEvent.index;

			this.swingSoundString = "DancerSwing1";
			this.swingEffectPrefab = Modules.Assets.swingEffect;
			this.hitEffectPrefab = Assets.hitEffect;
		}
		private void StartAttack()
		{
			base.characterBody.SetAimTimer(this.duration);
			//Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
			this.animator.SetBool("attacking", true);
			base.characterDirection.forward = base.inputBank.aimDirection;
			base.PlayAnimation("FullBody, Override", this.animString, "Slash.playbackRate", this.duration * this.anim);
		}

		public virtual void PlayHitSound()
		{
			if (this.secondAttack)
				Util.PlaySound("SwordHit3", base.gameObject);
			else
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
			if (NetworkServer.active)
			{
				if (!a)
				{
					a = true;
					//Debug.Log("test");
				}
				Transform t = base.FindModelChild(this.hitboxName);
				Vector3 position = t.position;
				Vector3 vector = t.localScale * 0.5f;
				Quaternion rot = t.rotation;
				Collider[] hits = Physics.OverlapBox(position, vector, rot, LayerIndex.entityPrecise.mask);
				for (int i = 0; i < hits.Length; i++)
				{
					HurtBox hurtBox = hits[i].GetComponent<HurtBox>();
					if (hurtBox)
					{
						HealthComponent healthComponent = hurtBox.healthComponent;
						if (healthComponent)
						{
							TeamComponent team = healthComponent.GetComponent<TeamComponent>();

							bool enemy = team.teamIndex != base.teamComponent.teamIndex;
							if (enemy)
							{

								if (!this.hits.Contains(healthComponent))
								{
									this.hits.Add(healthComponent);
									HealthComponent h = healthComponent;
									if (h)
									{
										if (!h.body.isChampion || (h.gameObject.name.Contains("Brother") && h.gameObject.name.Contains("Body")))
										{
											this.LaunchEnemy(h.body);
										}
									}
								}
							}
						}
					}

				}

			}

		}

		private bool a = false;
		public void LaunchEnemy(CharacterBody body)
		{
			Vector3 direction = Vector3.up * (this.secondAttack ? 40f : 10f);
			Vector3 launchVector = (direction + base.transform.position) - body.transform.position;
			launchVector = launchVector.normalized;
			launchVector *= this.pushForce;

			if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
			{
				body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}

			CharacterMotor m = body.characterMotor;

			float force = 0.25f;
			if (m)
			{
				m.velocity = Vector3.zero;
				float f = Mathf.Max(150f, m.mass);
				force = f / 150f;
				launchVector *= force;
				m.ApplyForce(launchVector);
			}
			else if (body.rigidbody)
			{
				body.rigidbody.velocity = Vector3.zero;
				float f = Mathf.Max(50f, body.rigidbody.mass);
				force = f / 200f;
				launchVector *= force;
				body.rigidbody.AddForce(launchVector, ForceMode.Impulse);
			}
		}

		protected virtual void ForceFlinch(CharacterBody body)
		{
			SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
			if (component)
			{
				bool canBeHitStunned = component.canBeHitStunned;
				if (canBeHitStunned && Util.HasEffectiveAuthority(body.gameObject))
				{
					component.SetPain();
				}
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			float f = Mathf.Clamp01((base.fixedAge / this.duration * this.attackResetTime)) * 90f;
			//base.characterDirection.forward = Quaternion.Euler(0, f, 0) * base.inputBank.aimDirection;

			this.hitPauseTimer -= Time.fixedDeltaTime;
			if (this.hitPauseTimer <= 0f && this.inHitPause)
			{
				base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
				this.inHitPause = false;
				base.characterMotor.velocity = this.storedVelocity;
			}
			if (!this.inHitPause)
			{
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
			if (this.stopwatch >= this.duration * this.attackStartTime && this.stopwatch <= this.duration * this.attackEndTime)
			{
				if (this.stopwatch >= this.duration * this.attackResetTime && !this.secondAttack)
				{
					this.damageCoefficient = StaticValues.upAir2DamageCoefficient;
					this.hasFired = false;
					this.secondAttack = true;
					this.hitboxName = "UpAir2";
					this.pushForce = 1200f;
					this.hitStopDuration = 0.125f;
					this.muzzleString = "eUpAir2";
					this.swingSoundString = "ForwardAirStart";
					this.swingEffectPrefab = Modules.Assets.dashAttackEffect;

					this.attack = new OverlapAttack();
					this.attack.damageType = DamageType.BonusToLowHealth;
					this.attack.attacker = base.gameObject;
					this.attack.inflictor = base.gameObject;
					this.attack.teamIndex = base.GetTeam();
					this.attack.damage = this.damageCoefficient * this.damageStat;
					this.attack.procCoefficient = this.procCoefficient;
					this.hitEffectPrefab = Assets.stabHitEffect;
					this.attack.impactSound = Modules.Assets.sword3HitSoundEvent.index;
					this.attack.forceVector = Vector3.zero;
					this.attack.pushAwayForce = 0f;

					HitBoxGroup hitBoxGroup = null;
					Transform modelTransform = base.GetModelTransform();
					if (modelTransform)
					{
						hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
					}

					this.attack.hitBoxGroup = hitBoxGroup;
					this.attack.isCrit = this.crit;
					this.attack.impactSound = Modules.Assets.sword3HitSoundEvent.index;
					this.attack.ResetIgnoredHealthComponents();
				}

				this.FireAttack(); //this.FireAttack()
			}
			else
			{
				if (this.stopwatch >= this.duration * this.attackStartTime)
				{
					if (this.stopwatch <= this.duration * this.attackEndTime)
					{
						this.FireAttack();
					}
					this.EvaluateInput();
				}

				if (this.stopwatch >= this.duration * this.earlyExitTime)
				{
					if (this.nextState != null)
						this.SetNextState();
				}

				if (this.stopwatch >= this.duration)
				{
					this.outer.SetNextStateToMain();
				}
			}


		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public override void OnExit()
		{
			if (!this.hasFired)
				this.FireAttack();
			if (this.cancelled)
				PlayAnimation("FullBody, Override", "BufferEmpty");

			base.GetAimAnimator().enabled = true;
			this.animator.SetFloat("Slash.playbackRate", 1f);
			base.OnExit();
		}


	}
}
