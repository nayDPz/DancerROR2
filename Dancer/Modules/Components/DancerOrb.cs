using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dancer.Modules.Components
{

    public class DancerOrb : Orb
    {
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

        public override void Begin()
        {
        }

        public override void OnArrival()
        {
            if ((bool)target)
            {
                HealthComponent healthComponent = target.healthComponent;
                if ((bool)healthComponent)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = damageValue;
                    damageInfo.attacker = attacker;
                    damageInfo.inflictor = inflictor;
                    damageInfo.force = Vector3.zero;
                    damageInfo.crit = isCrit;
                    damageInfo.procChainMask = procChainMask;
                    damageInfo.procCoefficient = procCoefficient;
                    damageInfo.position = target.transform.position;
                    damageInfo.damageColorIndex = damageColorIndex;
                    damageInfo.damageType = damageType;
                    healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
                }
            }
        }

        public HurtBox PickNextTarget(Vector3 position)
        {
            if (search == null)
            {
                search = new BullseyeSearch();
            }
            search.searchOrigin = position;
            search.searchDirection = Vector3.zero;
            search.teamMaskFilter = TeamMask.allButNeutral;
            search.teamMaskFilter.RemoveTeam(teamIndex);
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = range;
            search.RefreshCandidates();
            HurtBox hurtBox = (from v in search.GetResults()
                               where !bouncedObjects.Contains(v.healthComponent)
                               select v).FirstOrDefault();
            if ((bool)hurtBox)
            {
                bouncedObjects.Add(hurtBox.healthComponent);
            }
            return hurtBox;
        }
    }
}
