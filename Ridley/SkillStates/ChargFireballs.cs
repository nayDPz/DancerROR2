using System;
using EntityStates;
using EntityStates.LemurianMonster;
using RoR2;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x02000007 RID: 7
	public class ChargeFireballs : BaseSkillState
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002898 File Offset: 0x00000A98
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.animator = base.GetModelAnimator();
			this.childLocator = base.GetModelChildLocator();
			bool flag = this.childLocator;
			if (flag)
			{
				Transform transform = this.childLocator.FindChild("Mouth") ?? base.characterBody.coreTransform;
				bool flag2 = transform && this.chargeEffectPrefab;
				if (flag2)
				{
					this.chargeEffectInstance = GameObject.Instantiate<GameObject>(ChargeFireball.chargeVfxPrefab, transform.position, transform.rotation);
					this.chargeEffectInstance.transform.parent = transform;
					ScaleParticleSystemDuration component = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
					ObjectScaleCurve component2 = this.chargeEffectInstance.GetComponent<ObjectScaleCurve>();
					bool flag3 = component;
					if (flag3)
					{
						component.newDuration = this.duration;
					}
					bool flag4 = component2;
					if (flag4)
					{
						component2.timeMax = this.duration;
					}
				}
			}
			this.PlayChargeAnimation();
			this.loopSoundInstanceId = Util.PlayAttackSpeedSound("FireballCharge", base.gameObject, this.attackSpeedStat);
			this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
			bool flag5 = this.crosshairOverridePrefab;
			if (flag5)
			{
				base.characterBody.crosshairPrefab = this.crosshairOverridePrefab;
			}
			base.StartAimMode(this.duration + 2f, false);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002A18 File Offset: 0x00000C18
		public override void OnExit()
		{
			bool flag = base.characterBody;
			if (flag)
			{
				base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			}
			AkSoundEngine.StopPlayingID(this.loopSoundInstanceId);
			bool flag2 = !this.outer.destroying;
			if (flag2)
			{
				base.PlayAnimation("Gesture, Additive", "Empty");
			}
			base.PlayAnimation("FullBody, Overide", "BufferEmpty", "Slash.playbackRate", 1f);
			EntityState.Destroy(this.chargeEffectInstance);
			base.OnExit();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002AA8 File Offset: 0x00000CA8
		protected float CalcCharge()
		{
			return Mathf.Clamp01(base.fixedAge / this.duration);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002ACC File Offset: 0x00000CCC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			float charge = this.CalcCharge();
			bool flag = base.isAuthority && ((!base.IsKeyDownAuthority() && base.fixedAge >= this.minChargeDuration) || base.fixedAge >= this.duration);
			if (flag)
			{
				FireFireballs fireFireballs = new FireFireballs();
				fireFireballs.charge = charge;
				this.outer.SetNextState(fireFireballs);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002B3D File Offset: 0x00000D3D
		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(Util.Remap(this.CalcCharge(), 0f, 1f, this.minBloomRadius, this.maxBloomRadius), true);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002B78 File Offset: 0x00000D78
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002B8C File Offset: 0x00000D8C
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

		// Token: 0x04000046 RID: 70
		private float duration;

		// Token: 0x04000047 RID: 71
		private Animator animator;

		// Token: 0x04000048 RID: 72
		private ChildLocator childLocator;

		// Token: 0x04000049 RID: 73
		private protected GameObject chargeEffectInstance;

		// Token: 0x0400004A RID: 74
		[SerializeField]
		public GameObject chargeEffectPrefab;

		// Token: 0x0400004B RID: 75
		[SerializeField]
		public string chargeSoundString;

		// Token: 0x0400004C RID: 76
		[SerializeField]
		public float baseDuration = 1f;

		// Token: 0x0400004D RID: 77
		[SerializeField]
		public float minBloomRadius;

		// Token: 0x0400004E RID: 78
		[SerializeField]
		public float maxBloomRadius;

		// Token: 0x0400004F RID: 79
		[SerializeField]
		public GameObject crosshairOverridePrefab;

		// Token: 0x04000050 RID: 80
		[SerializeField]
		public float minChargeDuration = 0.4f;

		// Token: 0x04000051 RID: 81
		private GameObject defaultCrosshairPrefab;

		// Token: 0x04000052 RID: 82
		private uint loopSoundInstanceId;
	}
}
