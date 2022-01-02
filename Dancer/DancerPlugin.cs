using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using Dancer.Modules.Components;
using System.Collections.Generic;
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Dancer
{
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.KingEnderBrine.ScrollableLobbyUI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
    })]

    public class DancerPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.ndp.DancerBeta";
        public const string MODNAME = "DancerBeta";
        public const string MODVERSION = "0.0.1";

        public const string developerPrefix = "NDP";

        public static bool scepterInstalled = false;


        public static DancerPlugin instance;

        private void Awake()
        {
            instance = this;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter")) scepterInstalled = true;

            // load assets and read config
            Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules
            Modules.CameraParams.InitializeParams();
            Modules.Survivors.Dancer.CreateCharacter();
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        private void Start()
        {
            Modules.Survivors.Dancer.SetItemDisplays();
        }

        private void Hook()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public static List<HealthComponent> EnemyHealthComponentsFromRaycastHits(RaycastHit[] raycastHits, GameObject attacker)
        {
            List<Collider> colliders = new List<Collider>();
            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].collider)
                {
                    //Debug.Log(array2[i].collider.ToString());
                    colliders.Add(raycastHits[i].collider);
                }

            }
            return EnemyHealthComponentsFromColliders(colliders.ToArray(), attacker);
        }

        public static List<HealthComponent> EnemyHealthComponentsFromColliders(Collider[] colliders, GameObject attacker)
        {
            List<HealthComponent> healthComponents = new List<HealthComponent>();
            for (int i = 0; i < colliders.Length; i++)
            {
                HurtBox hurtBox = colliders[i].GetComponent<HurtBox>();
                //Debug.Log("zz" + hurtBox.ToString());
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    //Debug.Log("hh " + healthComponent.ToString());
                    if (healthComponent)
                    {
                        TeamComponent team = healthComponent.GetComponent<TeamComponent>();
                        TeamComponent self = attacker.GetComponent<TeamComponent>();
                        bool enemy = team.teamIndex != self.teamIndex;
                        if (enemy)
                        {
                            if (!healthComponents.Contains(healthComponent))
                            {
                                healthComponents.Add(healthComponent);
                            }
                        }
                    }
                }
            }
            return healthComponents;
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo != null && damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
            {
                if (self.GetComponent<CharacterBody>().baseNameToken == "NDP_DANCER_BODY_NAME")
                {
                    if(NetworkServer.active)//(damageInfo.damageType & DamageType.DoT) != DamageType.DoT)
                    {
                        //float num = Mathf.Min(self.body.armor, (self.body.baseArmor + self.body.levelArmor * self.body.level) * 2f);
                        float num = self.body.armor;
                        if(self.combinedHealthFraction < 0.5f && (damageInfo.damageType & DamageType.DoT) != DamageType.DoT)
                        {
                            damageInfo.damage -= num;
                            if (damageInfo.damage < 0f)
                            {
                                self.Heal(Mathf.Abs(damageInfo.damage), default(RoR2.ProcChainMask), true);
                                damageInfo.damage = 0f;
                            }
                        }
                        else
                        {
                            damageInfo.damage = Mathf.Max(1f, damageInfo.damage - num);
                        }
                        
                    }
                    


                }
            }

            




            orig(self, damageInfo);

        }
    }
}