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
	public class DownAirLand : BaseSkillState
	{
		public Vector3 launchTarget;
		private bool crit;
		private List<HealthComponent> hits;
		private float attackResetStopwatch;
		public int swingIndex;
		protected string animString = "DownAirGround";
		protected string hitboxName = "DownAirGround";
		protected DamageType damageType = DamageType.Generic;
		protected float damageCoefficient = StaticValues.downTiltDamageCoefficient;
		protected float procCoefficient = 1f;
		protected float pushForce = 1900f;
		protected float baseDuration = 0.55f;
		protected float attackStartTime = 0.0f;
		protected float attackEndTime = 1f;
		protected float hitStopDuration = 0.09f;
		protected float attackRecoil = 2f;
		protected float hitHopVelocity = 0f;
		protected bool cancelled = false;
		protected string swingSoundString = "";
		protected string hitSoundString = "";

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
			this.impactSound = Modules.Assets.sword2HitSoundEvent.index;
			

			this.AttackSetup();
			this.StartAttack(); //this.StartAttackServer();
		}

		protected float anim = 1f;

		private void AttackSetup()
		{
			this.hits = new List<HealthComponent>();
			this.duration = this.baseDuration / this.attackSpeedStat;
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
			this.attack.hitEffectPrefab = Assets.hitEffect;
			this.attack.forceVector = Vector3.zero;
			this.attack.pushAwayForce = 0f;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			this.attack.impactSound = this.impactSound;


			this.swingEffectPrefab = Modules.Assets.downAirEndEffect;
			this.muzzleString = "eDAirEnd";
			this.swingSoundString = "PunchSwing";
		}
		private void StartAttack()
		{
			base.characterBody.SetAimTimer(this.duration);
			Util.PlayAttackSpeedSound("DancerDownTilt", base.gameObject, this.attackSpeedStat);
			this.animator.SetBool("attacking", true);
			base.characterDirection.forward = base.inputBank.aimDirection;
			base.PlayCrossfade("FullBody, Override", "DAirGround", "Slash.playbackRate", this.duration * 1.2f, 0.01f);
		}
		public virtual void OnHitEnemyAuthority(List<HurtBox> list)
		{
			Util.PlaySound("SwordHit2", base.gameObject);
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
				Transform t = base.FindModelChild("DAirGround");
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
										if (h.body && h.body.characterMotor)
										{
											h.body.characterMotor.velocity = Vector3.zero;
										}
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

		public void LaunchEnemy(CharacterBody body)
		{

			Vector3 direction = Vector3.up * 15f;
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
				float f = Mathf.Max(100f, m.mass);
				force = f / 100f;
				launchVector *= force;
				m.ApplyForce(launchVector);
			}
			else if (body.rigidbody)
			{
				float f = Mathf.Max(50f, body.rigidbody.mass);
				force = f / 200f;
				launchVector *= force;
				body.rigidbody.AddForce(launchVector, ForceMode.Impulse);
			}

		}

		private void SetNextStateFromJump()
		{
			float y = base.inputBank.aimDirection.y;


			if (y > 0.575f)
			{
				this.outer.SetNextState(new UpAir());
			}
			else
			{
				this.outer.SetNextState(new FAir());
			}
					

		}

		private bool jumpCancelled;

		public override void FixedUpdate()
		{
			base.FixedUpdate();


			if (!base.isGrounded) // buggy
			{
				if (base.inputBank.jump.down)
				{
					this.jumpCancelled = true;
				}
			}
			if (this.jumpCancelled && base.fixedAge >= this.duration * (this.attackStartTime + this.attackEndTime) * 0.75f && !base.isGrounded)
			{
				this.cancelled = true;

				this.SetNextStateFromJump();
				return;
			}

			if (false)//!base.isGrounded && base.inputBank.jump.down)
			{
				if(this.inHitPause)
                {
					base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
					base.characterMotor.velocity = this.storedVelocity;
				}
					
				//this.SetNextStateFromJump();
				this.outer.SetNextStateToMain();
				this.cancelled = true;
				return;
			}

			base.inputBank.moveVector = Vector3.zero;
			this.hitPauseTimer -= Time.fixedDeltaTime;
			if (this.hitPauseTimer <= 0f && this.inHitPause)
			{
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

			if (this.stopwatch >= this.duration * this.attackStartTime && this.stopwatch <= this.duration * this.attackEndTime)
			{
				this.FireAttack(); //this.FireAttack()
			}
			else
			{
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
			if (this.cancelled)
				PlayAnimation("FullBody, Override", "BufferEmpty");

			base.GetAimAnimator().enabled = true;
			this.animator.SetFloat("Slash.playbackRate", 1f);
			base.OnExit();
		}


	}
}
