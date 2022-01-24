using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Dancer.Modules.Components;

namespace Dancer.Modules
{
    internal static class Projectiles
    {
        internal static GameObject dancerRibbonProjectile;
        
        internal static void RegisterProjectiles()
        {
            dancerRibbonProjectile = CloneProjectilePrefab("Fireball", "RibbonProjectile");

            SphereCollider zs = dancerRibbonProjectile.GetComponent<SphereCollider>();
            zs.radius = 1.25f;

            ProjectileController c = dancerRibbonProjectile.GetComponent<ProjectileController>();
            GameObject ghostPrefab = PrefabAPI.InstantiateClone(Assets.mainAssetBundle.LoadAsset<GameObject>("RibbonBall"), "RibbonProjectileGhost");
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();
            c.ghostPrefab = ghostPrefab;

            ProjectileSimple s = dancerRibbonProjectile.GetComponent<ProjectileSimple>();
            s.desiredForwardSpeed = 150f;

            ProjectileDamage d = dancerRibbonProjectile.GetComponent<ProjectileDamage>();
            d.damageType = DamageType.FruitOnHit;

            ProjectileSingleTargetImpact i = dancerRibbonProjectile.GetComponent<ProjectileSingleTargetImpact>();
            i.enemyHitSoundString = "WhipHit1";
            i.hitSoundString = "LungeHit";

            dancerRibbonProjectile.AddComponent<ProjectileSpawnRibbonController>();

            Modules.Prefabs.projectilePrefabs.Add(dancerRibbonProjectile);
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