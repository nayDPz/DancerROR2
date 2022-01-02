using System;
using EntityStates;
using EntityStates.LemurianMonster;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates
{
	public class ChargeFireballs : BaseSkillState
	{

		private float duration;

		private Animator animator;

		private ChildLocator childLocator;

		private protected GameObject chargeEffectInstance;

		[SerializeField]
		public GameObject chargeEffectPrefab;

		[SerializeField]
		public string chargeSoundString;

		[SerializeField]
		public float baseDuration = 1f;

		[SerializeField]
		public float minBloomRadius;

		[SerializeField]
		public float maxBloomRadius;

		[SerializeField]
		public GameObject crosshairOverridePrefab;

		[SerializeField]
		public float minChargeDuration = 0.4f;

		private GameObject defaultCrosshairPrefab;

		private uint loopSoundInstanceId;
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.animator = base.GetModelAnimator();
			this.childLocator = base.GetModelChildLocator();
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild("Mouth") ?? base.characterBody.coreTransform;
				if (transform && this.chargeEffectPrefab)
				{
					this.chargeEffectInstance = GameObject.Instantiate<GameObject>(ChargeFireball.chargeVfxPrefab, transform.position, transform.rotation);
					this.chargeEffectInstance.transform.parent = transform;
					ScaleParticleSystemDuration component = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
					ObjectScaleCurve component2 = this.chargeEffectInstance.GetComponent<ObjectScaleCurve>();
					if (component)
					{
						component.newDuration = this.duration;
					}
					if (component2)
					{
						component2.timeMax = this.duration;
					}
				}
			}
			this.PlayChargeAnimation();
			this.loopSoundInstanceId = Util.PlayAttackSpeedSound("FireballCharge", base.gameObject, this.attackSpeedStat);
			this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
			if (this.crosshairOverridePrefab)
			{
				base.characterBody.crosshairPrefab = this.crosshairOverridePrefab;
			}
			base.StartAimMode(this.duration + 2f, false);
		}

		public override void OnExit()
		{
			if (base.characterBody)
			{
				base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			}
			AkSoundEngine.StopPlayingID(this.loopSoundInstanceId);
			if (!this.outer.destroying)
			{
				base.PlayAnimation("Gesture, Additive", "Empty");
			}
			base.PlayAnimation("FullBody, Overide", "BufferEmpty", "Slash.playbackRate", 1f);
			EntityState.Destroy(this.chargeEffectInstance);
			base.OnExit();
		}

		protected float CalcCharge()
		{
			return Mathf.Clamp01(base.fixedAge / this.duration);
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			float charge = this.CalcCharge();
			if (Util.HasEffectiveAuthority(base.gameObject) && ((!base.IsKeyDownAuthority() && base.fixedAge >= this.minChargeDuration) || base.fixedAge >= this.duration))
			{
				FireFireballs fireFireballs = new FireFireballs();
				fireFireballs.charge = charge;
				this.outer.SetNextState(fireFireballs);
			}
		}
		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(Util.Remap(this.CalcCharge(), 0f, 1f, this.minBloomRadius, this.maxBloomRadius), true);
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		protected virtual void PlayChargeAnimation()
		{
			bool isGrounded = base.isGrounded;
			if (isGrounded)
			{
				base.PlayAnimation("FullBody, Override", "NSpecStart", "Slash.playbackRate", 0.225f);
			}
			else
			{
				base.PlayAnimation("FullBody, Override", "NSpecAirStart", "Slash.playbackRate", 0.225f);
			}
		}
		
	}
}
