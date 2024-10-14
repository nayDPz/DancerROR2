using BepInEx;
using Dancer.Modules;
using Dancer.Modules.Components;
using Dancer.SoftDependencies;
using HG;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Dancer
{
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [BepInDependency(R2API.DotAPI.PluginGUID)]
    [BepInDependency(R2API.LanguageAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    public class DancerPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.nayDPz.Dancer";

        public const string MODNAME = "Dancer";

        public const string MODVERSION = "0.10.0";

        public const string developerPrefix = "nayDPz";

        public static bool emotesInstalled;

        public static DancerPlugin instance;

        private void Awake()
        {
            instance = this;
            Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();
            SoundBanks.Init();
            States.RegisterStates();
            Buffs.RegisterBuffs();
            Projectiles.RegisterProjectiles();
            Tokens.AddTokens();
            ItemDisplays.PopulateDisplays();
            CameraParams.InitializeParams();
            Dancer.Modules.Survivors.Dancer.CreateCharacter();
            //Unlockables.RegisterUnlockables(); From what I understand it does nothing, none of the unlockabledefs are actually assigned to anything
            if (Modules.Config.artiBuddy.Value)
            {
                new LockedMageTracker();
            }
            new ContentPacks().Initialize();
            if (CustomEmotesAPICompat.enabled)
            {
                CustomEmotesAPICompat.SetupSkeleton();
            }
            ContentManager.onContentPacksAssigned += LateSetup;
            Hook();
        }

        private void LateSetup(ReadOnlyArray<ReadOnlyContentPack> obj)
        {
            Dancer.Modules.Survivors.Dancer.SetItemDisplays();
            if (Modules.Config.artiBuddy.Value)
            {
                Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/LockedMage.prefab").WaitForCompletion().GetComponent<GameObjectUnlockableFilter>()
                    .enabled = false;
            }
        }

        private void Hook()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig.Invoke(self);
            if (self.HasBuff(Buffs.ribbonDebuff))
            {
                self.moveSpeed *= 0.75f;
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            RibbonController ribbonController = RibbonController.FindRibbonController(self.gameObject);
            if (damageInfo != null && (bool)damageInfo.attacker && (bool)damageInfo.attacker.GetComponent<CharacterBody>())
            {
                if (damageInfo.attacker.GetComponent<CharacterBody>().baseNameToken == "NDP_DANCER_BODY_NAME")
                {
                    if ((bool)ribbonController && damageInfo.procChainMask.mask == 0 && damageInfo.damageType != DamageType.DoT && damageInfo.damageType != DamageType.ApplyMercExpose)
                    {
                        HealthComponent component = damageInfo.attacker.GetComponent<HealthComponent>();
                        if ((bool)component && damageInfo.damage > 0f)
                        {
                            float value = component.fullHealth * 0.045f * damageInfo.procCoefficient;
                            component.AddBarrier(value);
                        }
                        bool crit = damageInfo.crit;
                        float damageValue = 0f * damageInfo.attacker.GetComponent<CharacterBody>().baseDamage;
                        TeamIndex teamIndex = damageInfo.attacker.GetComponent<CharacterBody>().teamComponent.teamIndex;
                        if ((bool)ribbonController.NetworknextRoot)
                        {
                            CharacterBody component2 = ribbonController.nextRoot.GetComponent<CharacterBody>();
                            if ((bool)component2)
                            {
                                DancerOrb dancerOrb = new DancerOrb();
                                dancerOrb.attacker = damageInfo.attacker;
                                dancerOrb.bouncedObjects = null;
                                dancerOrb.bouncesRemaining = 0;
                                dancerOrb.damageCoefficientPerBounce = 1f;
                                dancerOrb.damageColorIndex = DamageColorIndex.Item;
                                dancerOrb.damageValue = damageValue;
                                dancerOrb.isCrit = crit;
                                dancerOrb.origin = damageInfo.position;
                                dancerOrb.procChainMask = default(ProcChainMask);
                                dancerOrb.procCoefficient = 0f;
                                dancerOrb.range = 0f;
                                dancerOrb.teamIndex = teamIndex;
                                dancerOrb.target = component2.mainHurtBox;
                                dancerOrb.duration = 0.25f;
                                OrbManager.instance.AddOrb(dancerOrb);
                            }
                        }
                        else
                        {
                            if (RibbonController.naturalSpread)
                            {
                                ribbonController.SpeedUpRibbon(0.25f);
                            }
                            else
                            {
                                ribbonController.SearchNewTarget();
                            }
                            ribbonController.inflictorRoot = damageInfo.attacker;
                        }
                    }
                    if (damageInfo.damageType == DamageType.FruitOnHit)
                    {
                        damageInfo.damageType = DamageType.Generic;
                        float ribbonDebuffDuration = Buffs.ribbonDebuffDuration;
                        if ((bool)ribbonController)
                        {
                            ribbonController.SyncRibbonTimersToNewTime(ribbonDebuffDuration);
                        }
                        else
                        {
                            GameObject gameObject = Object.Instantiate(Modules.Assets.ribbonController, self.gameObject.transform);
                            RibbonController component3 = gameObject.GetComponent<RibbonController>();
                            component3.timer = ribbonDebuffDuration;
                            component3.NetworkownerRoot = self.gameObject;
                            component3.inflictorRoot = damageInfo.attacker;
                            component3.spreadsRemaining = 2;
                            NetworkServer.Spawn(gameObject);
                            component3.StartRibbon();
                        }
                    }
                    if (damageInfo.damageType == DamageType.ApplyMercExpose)
                    {
                        damageInfo.damageType = DamageType.Stun1s;
                    }
                }
                if (self.body.baseNameToken == "NDP_DANCER_BODY_NAME" && self.body.HasBuff(Buffs.parryBuff))
                {
                    damageInfo.rejected = true;
                    self.body.RemoveBuff(Buffs.parryBuff);
                    self.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 1.5f);
                }
            }
            orig.Invoke(self, damageInfo);
            if ((bool)ribbonController && !self.alive && !ribbonController.ribbonAttached)
            {
                ribbonController.DetachFromOwner();
            }
        }
    }
}
