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
            On.RoR2.CharacterBody.Update += CharacterBody_Update;
            //On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
        }

        private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if(damageReport.damageInfo != null)
            {
                DamageInfo damageInfo = damageReport.damageInfo;

                if (damageInfo.attacker.GetComponent<CharacterBody>().baseNameToken == "NDP_DANCER_BODY_NAME")
                {
                    if (damageInfo.damageType == DamageType.FruitOnHit)
                    {
                        damageInfo.damageType = DamageType.Generic;
                        float duration = Modules.Buffs.ribbonDebuffDuration;
                        self.AddTimedBuff(Modules.Buffs.ribbonDebuff, duration);

                        EntityStateMachine component = self.GetComponent<EntityStateMachine>();
                        if (self.GetComponent<SetStateOnHurt>() && self.GetComponent<SetStateOnHurt>().canBeFrozen && component && !self.isChampion)
                        {

                            RibbonedState newNextState = new RibbonedState
                            {
                                duration = duration,
                            };
                            component.SetInterruptState(newNextState, InterruptPriority.Death);


                        }

                    }
                }
                

            }
            
        }

        private void CharacterBody_Update(On.RoR2.CharacterBody.orig_Update orig, CharacterBody self)
        {
            if(self.HasBuff(Modules.Buffs.ribbonDebuff))
            {
                float time = Modules.Buffs.ribbonDebuffDuration;
                foreach(CharacterBody.TimedBuff buff in self.timedBuffs)
                {
                    if (buff.buffIndex == Modules.Buffs.ribbonDebuff.buffIndex)
                        time = buff.timer;
                }
                EntityStateMachine e = self.GetComponent<EntityStateMachine>();
                if (e)
                {
                    if (self.GetComponent<SetStateOnHurt>() && self.GetComponent<SetStateOnHurt>().canBeFrozen)
                    {
                        if(!(e.state is RibbonedState))
                        {
                            RibbonedState newNextState = new RibbonedState
                            {
                                duration = time,
                            };
                            e.SetInterruptState(newNextState, InterruptPriority.Frozen);
                        }                    
                        else
                        {
                            RibbonedState ribbonedState = e.state as RibbonedState;
                            if (ribbonedState.timer < time)
                            {
                                ribbonedState.SetNewTimer(time);
                            }
                        }

                    }
                    
                }
            }

            orig(self);

        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo != null && damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
            {
                if (damageInfo.attacker.GetComponent<CharacterBody>().baseNameToken == "NDP_DANCER_BODY_NAME")
                {
                    if(self.body.HasBuff(Modules.Buffs.ribbonDebuff) && damageInfo.procChainMask.mask == 0 && damageInfo.damageType != DamageType.DoT)
                    {                      
                        damageInfo.attacker.GetComponent<CharacterBody>().healthComponent.HealFraction(.03f * damageInfo.procCoefficient, default(ProcChainMask));
                        Vector3 position = self.body.corePosition;
                        RibbonController ribbon = self.GetComponent<RibbonController>();
                        if(ribbon)
                        {
                            #region hasribbon
                            bool isCrit = damageInfo.crit;
                            float damageValue = 0f * damageInfo.attacker.GetComponent<CharacterBody>().baseDamage; //dmg co
                            TeamIndex teamIndex2 = damageInfo.attacker.GetComponent<CharacterBody>().teamComponent.teamIndex;
                            if (ribbon.nextRoot) // orbs too damb buggy goddd
                            {
                                CharacterBody body = ribbon.nextRoot.GetComponent<CharacterBody>();
                                if(body)
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
                                    dancerOrb.teamIndex = teamIndex2;
                                    dancerOrb.target = body.mainHurtBox;
                                    dancerOrb.duration = 0.2f; //change to static value
                                    OrbManager.instance.AddOrb(dancerOrb);
                                }
                                
                            }
                            else
                            {
                                #region searchnewribbon
                                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                                bullseyeSearch.searchOrigin = position;
                                bullseyeSearch.maxDistanceFilter = Modules.Buffs.ribbonSpreadRange;
                                bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
                                bullseyeSearch.teamMaskFilter.RemoveTeam(damageInfo.attacker.GetComponent<CharacterBody>().teamComponent.teamIndex);
                                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
                                bullseyeSearch.filterByLoS = true;
                                bullseyeSearch.RefreshCandidates();
                                bullseyeSearch.FilterOutGameObject(self.gameObject);
                                foreach (HurtBox hurtBox in bullseyeSearch.GetResults())
                                {
                                    if (hurtBox.healthComponent && hurtBox.healthComponent.body)
                                    {
                                        if (hurtBox.healthComponent.body.HasBuff(Modules.Buffs.ribbonDebuff))
                                            bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
                                        if (ribbon.previousRoot && ribbon.previousRoot == hurtBox.healthComponent.gameObject)
                                            bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
                                    }
                                }
                                HurtBox target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
                                if (target && target.healthComponent && target.healthComponent.body)
                                {
                                    
                                    ribbon.nextRoot = target.healthComponent.body.gameObject;
                                    ribbon.inflictorRoot = damageInfo.attacker;

                                    //Debug.Log("Setting " + self.gameObject.name + "'s next to " + target.healthComponent.body.gameObject);
                                }
                                #endregion
                            }

                            #endregion
                        }
                        else
                        {
                         
                            #region searchnewribbon
                            BullseyeSearch bullseyeSearch = new BullseyeSearch();
                            bullseyeSearch.searchOrigin = position;
                            bullseyeSearch.maxDistanceFilter = Modules.Buffs.ribbonSpreadRange;
                            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
                            bullseyeSearch.teamMaskFilter.RemoveTeam(damageInfo.attacker.GetComponent<CharacterBody>().teamComponent.teamIndex);
                            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
                            bullseyeSearch.filterByLoS = true;
                            bullseyeSearch.RefreshCandidates();

                            foreach (HurtBox hurtBox in bullseyeSearch.GetResults())
                            {
                                if (hurtBox.healthComponent && hurtBox.healthComponent.body)
                                {
                                    if (hurtBox.healthComponent.body.HasBuff(Modules.Buffs.ribbonDebuff))
                                        bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
                                    if (hurtBox.healthComponent.gameObject.GetComponent<RibbonController>())
                                        bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
                                }
                            }
                            HurtBox target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
                            if (target && target.healthComponent && target.healthComponent.body)
                            {
                                RibbonController newRibbon = self.gameObject.AddComponent<RibbonController>();
                                newRibbon.nextRoot = target.healthComponent.body.gameObject;
                                //Debug.Log("Setting " + self.gameObject.name + "'s next to " + target.healthComponent.body.gameObject);
                                newRibbon.inflictorRoot = damageInfo.attacker;
                            }
                            #endregion
                        }
                    }
                    if (damageInfo.damageType == DamageType.FruitOnHit)
                    {
                        RibbonController ribbon = self.gameObject.GetComponent<RibbonController>();
                        damageInfo.damageType = DamageType.Generic;
                        float duration = Modules.Buffs.ribbonDebuffDuration;
                        if(!self.body.HasBuff(Modules.Buffs.ribbonDebuff))
                            self.body.AddTimedBuff(Modules.Buffs.ribbonDebuff, duration);
                        else
                        {
                            foreach (CharacterBody.TimedBuff buff in self.body.timedBuffs)
                            {
                                if (buff.buffIndex == Modules.Buffs.ribbonDebuff.buffIndex)
                                    buff.timer = duration;
                            }
                        }

                        if(ribbon)
                        {
                            ribbon.SyncRibbonTimersToNewTime(duration);
                        }
                        else
                        {
                            RibbonController newRibbon = self.gameObject.AddComponent<RibbonController>();
                            newRibbon.nextRoot = null;                           
                            newRibbon.inflictorRoot = damageInfo.attacker;
                            newRibbon.SyncRibbonTimersToNewTime(duration);
                        }


                        
                        
                    }

                }

            }

            




            orig(self, damageInfo);

        }
    }

}