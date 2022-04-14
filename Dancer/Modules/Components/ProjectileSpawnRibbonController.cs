using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;
namespace Dancer.Modules.Components
{
    public class ProjectileSpawnRibbonController : MonoBehaviour, IProjectileImpactBehavior
    {
        private ProjectileController controller;

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            Collider collider = impactInfo.collider;
            HurtBox component = collider.GetComponent<HurtBox>();
            if (component)
            {
                return;         
            }

            if (NetworkServer.active)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.ribbonController, base.gameObject.transform.position, Quaternion.identity);
                RibbonController newRibbon = gameObject.GetComponent<RibbonController>();
                newRibbon.timer = Modules.Buffs.ribbonDebuffDuration;
                newRibbon.inflictorRoot = this.controller.owner;
                newRibbon.spreadsRemaining = Modules.StaticValues.ribbonInitialTargets + 1;
                NetworkServer.Spawn(gameObject);
                newRibbon.StartRibbon();
            }

        }

        private void Awake()
        {
            this.controller = base.GetComponent<ProjectileController>();
        }
        private void OnDestroy()
        {
            
            
        }

    }
}
