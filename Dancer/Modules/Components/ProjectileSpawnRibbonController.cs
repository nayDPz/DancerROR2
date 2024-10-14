using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.Modules.Components
{

    public class ProjectileSpawnRibbonController : MonoBehaviour, IProjectileImpactBehavior
    {
        private ProjectileController controller;

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            Collider collider = impactInfo.collider;
            HurtBox component = collider.GetComponent<HurtBox>();
            if (!component && NetworkServer.active)
            {
                GameObject gameObject = Object.Instantiate(Assets.ribbonController, base.gameObject.transform.position, Quaternion.identity);
                RibbonController component2 = gameObject.GetComponent<RibbonController>();
                component2.timer = Buffs.ribbonDebuffDuration;
                component2.inflictorRoot = controller.owner;
                component2.spreadsRemaining = 3;
                NetworkServer.Spawn(gameObject);
                component2.StartRibbon();
            }
        }

        private void Awake()
        {
            controller = GetComponent<ProjectileController>();
        }

        private void OnDestroy()
        {
        }
    }
}
