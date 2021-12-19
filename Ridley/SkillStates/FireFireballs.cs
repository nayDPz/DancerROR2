using System;
using EntityStates;
using EntityStates.LemurianMonster;
using Ridley.Modules;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x02000008 RID: 8
	public class FireFireballs : BaseSkillState
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002BFC File Offset: 0x00000DFC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration /= this.attackSpeedStat;
			this.numFireballs = (int)(this.charge / 0.33f) + 1;
			this.fireInterval /= this.attackSpeedStat;
			base.StartAimMode(this.fireInterval, false);
			base.GetModelAnimator();
			this.aimRay = base.GetAimRay();
			bool isGrounded = base.isGrounded;
			if (isGrounded)
			{
				base.PlayAnimation("FullBody, Override", "NSpecShoot", "Slash.playbackRate", 1f);
			}
			else
			{
				base.PlayAnimation("FullBody, Override", "NSpecAirShoot", "Slash.playbackRate", 1f);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002C9C File Offset: 0x00000E9C
		private void Fire()
		{
			this.ballsFired++;
			this.hasFired = true;
			Util.PlaySound("FireFireball", base.gameObject);
			bool flag = FireFireball.effectPrefab;
			if (flag)
			{
				EffectManager.SimpleMuzzleFlash(FireFireball.effectPrefab, base.gameObject, "Mouth", false);
			}
			bool isAuthority = base.isAuthority;
			if (isAuthority)
			{
				Projectiles.ridleyFireballPrefab.GetComponent<ProjectileExplosion>().explosionEffect = GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab;
				ProjectileManager.instance.FireProjectile(Projectiles.ridleyFireballPrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(this.aimRay.direction), base.gameObject, this.damageStat * FireFireballs.damageCoefficient, FireFireballs.force, base.RollCrit(), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002D52 File Offset: 0x00000F52
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002D5C File Offset: 0x00000F5C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireStopwatch += Time.fixedDeltaTime;
			bool flag = this.fireStopwatch >= this.fireInterval && this.ballsFired < this.numFireballs;
			if (flag)
			{
				this.fireStopwatch = 0f;
				this.Fire();
			}
			bool flag2 = base.fixedAge >= this.duration && base.isAuthority;
			if (flag2)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002DE4 File Offset: 0x00000FE4
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000053 RID: 83
		public float charge;

		// Token: 0x04000054 RID: 84
		public static float damageCoefficient = 3f;

		// Token: 0x04000055 RID: 85
		public static float force = 200f;

		// Token: 0x04000056 RID: 86
		private bool hasFired;

		// Token: 0x04000057 RID: 87
		private float duration = 0.5f;

		// Token: 0x04000058 RID: 88
		private float fireInterval = 0.125f;

		// Token: 0x04000059 RID: 89
		private float fireStopwatch = 0.1f;

		// Token: 0x0400005A RID: 90
		private int numFireballs;

		// Token: 0x0400005B RID: 91
		private int ballsFired;

		// Token: 0x0400005C RID: 92
		private Ray aimRay;

		// Token: 0x0400005D RID: 93
		private bool a;
	}
}
