using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using Ridley.Modules.Components;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Ridley
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

    public class RidleyPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.ndp.RidleyBeta";
        public const string MODNAME = "RidleyBeta";
        public const string MODVERSION = "0.0.1";

        // a prefix for name tokens to prevent conflicts
        public const string developerPrefix = "NDP";

        // soft dependency stuff
        public static bool starstormInstalled = false;
        public static bool scepterInstalled = false;
        public static bool scrollableLobbyInstalled = false;


        public static RidleyPlugin instance;

        private void Awake()
        {
            instance = this;

            // check for soft dependencies
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2")) starstormInstalled = true;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter")) scepterInstalled = true;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ScrollableLobbyUI")) scrollableLobbyInstalled = true;

            // load assets and read config
            Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            Modules.Survivors.Ridley.CreateCharacter();
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        private void Start()
        {
            Modules.Survivors.Ridley.SetItemDisplays();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }


        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.armorBuff))
                {
                    //self.armor += 300f;
                }
                if(self.GetBuffCount(Modules.Buffs.invulChargeStacks) >= 100)
                {
                    self.SetBuffCount(Modules.Buffs.invulChargeStacks.buffIndex, self.GetBuffCount(Modules.Buffs.invulChargeStacks) - 100);
                    self.AddBuff(Modules.Buffs.invulCharge);
                }
            }
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.speedBuff))
                {
                    self.moveSpeed *= 1.3f;
                    self.attackSpeed *= 1.3f;
                }
            }

        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool addInvuln = false;
            if (damageInfo != null && damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
            {
                if (self.GetComponent<CharacterBody>().baseNameToken == "NDP_RIDLEY_BODY_NAME")
                {
                    if((damageInfo.damageType & DamageType.DoT) != DamageType.DoT)
                    {
                        float num = Mathf.Min(self.body.armor, (self.body.baseArmor + self.body.levelArmor * self.body.level) * 2f);
                        damageInfo.damage -= num;
                        bool flag3 = damageInfo.damage < 0f;
                        if (flag3)
                        {
                            self.Heal(Mathf.Abs(damageInfo.damage / 2), default(RoR2.ProcChainMask), true);
                            damageInfo.damage = 0f;
                        }
                    }
                    


                }
            }

            




            orig(self, damageInfo);

            if(addInvuln)
                self.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 3f);
        }
    }
}