using Dancer.Modules.Components;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Dancer.Modules
{

    internal static class Projectiles
    {
        internal static GameObject dancerRibbonProjectile;

        internal static void RegisterProjectiles()
        {
            dancerRibbonProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion(), "RibbonProjectile");
            SphereCollider component = dancerRibbonProjectile.GetComponent<SphereCollider>();
            component.radius = 1.25f;
            ProjectileController component2 = dancerRibbonProjectile.GetComponent<ProjectileController>();
            GameObject gameObject = PrefabAPI.InstantiateClone(Assets.mainAssetBundle.LoadAsset<GameObject>("RibbonBall"), "RibbonProjectileGhost");
            if (!gameObject.GetComponent<NetworkIdentity>())
            {
                gameObject.AddComponent<NetworkIdentity>();
            }
            if (!gameObject.GetComponent<ProjectileGhostController>())
            {
                gameObject.AddComponent<ProjectileGhostController>();
            }
            component2.ghostPrefab = gameObject;
            ProjectileSimple component3 = dancerRibbonProjectile.GetComponent<ProjectileSimple>();
            component3.desiredForwardSpeed = 150f;
            ProjectileDamage component4 = dancerRibbonProjectile.GetComponent<ProjectileDamage>();
            component4.damageType = DamageType.FruitOnHit;
            ProjectileSingleTargetImpact component5 = dancerRibbonProjectile.GetComponent<ProjectileSingleTargetImpact>();
            component5.enemyHitSoundString = "WhipHit1";
            component5.hitSoundString = "LungeHit";
            dancerRibbonProjectile.AddComponent<ProjectileSpawnRibbonController>();
            Prefabs.projectilePrefabs.Add(dancerRibbonProjectile);
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
            GameObject gameObject = Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!gameObject.GetComponent<NetworkIdentity>())
            {
                gameObject.AddComponent<NetworkIdentity>();
            }
            if (!gameObject.GetComponent<ProjectileGhostController>())
            {
                gameObject.AddComponent<ProjectileGhostController>();
            }
            Assets.ConvertAllRenderersToHopooShader(gameObject);
            return gameObject;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            return PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
        }
    }
}
