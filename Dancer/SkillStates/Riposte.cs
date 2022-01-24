using System;
using EntityStates;
using EntityStates.LemurianMonster;
using Dancer.Modules;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Dancer.SkillStates
{
	public class Riposte : BaseSkillState
	{
		public float charge;
		public static float radius = 5f;
		public static float perfectDamageCoefficient = 24f;
		public static float maxDamageCoefficient = 18f;
		public static float minDamageCoefficient = 12f;
		private float damageCoefficient;
		public static Vector3 force = Vector3.up * 2000f;
		public static float procCoefficient = 1f;
		private float duration = 0.67f;
		private bool hasFired;
		private Ray aimRay;
		private float fireTime;
		
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration /= this.attackSpeedStat;
			if (this.charge >= 1)
				this.damageCoefficient = perfectDamageCoefficient;
			else
				this.damageCoefficient = Mathf.Lerp(maxDamageCoefficient, minDamageCoefficient, this.charge);

			this.fireTime = (.5f / .67f) * this.duration;
			base.StartAimMode(this.duration, false);
			base.GetModelAnimator();
			this.aimRay = base.GetAimRay();
			base.PlayAnimation("FullBody, Override", "FAir", "Slash.playbackRate", 1f);

		}

		private void Fire()
		{
			if(!this.hasFired)
            {
				this.hasFired = true;
				Util.PlaySound("FireFireball", base.gameObject);
				Vector3 attackPosition = base.GetAimRay().GetPoint(3f);
				attackPosition.y = 0f;
				if (new BlastAttack
				{
					attacker = this.gameObject,
					procChainMask = default(ProcChainMask),
					impactEffect = EffectIndex.Invalid,
					losType = BlastAttack.LoSType.NearestHit,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					procCoefficient = procCoefficient,
					bonusForce = force,
					baseForce = 0f,
					baseDamage = this.damageCoefficient * this.damageStat,
					falloffModel = BlastAttack.FalloffModel.None,
					radius = radius,
					position = attackPosition,
					attackerFiltering = AttackerFiltering.NeverHit,
					teamIndex = base.GetTeam(),
					inflictor = base.gameObject,
					crit = base.RollCrit(),
				}.Fire().hitCount > 0)
				{
					this.OnHitEnemyAuthority();
					Util.PlaySound("SwordHit2", base.gameObject);
				}
			}
			


		}

		private void OnHitEnemyAuthority()
		{

		}

		public override void OnExit()
		{
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.fixedAge += Time.fixedDeltaTime;
			if (base.fixedAge >= this.fireTime)
			{
				this.Fire();
			}
			if (base.fixedAge >= this.duration && Util.HasEffectiveAuthority(base.gameObject))
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}



	}
}
