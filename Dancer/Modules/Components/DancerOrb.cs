using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoR2;
using RoR2.Orbs;

namespace Dancer.Modules.Components
{
	// Token: 0x02000636 RID: 1590
	public class DancerOrb : Orb
	{
		// Token: 0x06002711 RID: 10001 RVA: 0x0009D3D8 File Offset: 0x0009B5D8
		public override void Begin()
		{
			
		}

		// Token: 0x06002712 RID: 10002 RVA: 0x0009D51C File Offset: 0x0009B71C
		public override void OnArrival()
		{
			if (this.target)
			{
				//Vector3 direction = this.search.searchOrigin - this.target.transform.position;
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageValue;
					damageInfo.attacker = this.attacker;
					damageInfo.inflictor = this.inflictor;
					damageInfo.force = Vector3.zero;// direction * this.pullForce;
					damageInfo.crit = this.isCrit;
					damageInfo.procChainMask = this.procChainMask;
					damageInfo.procCoefficient = this.procCoefficient;
					damageInfo.position = this.target.transform.position;
					damageInfo.damageColorIndex = this.damageColorIndex;
					damageInfo.damageType = this.damageType;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}			
			}
		}

		public HurtBox PickNextTarget(Vector3 position)
		{
			if (this.search == null)
			{
				this.search = new BullseyeSearch();
			}
			this.search.searchOrigin = position;
			this.search.searchDirection = Vector3.zero;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.teamIndex);
			this.search.filterByLoS = false;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.range;
			this.search.RefreshCandidates();
			HurtBox hurtBox = (from v in this.search.GetResults()
							   where !this.bouncedObjects.Contains(v.healthComponent)
							   select v).FirstOrDefault<HurtBox>();
			if (hurtBox)
			{
				this.bouncedObjects.Add(hurtBox.healthComponent);
			}
			return hurtBox;
		}

		public float speed = 100f;

		public float damageValue;

		public GameObject attacker;

		public GameObject inflictor;

		public int bouncesRemaining;

		public List<HealthComponent> bouncedObjects;

		public TeamIndex teamIndex;

		public bool isCrit;

		public ProcChainMask procChainMask;

		public float procCoefficient = 1f;

		public DamageColorIndex damageColorIndex;

		public float range = 20f;
		public float damageCoefficientPerBounce = 1f;

		public int targetsToFindPerBounce = 1;

		public float pullForce;

		public DamageType damageType;

		private bool canBounceOnSameTarget;

		private bool failedToKill;

		public LightningOrb.LightningType lightningType;

		private BullseyeSearch search;

	}
}
