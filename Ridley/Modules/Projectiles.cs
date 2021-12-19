using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Ridley.Modules.Components;

namespace Ridley.Modules
{
    internal static class Projectiles
    {
        internal static GameObject ridleyFireballPrefab;
        internal static void RegisterProjectiles()
        {
            ridleyFireballPrefab = CloneProjectilePrefab("MageLightningBombProjectile", "RidleyFireball");

            ProjectileSimple s = ridleyFireballPrefab.GetComponent<ProjectileSimple>();
            s.desiredForwardSpeed = 60f;

            AntiGravityForce a = ridleyFireballPrefab.GetComponent<AntiGravityForce>();
            a.antiGravityCoefficient = 0.1f;

            GameObject g = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectileghosts/FireballGhost"), "RidleyFireballGhost");
            g.transform.localScale *= 2.5f;
            ProjectileController ss = ridleyFireballPrefab.GetComponent<ProjectileController>();
            ss.ghostPrefab = g;

            ProjectileImpactExplosion p = ridleyFireballPrefab.GetComponent<ProjectileImpactExplosion>();
            p.lifetime = 5f;
            p.blastDamageCoefficient = 0.5f;
            p.blastRadius = 12f;
            p.impactEffect = GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab;
            p.explosionEffect = GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab;
            p.explosionSoundString = "FireballHit";
            p.falloffModel = BlastAttack.FalloffModel.Linear;

            ProjectileExplosion pz = ridleyFireballPrefab.GetComponent<ProjectileExplosion>();
            pz.explosionEffect = GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab;
            pz.falloffModel = BlastAttack.FalloffModel.Linear;
            ProjectileProximityBeamController bb = ridleyFireballPrefab.GetComponent<ProjectileProximityBeamController>();
            GameObject.Destroy(bb);

            GameObject.Destroy(ridleyFireballPrefab.GetComponent<AkEvent>());
            /*
            RidleyFireballBouncer b = ridleyFireballPrefab.AddComponent<RidleyFireballBouncer>();
            InitializeImpactExplosion(b);
            b.lifetime = 5f;
            b.blastDamageCoefficient = 1f;
            b.blastRadius = 12f;
            b.explosionEffect = GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab;
            b.explosionSoundString = "FireballHit";
            */

            Modules.Prefabs.projectilePrefabs.Add(ridleyFireballPrefab);
        }


        private static void InitializeImpactExplosion(RidleyFireballBouncer projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.explosionSoundString = "";
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeExpiredSoundString = "";

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

       

        private static void InitializeProjectileSimple(ProjectileSimple projectileSimple)
        {
            projectileSimple.desiredForwardSpeed = 0f;
            projectileSimple.oscillationStopwatch = 0f;
            projectileSimple.deltaHeight = 0f;
            projectileSimple.oscillateSpeed = 0f;
            projectileSimple.oscillateMagnitude = 0f;
            projectileSimple.oscillate = false;
            projectileSimple.stopwatch = 0f;
            projectileSimple.velocityOverLifetime = null;
            projectileSimple.enableVelocityOverLifetime = false;
            projectileSimple.updateAfterFiring = true;
            projectileSimple.lifetime = 1f;

            projectileSimple.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}