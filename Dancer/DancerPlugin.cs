using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using RoR2.Orbs;
using Dancer.Modules.Components;
using System.Collections.Generic;
using System.Linq;
using Dancer.SkillStates;
using EntityStates;
using System;
using R2API.Networking.Interfaces;
using R2API.Networking;

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
        "NetworkingAPI",
        "DotAPI",
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

            Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates();
            Modules.Buffs.RegisterBuffs();
            Modules.Projectiles.RegisterProjectiles();
            Modules.Tokens.AddTokens(); 
            Modules.ItemDisplays.PopulateDisplays();
            Modules.CameraParams.InitializeParams();
            Modules.Survivors.Dancer.CreateCharacter();
            new Modules.ContentPacks().Initialize();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;

            Hook();
        }



        private void LateSetup(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            Modules.Survivors.Dancer.SetItemDisplays();

            
        }

        private void Hook()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            
            orig(self);

            if (self.HasBuff(Modules.Buffs.ribbonDebuff))
            {
                self.moveSpeed /= 2f;
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            RibbonController ribbon = RibbonController.FindRibbonController(self.gameObject);

            if (damageInfo != null && damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
            {
                if (damageInfo.attacker.GetComponent<CharacterBody>().baseNameToken == "NDP_DANCER_BODY_NAME")
                {

                    
                    if (ribbon)
                    {
                        if(damageInfo.procChainMask.mask == 0 && damageInfo.damageType != DamageType.AOE && damageInfo.damageType != DamageType.DoT && damageInfo.damageType != DamageType.ApplyMercExpose)
                        {
                            HealthComponent h = damageInfo.attacker.GetComponent<HealthComponent>();
                            if (h && damageInfo.damage > 0f)
                            {
                                float b = h.fullHealth * Modules.StaticValues.ribbonBarrierFraction * damageInfo.procCoefficient;
                                h.AddBarrier(b);
                            }
                            

                            bool isCrit = damageInfo.crit;
                            float damageValue = Modules.StaticValues.ribbonChainDamageCoefficient * damageInfo.attacker.GetComponent<CharacterBody>().baseDamage;
                            TeamIndex teamIndex = damageInfo.attacker.GetComponent<CharacterBody>().teamComponent.teamIndex;
                            if (ribbon.NetworknextRoot)
                            {
                                CharacterBody body = ribbon.nextRoot.GetComponent<CharacterBody>();
                                if (body)
                                {
                                    DancerOrb dancerOrb = new DancerOrb();
                                    dancerOrb.attacker = damageInfo.attacker;
                                    dancerOrb.bouncedObjects = null;
                                    dancerOrb.bouncesRemaining = 0;
                                    dancerOrb.damageCoefficientPerBounce = 1f;
                                    dancerOrb.damageColorIndex = DamageColorIndex.Item;
                                    dancerOrb.damageValue = damageValue;
                                    dancerOrb.isCrit = isCrit;
                                    dancerOrb.origin = damageInfo.position;
                                    dancerOrb.procChainMask = default(ProcChainMask);
                                    dancerOrb.procCoefficient = 0f;
                                    dancerOrb.range = 0f;
                                    dancerOrb.teamIndex = teamIndex;
                                    dancerOrb.target = body.mainHurtBox;
                                    dancerOrb.duration = Modules.StaticValues.ribbonChainTime;
                                    OrbManager.instance.AddOrb(dancerOrb);
                                }

                            }
                            else
                            {
                                ribbon.inflictorRoot = damageInfo.attacker;
                                ribbon.SearchNewTarget();
                            }
                        }                     

                    }
                 
                    if (damageInfo.damageType == DamageType.FruitOnHit) // figure out custom dmg types eventually
                    {
                        damageInfo.damageType = DamageType.Generic;               
                        float duration = Modules.Buffs.ribbonDebuffDuration;
                        
                        if (ribbon)
                        {
                            ribbon.SyncRibbonTimersToNewTime(duration);
                        }
                        else
                        {
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.ribbonController, self.gameObject.transform);
                            RibbonController newRibbon = gameObject.GetComponent<RibbonController>();
                            newRibbon.timer = duration;
                            newRibbon.NetworkownerRoot = self.gameObject;
                            newRibbon.inflictorRoot = damageInfo.attacker;
                            NetworkServer.Spawn(gameObject);
                            newRibbon.StartRibbon();
                        }
                    }

                    if(damageInfo.damageType == DamageType.ApplyMercExpose)
                    {
                        damageInfo.damageType = DamageType.Generic;
                    }

                }

                if(self.body.baseNameToken == "NDP_DANCER_BODY_NAME")
                {
                    if(self.body.HasBuff(Modules.Buffs.parryBuff))
                    {
                        damageInfo.rejected = true;
                        self.body.RemoveBuff(Modules.Buffs.parryBuff);
                        self.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, Modules.StaticValues.parryInvincibilityDuration);
                    }
                }
            }

            




            orig(self, damageInfo);

            if(ribbon)
            {
                if(!self.alive)
                {
                    if(!ribbon.ribbonAttached)
                        ribbon.DetachFromOwner();
                }
            }

        }
    }
 
}