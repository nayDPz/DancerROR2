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
	public class SpinDash : BaseSkillState
	{
		public Vector3 launchTarget;
		public bool launchVectorOverride;
		public bool cancelledFromSprinting;
		public bool earlyExitJump;
		public string critHitSoundString;
		private List<HealthComponent> hits;
		private int numResets = 2;
		private int timesReset = 0;
		protected float attackResetInterval;
		private float attackResetStopwatch;
		public int swingIndex;
		protected string hitboxName = "SpinLunge";
		protected DamageType damageType = DamageType.Generic;
		protected float damageCoefficient = StaticValues.spinDashDamageCoefficient;
		protected float procCoefficient = 0.75f;
		protected float pushForce = 3000f;
		protected float baseDuration = 0.55f;
		protected float attackStartTime = 0.15f;
		protected float attackEndTime = .9f;
		protected float hitStopDuration = 0.05f;
		protected float attackRecoil = 2f;
		protected float hitHopVelocity = 0f;
		protected bool cancelled = false;
		protected string swingSoundString = "";
		protected string hitSoundString = "";
		private float speedCoefficient = 35f;
		private bool hitGround;

		protected string muzzleString = "eDashAttack";
		private Transform muzzleTransform;

		protected GameObject swingEffectPrefab;
		private GameObject swingEffect;

		protected GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		public float duration;

		private bool hasFired;

		private float hitPauseTimer;

		protected OverlapAttack attack;

		protected bool inHitPause;

		protected float stopwatch;

		protected Animator animator;

		private Vector3 moveVector;

		private BaseState.HitStopCachedState hitStopCachedState;
		private Vector3 storedVelocity;

		private DancerComponent dancerComponent;
		private bool reset = true;

		public override void OnEnter()
		{
			base.OnEnter();

			this.animator = base.GetModelAnimator();

			if (base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.muzzleTransform = component.FindChild("eDashAttack");
				}
			}
			this.dancerComponent = base.characterBody.GetComponent<DancerComponent>();

			this.moveVector = base.inputBank.aimDirection;
			this.moveVector.y = Mathf.Clamp(this.moveVector.y, -0.2f, 0.2f);

			//if (this.dancerComponent)
			//	this.dancerComponent.BodyRotationOverride(this.moveVector);

			base.characterMotor.velocity.y = 0f;

			this.AttackSetup();
			this.StartAttack(); //this.StartAttackServer();
		}

		protected float anim = 1f;

		private void AttackSetup()
		{
			this.hits = new List<HealthComponent>();
			this.duration = this.baseDuration / this.attackSpeedStat;
			float d = (this.duration * this.attackEndTime) - (this.duration * this.attackStartTime);

			this.attackResetInterval = d / this.numResets;

			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
			}
			this.damageCoefficient = StaticValues.spinDashDamageCoefficient / this.numResets;
			this.attack = new OverlapAttack();
			this.attack.damageType = this.damageType;
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = base.GetTeam();
			this.attack.damage = this.damageCoefficient * this.damageStat;
			this.attack.procCoefficient = this.procCoefficient;
			this.attack.hitEffectPrefab = Assets.stabHitEffect;
			this.attack.forceVector = Vector3.zero;
			this.attack.pushAwayForce = 0f;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			this.attack.impactSound = Modules.Assets.sword1HitSoundEvent.index;

			this.swingEffectPrefab = Modules.Assets.downAirEffect;
			this.muzzleString = "eDashAttack";
		}
		private void StartAttack()
		{
			base.characterBody.SetAimTimer(this.duration);
			Util.PlayAttackSpeedSound("DancerSwing1", base.gameObject, this.attackSpeedStat);
			this.animator.SetBool("attacking", true);
			base.characterDirection.forward = base.inputBank.aimDirection;
			base.PlayAnimation("FullBody, Override", "Drill", "DragonLunge.playbackRate", 1.1f);
		}

		public virtual void PlayHitSound()
		{
			Util.PlaySound("SwordHit", base.gameObject);
		}

		public virtual void OnHitEnemyAuthority(List<HurtBox> list)
		{
			this.PlayHitSound();
			if (!this.inHitPause && this.hitStopDuration > 0f)
			{
				this.storedVelocity = base.characterMotor.velocity;
				this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "DragonLunge.playbackRate");
				this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
				this.inHitPause = true;
			}
		}

		private void PlaySwingEffect()
		{
			this.swingEffect = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, this.muzzleTransform);
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
			if(this.reset)
            {
				this.reset = false;
				if (NetworkServer.active)
				{
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
			
		}


		private void LaunchEnemy(CharacterBody body)
		{
			Vector3 direction = this.moveVector * 50f;
			Vector3 launchVector = (direction + base.transform.position) - body.transform.position;
			launchVector = launchVector.normalized;
			launchVector *= this.pushForce;

			if (this.timesReset == this.numResets)
				launchVector /= 4;

			if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
			{
				body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
			}

			CharacterMotor m = body.characterMotor;

			float force = 0.25f;
			if (m)
			{
				m.velocity = Vector3.zero;
				float f = Mathf.Max(100f, m.mass);
				force = f / 100f;
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

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			base.inputBank.moveVector = Vector3.zero;
			base.characterMotor.moveDirection = Vector3.zero;

			this.hitPauseTimer -= Time.fixedDeltaTime;
			if (this.hitPauseTimer <= 0f && this.inHitPause)
			{
				this.animator.SetFloat("DragonLunge.playbackRate", 1f);
				base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
				this.inHitPause = false;
				base.characterMotor.velocity = this.storedVelocity;
			}
			if (!this.inHitPause)
			{
				
				this.stopwatch += Time.fixedDeltaTime;
				if (this.stopwatch >= this.duration * this.attackStartTime)
					this.attackResetStopwatch += Time.fixedDeltaTime;
			}
			else
			{
				if (base.characterMotor)
				{
					base.characterMotor.velocity = Vector3.zero;
				}
				if (this.animator)
				{
					this.animator.SetFloat("DragonLunge.playbackRate", 0f);
				}
			}


			if (base.characterMotor && !this.inHitPause)
			{
				float f = Mathf.Lerp(this.speedCoefficient * 1.25f, this.speedCoefficient * 0.75f, this.stopwatch / this.duration);
				base.characterMotor.rootMotion += (f * this.moveVector * Time.fixedDeltaTime);
				base.characterMotor.velocity.y = 0;
				base.characterDirection.forward = this.moveVector;
			}

			if (this.stopwatch >= this.duration * this.attackStartTime && this.stopwatch <= this.duration * this.attackEndTime)
			{
				if (this.attackResetStopwatch >= this.attackResetInterval)
				{
					this.reset = true;
					this.timesReset++;
					this.attack.ResetIgnoredHealthComponents();
					this.attackResetStopwatch = 0f;
				}
				if(this.timesReset <= this.numResets)
					this.FireAttack();
			}
			if (this.stopwatch >= this.duration - (this.attackResetInterval / 2))
			{
				base.SmallHop(base.characterMotor, base.characterBody.jumpPower);
				this.outer.SetNextState(new SpinDashEnd());
			}
			
			

		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public override void OnExit()
		{
			if (this.dancerComponent)
				this.dancerComponent.StopBodyOverride();

			if (this.swingEffect) GameObject.Destroy(this.swingEffect);
			base.GetAimAnimator().enabled = true;
			this.animator.SetFloat("DragonLunge.playbackRate", 1f);
			base.OnExit();
		}

		protected Vector3 slideVector;
		protected Quaternion slideRotation;


	}
}
