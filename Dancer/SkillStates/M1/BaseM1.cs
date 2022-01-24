using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Dancer.SkillStates
{
    public class BaseM1 : BaseSkillState
	{
		public Vector3 launchTarget;
		public bool launchVectorOverride;
		public bool cancelledFromSprinting;
		public bool earlyExitJump;
		public string critHitSoundString;
		private bool jumpCancelled;
		private bool crit;
		protected bool canMove = false;
		private List<HealthComponent> hits;
		protected EntityState nextState;

		public override void OnEnter()
		{
			base.OnEnter();
			
			this.animator = base.GetModelAnimator();

			

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
			if (this.launchVectorOverride)
			{
				this.attack.forceVector = Vector3.zero;
				this.attack.pushAwayForce = 0f;
			}
			else
			{
				this.attack.forceVector = this.bonusForce;
				this.attack.pushAwayForce = this.pushForce;
			}
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
			base.PlayAnimation("FullBody, Override", this.animString, "Slash.playbackRate", this.duration * this.anim);
			if (!this.isAerial)
			{
				//base.inputBank.moveVector = Vector3.zero;
				if (this.isDash)
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
				else
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
			}
			if (this.isDash)
			{
				if (Util.HasEffectiveAuthority(base.gameObject))
				{
					base.characterMotor.velocity *= 0.2f;
					//this.slideVector = base.inputBank.moveVector != Vector3.zero && !base.characterBody.isSprinting ? base.inputBank.moveVector.normalized : base.inputBank.aimDirection;// base.inputBank.aimDirection;
					this.slideVector = base.inputBank.aimDirection;
					this.slideVector.y = 0f;
				}
			}
		}
		public virtual void OnHitEnemyAuthority(List<HurtBox> list)
        {
            PlayHitSound();
            if (!this.hasHopped)
            {
                if (base.characterMotor && !base.characterMotor.isGrounded && this.hitHopVelocity > 0f)
                {
                    base.SmallHop(base.characterMotor, this.hitHopVelocity);
                }
                this.hasHopped = true;
            }
            if (!this.inHitPause && this.hitStopDuration > 0f && !this.jumpCancelled)
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

		public virtual void PlayHitSound()
        {
			if (this.attack.isCrit && this.critHitSoundString != "")
				Util.PlaySound(this.critHitSoundString, base.gameObject);
			else
				Util.PlaySound(this.hitSoundString, base.gameObject);
			
		}
		private void FireAttack()
		{
			if (!this.hasFired)
			{

				this.hasFired = true;
				
				Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);

				if (Util.HasEffectiveAuthority(base.gameObject))
				{
					this.PlaySwingEffect();
					base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
				}
			}
			List<HurtBox> list = new List<HurtBox>();
			if (Util.HasEffectiveAuthority(base.gameObject))
			{				
				if (this.attack.Fire(list))
				{				
					this.OnHitEnemyAuthority(list);
					/*
					foreach (HurtBox hurtBox in list)
					{
						HealthComponent h = hurtBox.healthComponent;
						if (h)
						{
							if (this.isSus && h.body && h.body.characterMotor)
							{
								h.body.characterMotor.velocity.y = 0f;
							}
							if (this.isFlinch && h.body)
							{
								this.ForceFlinch(h.body);
							}
							if (launchVectorOverride && !h.body.isChampion || (h.gameObject.name.Contains("Brother") && h.gameObject.name.Contains("Body")))
							{
								Debug.Log(h.ToString());
								this.LaunchEnemy(hurtBox);
							}
						}
					}		
					*/
				}
			}
			if (NetworkServer.active)
			{ 
				if(!a)
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
					if(hurtBox)
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
										if (this.isSus && h.body && h.body.characterMotor)
										{
											h.body.characterMotor.velocity.y = 0f;
										}
										if (this.isFlinch && h.body)
										{
											this.ForceFlinch(h.body);
										}
										if (launchVectorOverride && !h.body.isChampion || (h.gameObject.name.Contains("Brother") && h.gameObject.name.Contains("Body")))
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
		public virtual void LaunchEnemy(CharacterBody body)
        {

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

		private void SetNextStateFromJump()
        {
			float y = base.inputBank.aimDirection.y;


			if (y > 0.575f)
			{
				this.outer.SetNextState(new UpAir());
			}
			else if (y < -0.74f)
			{
				this.outer.SetNextState(new DownAir());
			}
			else
			{
				this.outer.SetNextState(new FAir());
			}
				
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if(true)//NetworkServer.active)
            {
                if (this.cancelledFromSprinting && (base.characterBody.isSprinting || base.inputBank.sprint.justPressed) && base.inputBank.skill1.down)
                {
                    this.outer.SetNextStateToMain();
                    this.cancelled = true;
                    return;
                }
				if (!base.isGrounded && this.isDash) // buggy
				{
					if(base.inputBank.jump.down)
                    {
						this.jumpCancelled = true;
					}										
				}
				if(this.jumpCancelled && base.fixedAge >= this.duration * (this.attackStartTime + this.attackEndTime) / 2f && !base.isGrounded)
                {
					this.cancelled = true;

					this.SetNextStateFromJump();
					return;
				}

				this.hitPauseTimer -= Time.fixedDeltaTime;
				if (this.hitPauseTimer <= 0f && this.inHitPause)
				{
					base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
					this.inHitPause = false;
					base.characterMotor.velocity = this.storedVelocity;
				}
				if (!this.inHitPause)
				{
					if (this.isMultiHit)
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
				if (!this.isAerial && !this.canMove)
				{
					base.inputBank.moveVector = Vector3.zero;
					base.characterMotor.moveDirection = Vector3.zero;
				}
				if (this.isDash)
				{
					if (base.characterMotor && !this.inHitPause)
					{
						float num = this.dashSpeedCurve.Evaluate(this.stopwatch / this.duration);
						float num2 = (!this.hasHopped) ? 1f : 0.65f;
						base.characterDirection.forward = this.slideVector;
						base.characterMotor.rootMotion += 0.6f * num2 * (this.slideRotation * (num * this.moveSpeedStat * this.slideVector * Time.fixedDeltaTime));
						if(!base.isGrounded && !this.jumpCancelled)
							base.characterMotor.velocity.y = 0f;
					}
					
				}
				if (this.stopwatch >= this.duration * this.attackStartTime && this.stopwatch <= this.duration * this.attackEndTime)
				{
					if (this.isMultiHit)
					{

						if (this.attackResetStopwatch >= this.attackResetInterval)
						{
							Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);
							this.attack.ResetIgnoredHealthComponents();
							this.attackResetStopwatch = 0f;
						}
					}
					this.FireAttack(); //this.FireAttack()
				}

				if (this.stopwatch >= this.duration - this.earlyExitTime && this.isCombo)
				{
					EntityStateMachine e = base.GetComponent<EntityStateMachine>();
					if (e && e.state is GenericCharacterMain)
					{
						if (base.inputBank.skill1.down)
						{
							if (!this.hasFired)
							{
								this.FireAttack(); //this.FireAttack()
							}
							this.SetNextState();
							return;
						}
					}

				}
				if (this.stopwatch >= this.duration)
				{
					this.outer.SetNextStateToMain();
				}
				
			}
			
		}

		protected virtual void SetNextState()
		{
			this.outer.SetNextState(this.nextState);
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
            {				
				PlayAnimation("FullBody, Override", "BufferEmpty");
			}
				

			base.GetAimAnimator().enabled = true;
			this.animator.SetFloat("Slash.playbackRate", 1f);
			base.OnExit();
		}

		protected Vector3 slideVector;
		protected Quaternion slideRotation;

		protected bool isDash = false;
		protected bool isCombo = false;
		protected bool isMultiHit = false;
		protected bool isAerial = false;
		protected bool isSus = false;
		protected bool isFlinch = false;

		protected int stackGainAmount = 8;
		protected float attackResetInterval;
		private float attackResetStopwatch;
		protected AnimationCurve dashSpeedCurve;
		public int swingIndex;
		protected string animString = "Jab1";
		protected string hitboxName = "Jab";
		protected DamageType damageType = DamageType.Generic;
		protected float damageCoefficient = 2.5f;
		protected float procCoefficient = 1f;
		protected float pushForce = 300f;
		protected Vector3 bonusForce = Vector3.zero;
		protected float baseDuration = 0.3f;
		protected float attackStartTime = 0f;
		protected float attackEndTime = 0.4f;
		protected float hitStopDuration = 0.06f;
		protected float attackRecoil = 2f;
		protected float hitHopVelocity = 2f;
		protected bool cancelled = false;
		protected string swingSoundString = "";
		protected string hitSoundString = "";

		protected string muzzleString = "";
		protected GameObject swingEffectPrefab;

		protected GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		protected float earlyExitTime = 0.1f;

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
	}
}
