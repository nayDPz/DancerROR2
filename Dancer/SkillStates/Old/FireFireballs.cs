using System;
using EntityStates;
using EntityStates.LemurianMonster;
using Dancer.Modules;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Dancer.SkillStates
{
	public class FireFireballs : BaseSkillState
	{
		public float charge;
		public static float damageCoefficient = 2.5f;
		public static float force = 200f;
		private float duration = 0.5f;
		private float fireInterval = 0.125f;
		private float fireStopwatch = 0.1f;
		private int numFireballs;
		private int ballsFired;
		private Ray aimRay;
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration /= this.attackSpeedStat;
			this.numFireballs = (int)(this.charge / 0.25f) + 1;
			this.fireInterval /= this.attackSpeedStat;
			base.StartAimMode(this.fireInterval, false);
			base.GetModelAnimator();
			this.aimRay = base.GetAimRay();
			if (base.isGrounded)
			{
				base.PlayAnimation("FullBody, Override", "NSpecShoot", "Slash.playbackRate", 1f);
			}
			else
			{
				base.PlayAnimation("FullBody, Override", "NSpecAirShoot", "Slash.playbackRate", 1f);
			}
		}

		private void Fire()
		{
			this.ballsFired++;
			Util.PlaySound("FireFireball", base.gameObject);
			if (FireFireball.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireFireball.effectPrefab, base.gameObject, "Mouth", false);
			}
			Projectiles.dancerRibbonProjectile.GetComponent<ProjectileExplosion>().explosionEffect = GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab;
			if (Util.HasEffectiveAuthority(base.gameObject))
			{			
				ProjectileManager.instance.FireProjectile(Projectiles.dancerRibbonProjectile, this.aimRay.origin, Util.QuaternionSafeLookRotation(this.aimRay.direction), base.gameObject, this.damageStat * FireFireballs.damageCoefficient, FireFireballs.force, base.RollCrit(), DamageColorIndex.Default, null, -1f);
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireStopwatch += Time.fixedDeltaTime;
			if (this.fireStopwatch >= this.fireInterval && this.ballsFired < this.numFireballs)
			{
				this.fireStopwatch = 0f;
				this.Fire();
			}
			if (base.fixedAge >= this.duration && Util.HasEffectiveAuthority(base.gameObject))
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		

	}
}
