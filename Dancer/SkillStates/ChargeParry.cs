using System;
using EntityStates;
using EntityStates.LemurianMonster;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates
{
	public class ChargeParry : BaseSkillState
	{

		private float duration;

		private Animator animator;

		private ChildLocator childLocator;

		private protected GameObject chargeEffectInstance;

		public GameObject chargeEffectPrefab;

		public string chargeSoundString;

		public float baseDuration = 1f;

		public GameObject crosshairOverridePrefab;

		public float minChargeDuration = 1f;

		private GameObject defaultCrosshairPrefab;

		private float timer;

		private float perfectTime = Modules.StaticValues.perfectParryTime;

		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.timer = this.duration;
			this.animator = base.GetModelAnimator();
			this.childLocator = base.GetModelChildLocator();
			this.PlayChargeAnimation();
			Util.PlayAttackSpeedSound("FireballCharge", base.gameObject, this.attackSpeedStat);
			this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;

			if (NetworkServer.active)
				base.characterBody.AddBuff(Modules.Buffs.parryBuff);
			if (this.crosshairOverridePrefab)
			{
				base.characterBody.crosshairPrefab = this.crosshairOverridePrefab;
			}
			base.StartAimMode(this.duration + 2f, false);
		}

		public override void OnExit()
		{
			if(NetworkServer.active)
				base.characterBody.RemoveBuff(Modules.Buffs.parryBuff);
			if (base.characterBody)
			{
				base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			}
			base.OnExit();
		}

		protected float CalcCharge()
		{
			if (base.fixedAge <= this.perfectTime)
				return 1;
			else
				return Mathf.Clamp01(this.timer / this.duration);
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();

			this.timer -= Time.fixedDeltaTime;
			float charge = this.CalcCharge();

			if (Util.HasEffectiveAuthority(base.gameObject) && 
				((!base.IsKeyDownAuthority() && base.fixedAge >= this.minChargeDuration) 
				|| !base.characterBody.HasBuff(Modules.Buffs.parryBuff)))
			{
				
				Riposte riposte = new Riposte();
				riposte.charge = charge;
				this.outer.SetNextState(riposte);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		protected virtual void PlayChargeAnimation()
		{
			base.PlayAnimation("Head, Override", "NSpecStart", "Slash.playbackRate", 0.225f);

		}

	}
}
