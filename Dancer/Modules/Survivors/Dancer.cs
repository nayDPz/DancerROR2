using BepInEx.Configuration;
using Dancer.SkillStates;
using Dancer.SkillStates.DirectionalM1;
using Dancer.SkillStates.M1;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace Dancer.Modules.Survivors
{

    public static class Dancer
    {
        public static SkillDef lockedSkillDef;

        public static SkillDef directionalSkillDef;

        public static SkillDef primarySkillDef;

        public static SkillDef lungeSkillDef;

        public static SkillDef secondarySkillDef;

        public static SkillDef spinLungeSkillDef;

        public static SkillDef chainRibbonSkillDef;

        internal static GameObject characterPrefab;

        internal static GameObject displayPrefab;

        internal static ConfigEntry<bool> characterEnabled;

        public const string bodyName = "NdpDancerBody";

        public static int bodyRendererIndex;

        internal static ItemDisplayRuleSet itemDisplayRuleSet;

        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        internal static void CreateCharacter()
        {
            characterPrefab = Prefabs.CreatePrefab("DancerBody", "mdlDancer", new BodyInfo
            {
                armor = 8f,
                armorGrowth = 0f,
                bodyName = "DancerBody",
                bodyNameToken = "NDP_DANCER_BODY_NAME",
                bodyColor = Color.magenta,
                characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texDancerIcon"),
                crosshair = Assets.crosshairPrefab,
                damage = 15f,
                healthGrowth = 48f,
                healthRegen = 2.5f,
                jumpCount = 1,
                jumpPower = 17f,
                acceleration = 80f,
                moveSpeed = 7f,
                maxHealth = 160f,
                subtitleNameToken = "NDP_DANCER_BODY_SUBTITLE",
                podPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/SurvivorPod.prefab").WaitForCompletion()
            });
            characterPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<FootstepHandler>().baseFootstepString = "Play_treeBot_step";
            characterPrefab.GetComponent<Interactor>().maxInteractionDistance = 5f;
            characterPrefab.GetComponent<CameraTargetParams>().cameraParams = CameraParams.defaultCameraParams;
            characterPrefab.GetComponent<SfxLocator>().landingSound = "DancerLand";
            characterPrefab.GetComponent<CharacterMotor>().mass = 200f;
            characterPrefab.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));
            characterPrefab.AddComponent<DancerComponent>();
            Material material = Assets.mainAssetBundle.LoadAsset<Material>("matDancer");
            bodyRendererIndex = 0;
            Prefabs.SetupCharacterModel(characterPrefab, new CustomRendererInfo[4]
            {
            new CustomRendererInfo
            {
                childName = "Body",
                material = material
            },
            new CustomRendererInfo
            {
                childName = "HeadMesh",
                material = material
            },
            new CustomRendererInfo
            {
                childName = "Lance",
                material = material
            },
            new CustomRendererInfo
            {
                childName = "Ribbons",
                material = material
            }
            }, bodyRendererIndex);
            displayPrefab = Prefabs.CreateDisplayPrefab("mdlDancer", characterPrefab);
            Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, Color.magenta, "DANCER");
            CreateHitboxes();
            CreateSkills();
            CreateSkins();
            InitializeItemDisplays();
            CreateDoppelganger();
        }

        private static void CreateDoppelganger()
        {
            Prefabs.CreateGenericDoppelganger(characterPrefab, "DancerMonsterMaster", "Merc");
        }

        private static void CreateHitboxes()
        {
            ChildLocator componentInChildren = characterPrefab.GetComponentInChildren<ChildLocator>();
            GameObject gameObject = componentInChildren.gameObject;
            Transform hitboxTransform = componentInChildren.FindChild("Jab");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "Jab");
            hitboxTransform = componentInChildren.FindChild("UpAir");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "UpAir");
            hitboxTransform = componentInChildren.FindChild("UpAir2");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "UpAir2");
            hitboxTransform = componentInChildren.FindChild("DownTilt");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "DownTilt");
            hitboxTransform = componentInChildren.FindChild("DAir");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "DownAir");
            hitboxTransform = componentInChildren.FindChild("DAirGround");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "DownAirGround");
            hitboxTransform = componentInChildren.FindChild("NAir");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "NAir");
            hitboxTransform = componentInChildren.FindChild("FAir");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "FAir");
            hitboxTransform = componentInChildren.FindChild("SpinLunge");
            Prefabs.SetupHitbox(gameObject, hitboxTransform, "SpinLunge");
        }

        private static void CreateSkills()
        {
            Skills.CreateSkillFamilies(characterPrefab);
            string text = "NDP";
            SkillDefInfo skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_PRIMARY_SLASH_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_PRIMARY_SLASH_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_PRIMARY_SLASH_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(M1Entry));
            skillDefInfo.activationStateMachineName = "Weapon";
            skillDefInfo.baseMaxStock = 1;
            skillDefInfo.baseRechargeInterval = 0f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = true;
            skillDefInfo.canceledFromSprinting = false;
            skillDefInfo.forceSprintDuringState = false;
            skillDefInfo.fullRestockOnAssign = true;
            skillDefInfo.interruptPriority = InterruptPriority.Any;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = true;
            skillDefInfo.mustKeyPress = false;
            skillDefInfo.cancelSprintingOnActivation = false;
            skillDefInfo.rechargeStock = 1;
            skillDefInfo.requiredStock = 1;
            skillDefInfo.stockToConsume = 1;
            skillDefInfo.keywordTokens = new string[7] { "KEYWORD_DANCER_CANCELS", "KEYWORD_DANCER_JAB", "KEYWORD_DANCER_DASH", "KEYWORD_DANCER_DOWNTILT", "KEYWORD_DANCER_UPAIR", "KEYWORD_DANCER_FORWARDAIR", "KEYWORD_DANCER_DOWNAIR" };
            primarySkillDef = Skills.CreateSkillDef(skillDefInfo);
            skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_PRIMARY_SLASH2_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_PRIMARY_SLASH2_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_PRIMARY_SLASH2_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(EnterDirectionalAttack));
            skillDefInfo.activationStateMachineName = "Weapon";
            skillDefInfo.baseMaxStock = 1;
            skillDefInfo.baseRechargeInterval = 0f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = true;
            skillDefInfo.canceledFromSprinting = false;
            skillDefInfo.forceSprintDuringState = false;
            skillDefInfo.fullRestockOnAssign = true;
            skillDefInfo.interruptPriority = InterruptPriority.Any;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = true;
            skillDefInfo.mustKeyPress = false;
            skillDefInfo.cancelSprintingOnActivation = true;
            skillDefInfo.rechargeStock = 1;
            skillDefInfo.requiredStock = 1;
            skillDefInfo.stockToConsume = 1;
            skillDefInfo.keywordTokens = new string[1] { "KEYWORD_DANCER_INPUTS" };
            directionalSkillDef = Skills.CreateSkillDef(skillDefInfo);
            Skills.AddPrimarySkill(characterPrefab, primarySkillDef);
            Skills.AddPrimarySkill(characterPrefab, directionalSkillDef);
            skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_SECONDARY_SLASH_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_SECONDARY_SLASH_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_SECONDARY_SLASH_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(SpinnyMove));
            skillDefInfo.activationStateMachineName = "Body";
            skillDefInfo.baseMaxStock = 1;
            skillDefInfo.baseRechargeInterval = 5f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = true;
            skillDefInfo.canceledFromSprinting = false;
            skillDefInfo.forceSprintDuringState = false;
            skillDefInfo.fullRestockOnAssign = true;
            skillDefInfo.interruptPriority = InterruptPriority.Skill;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = true;
            skillDefInfo.mustKeyPress = false;
            skillDefInfo.cancelSprintingOnActivation = true;
            skillDefInfo.rechargeStock = 1;
            skillDefInfo.requiredStock = 1;
            skillDefInfo.stockToConsume = 1;
            secondarySkillDef = Skills.CreateSkillDef(skillDefInfo);
            skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_SECONDARY_PARRY_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_SECONDARY_PARRY_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_SECONDARY_PARRY_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(ChargeParry));
            skillDefInfo.activationStateMachineName = "Body";
            skillDefInfo.baseMaxStock = 1;
            skillDefInfo.baseRechargeInterval = 5f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = true;
            skillDefInfo.canceledFromSprinting = true;
            skillDefInfo.forceSprintDuringState = false;
            skillDefInfo.fullRestockOnAssign = true;
            skillDefInfo.interruptPriority = InterruptPriority.PrioritySkill;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = true;
            skillDefInfo.mustKeyPress = true;
            skillDefInfo.cancelSprintingOnActivation = true;
            skillDefInfo.rechargeStock = 1;
            skillDefInfo.requiredStock = 1;
            skillDefInfo.stockToConsume = 1;
            SkillDef skillDef = Skills.CreateSkillDef(skillDefInfo);
            Skills.AddSecondarySkills(characterPrefab, secondarySkillDef);
            skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_UTILITY_PULL_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_UTILITY_PULL_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_UTILITY_PULL_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(DragonLunge));
            skillDefInfo.activationStateMachineName = "Body";
            skillDefInfo.baseMaxStock = 1;
            skillDefInfo.baseRechargeInterval = 6f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = true;
            skillDefInfo.canceledFromSprinting = false;
            skillDefInfo.forceSprintDuringState = false;
            skillDefInfo.fullRestockOnAssign = true;
            skillDefInfo.interruptPriority = InterruptPriority.PrioritySkill;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = true;
            skillDefInfo.mustKeyPress = false;
            skillDefInfo.cancelSprintingOnActivation = true;
            skillDefInfo.rechargeStock = 1;
            skillDefInfo.requiredStock = 1;
            skillDefInfo.stockToConsume = 1;
            lungeSkillDef = Skills.CreateSkillDef(skillDefInfo);
            skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_UTILITY_DRILL_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_UTILITY_DRILL_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_UTILITY_DRILL_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(SpinDash));
            skillDefInfo.activationStateMachineName = "Weapon";
            skillDefInfo.baseMaxStock = 2;
            skillDefInfo.baseRechargeInterval = 5.5f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = false;
            skillDefInfo.canceledFromSprinting = false;
            skillDefInfo.forceSprintDuringState = true;
            skillDefInfo.fullRestockOnAssign = true;
            skillDefInfo.interruptPriority = InterruptPriority.Skill;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = true;
            skillDefInfo.mustKeyPress = false;
            skillDefInfo.cancelSprintingOnActivation = true;
            skillDefInfo.rechargeStock = 1;
            skillDefInfo.requiredStock = 1;
            skillDefInfo.stockToConsume = 1;
            spinLungeSkillDef = Skills.CreateSkillDef(skillDefInfo);
            Skills.AddUtilitySkills(characterPrefab, lungeSkillDef, spinLungeSkillDef);
            skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_SPECIAL_RIBBON_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_SPECIAL_RIBBON_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_SPECIAL_RIBBON_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(FireChainRibbons));
            skillDefInfo.activationStateMachineName = "Weapon";
            skillDefInfo.baseMaxStock = 1;
            skillDefInfo.baseRechargeInterval = 13f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = true;
            skillDefInfo.canceledFromSprinting = false;
            skillDefInfo.forceSprintDuringState = false;
            skillDefInfo.fullRestockOnAssign = true;
            skillDefInfo.interruptPriority = InterruptPriority.PrioritySkill;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = true;
            skillDefInfo.mustKeyPress = true;
            skillDefInfo.cancelSprintingOnActivation = true;
            skillDefInfo.rechargeStock = 1;
            skillDefInfo.requiredStock = 1;
            skillDefInfo.stockToConsume = 1;
            skillDefInfo.keywordTokens = new string[0];
            chainRibbonSkillDef = Skills.CreateSkillDef(skillDefInfo);
            Skills.AddSpecialSkills(characterPrefab, chainRibbonSkillDef);
            skillDefInfo = new SkillDefInfo();
            skillDefInfo.skillName = text + "_DANCER_BODY_SPECIAL_RIBBON_LOCK_NAME";
            skillDefInfo.skillNameToken = text + "_DANCER_BODY_SPECIAL_RIBBON_LOCK_NAME";
            skillDefInfo.skillDescriptionToken = text + "_DANCER_BODY_SPECIAL_RIBBON_LOCK_DESCRIPTION";
            skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon");
            skillDefInfo.activationState = new SerializableEntityStateType(typeof(LockSkill));
            skillDefInfo.activationStateMachineName = "Weapon";
            skillDefInfo.baseMaxStock = 0;
            skillDefInfo.baseRechargeInterval = 0f;
            skillDefInfo.beginSkillCooldownOnSkillEnd = false;
            skillDefInfo.canceledFromSprinting = false;
            skillDefInfo.forceSprintDuringState = false;
            skillDefInfo.fullRestockOnAssign = false;
            skillDefInfo.interruptPriority = InterruptPriority.Any;
            skillDefInfo.resetCooldownTimerOnUse = false;
            skillDefInfo.isCombatSkill = false;
            skillDefInfo.mustKeyPress = false;
            skillDefInfo.cancelSprintingOnActivation = false;
            skillDefInfo.rechargeStock = 0;
            skillDefInfo.requiredStock = 999;
            skillDefInfo.stockToConsume = 0;
            lockedSkillDef = Skills.CreateSkillDef(skillDefInfo);
        }

        private static void CreateSkins()
        {
            GameObject gameObject = characterPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel component = gameObject.GetComponent<CharacterModel>();
            ModelSkinController modelSkinController = gameObject.AddComponent<ModelSkinController>();
            SkinnedMeshRenderer mainSkinnedMeshRenderer = component.mainSkinnedMeshRenderer;
            CharacterModel.RendererInfo[] baseRendererInfos = component.baseRendererInfos;
            List<SkinDef> list = new List<SkinDef>();
            SkinDef skinDef = Skins.CreateSkinDef("NDP_DANCER_BODY_DEFAULT_SKIN_NAME", Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"), baseRendererInfos, mainSkinnedMeshRenderer, gameObject);
            skinDef.meshReplacements = new SkinDef.MeshReplacement[4]
            {
            new SkinDef.MeshReplacement
            {
                mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshDancerBody"),
                renderer = baseRendererInfos[0].renderer
            },
            new SkinDef.MeshReplacement
            {
                mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshDancerHead"),
                renderer = baseRendererInfos[1].renderer
            },
            new SkinDef.MeshReplacement
            {
                mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshDancerWeapon"),
                renderer = baseRendererInfos[2].renderer
            },
            new SkinDef.MeshReplacement
            {
                mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshDancerRibbons"),
                renderer = baseRendererInfos[3].renderer
            }
            };
            list.Add(skinDef);
            Material defaultMaterial = Assets.mainAssetBundle.LoadAsset<Material>("matDancerGold");
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[4]
            {
            new CharacterModel.RendererInfo
            {
                defaultMaterial = defaultMaterial,
                defaultShadowCastingMode = ShadowCastingMode.On,
                ignoreOverlays = false,
                renderer = baseRendererInfos[0].renderer
            },
            new CharacterModel.RendererInfo
            {
                defaultMaterial = defaultMaterial,
                defaultShadowCastingMode = ShadowCastingMode.On,
                ignoreOverlays = false,
                renderer = baseRendererInfos[1].renderer
            },
            new CharacterModel.RendererInfo
            {
                defaultMaterial = defaultMaterial,
                defaultShadowCastingMode = ShadowCastingMode.On,
                ignoreOverlays = false,
                renderer = baseRendererInfos[2].renderer
            },
            new CharacterModel.RendererInfo
            {
                defaultMaterial = null,
                defaultShadowCastingMode = ShadowCastingMode.Off,
                ignoreOverlays = true,
                renderer = baseRendererInfos[3].renderer
            }
            };
            SkinDef.MeshReplacement[] meshReplacements = new SkinDef.MeshReplacement[4]
            {
            new SkinDef.MeshReplacement
            {
                mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshGoldBody"),
                renderer = baseRendererInfos[0].renderer
            },
            new SkinDef.MeshReplacement
            {
                mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshGoldHead"),
                renderer = baseRendererInfos[1].renderer
            },
            new SkinDef.MeshReplacement
            {
                mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshGoldWeapon"),
                renderer = baseRendererInfos[2].renderer
            },
            new SkinDef.MeshReplacement
            {
                mesh = null,
                renderer = baseRendererInfos[3].renderer
            }
            };
            On.RoR2.SkinDef.Awake += DoNothing;
            SkinDef skinDef2 = ScriptableObject.CreateInstance<SkinDef>();
            skinDef2.baseSkins = Array.Empty<SkinDef>();
            skinDef2.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldSkin");
            skinDef2.unlockableDef = null;
            skinDef2.rootObject = gameObject;
            skinDef2.rendererInfos = rendererInfos;
            skinDef2.gameObjectActivations = new SkinDef.GameObjectActivation[0];
            skinDef2.meshReplacements = meshReplacements;
            skinDef2.projectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            skinDef2.minionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            skinDef2.nameToken = "NDP_DANCER_GOLD_SKIN_NAME";
            skinDef2.name = "NDP_DANCER_GOLD_SKIN_NAME";
            On.RoR2.SkinDef.Awake -= DoNothing;
            LanguageAPI.Add("NDP_DANCER_GOLD_SKIN_NAME", "Gold");
            list.Add(skinDef2);
            modelSkinController.skins = list.ToArray();
        }

        private static void DoNothing(On.RoR2.SkinDef.orig_Awake orig, RoR2.SkinDef self)
        {
        }

        private static void InitializeItemDisplays()
        {
            CharacterModel componentInChildren = characterPrefab.GetComponentInChildren<CharacterModel>();
            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemDisplayRuleSet.name = "idrsDancer";
            componentInChildren.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        internal static void SetItemDisplays()
        {
            itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Jetpack,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
                        childName = "Chest",
                        localPos = new Vector3(0.00477f, 0.10516f, -0.07228f),
                        localAngles = new Vector3(341.0195f, 0f, 0f),
                        localScale = new Vector3(0.35593f, 0.15914f, 0.12779f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GoldGat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
                        childName = "Chest",
                        localPos = new Vector3(0.19943f, 0.36153f, 0.04903f),
                        localAngles = new Vector3(58.84191f, 250.4249f, 142.6867f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BFG,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
                        childName = "Chest",
                        localPos = new Vector3(0.10222f, 0.20518f, 0.05978f),
                        localAngles = new Vector3(0f, 0f, 327.4607f),
                        localScale = new Vector3(0.3f, 0.3f, 0.3f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CritGlasses,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.12999f, 0.16031f),
                        localAngles = new Vector3(9.15538f, 0f, 0f),
                        localScale = new Vector3(0.3215f, 0.3034f, 0.3034f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Syringe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.00565f, 0.18895f, -0.04959f),
                        localAngles = new Vector3(352.4485f, 266.223f, 79.63069f),
                        localScale = new Vector3(0.15f, 0.15f, 0.15f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Behemoth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
                        childName = "Chest",
                        localPos = new Vector3(0f, 0.2158f, -0.19895f),
                        localAngles = new Vector3(6.223f, 180f, 330.578f),
                        localScale = new Vector3(0.05801f, 0.07173f, 0.05726f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Missile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
                        childName = "Chest",
                        localPos = new Vector3(-0.42134f, 0.52133f, 0.02885f),
                        localAngles = new Vector3(2.35642f, 3.01076f, 51.98444f),
                        localScale = new Vector3(0.1f, 0.12268f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Dagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
                        childName = "Chest",
                        localPos = new Vector3(0.27055f, 0.31367f, -0.03923f),
                        localAngles = new Vector3(305.3635f, 11.31594f, 355.1549f),
                        localScale = new Vector3(0.7f, 0.7f, 0.7f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
                        childName = "Chest",
                        localPos = new Vector3(-0.26105f, 0.30321f, -0.044f),
                        localAngles = new Vector3(56.48328f, 175.5279f, 179.5179f),
                        localScale = new Vector3(-0.7f, -0.7f, -0.7f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Hoof,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
                        childName = "CalfL",
                        localPos = new Vector3(-0.01997f, 0.78193f, 0.05326f),
                        localAngles = new Vector3(87.04044f, 76.51312f, 243.9886f),
                        localScale = new Vector3(0.13069f, 0.1303f, 0.13955f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ChainLightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
                        childName = "Chest",
                        localPos = new Vector3(-0.0011f, 0.1031f, -0.16005f),
                        localAngles = new Vector3(9.29676f, 174.7346f, 24.99983f),
                        localScale = new Vector3(0.6f, 0.6f, 0.6f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GhostOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
                        childName = "Head",
                        localPos = new Vector3(0.00301f, 0.12038f, 0.056f),
                        localAngles = new Vector3(7.80833f, 0f, 0f),
                        localScale = new Vector3(0.73147f, 0.6313f, 1.33377f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Mushroom,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.00688f, 0.15733f, -0.04649f),
                        localAngles = new Vector3(359.4526f, 271.4876f, 89.98575f),
                        localScale = new Vector3(0.07f, 0.071f, 0.0701f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.14479f, 0.09794f),
                        localAngles = new Vector3(13.53955f, 0f, 0f),
                        localScale = new Vector3(0.5666f, 0.5666f, 0.5666f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
                        childName = "Pelvis",
                        localPos = new Vector3(0.19175f, 0.34523f, 0.0177f),
                        localAngles = new Vector3(36.59357f, 177.5122f, 178.7256f),
                        localScale = new Vector3(0.5f, 0.5f, 0.68416f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WardOnLevel,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWarbanner"),
                        childName = "Chest",
                        localPos = new Vector3(0.0168f, 0.26039f, -0.21587f),
                        localAngles = new Vector3(0f, 0f, 90f),
                        localScale = new Vector3(0.3162f, 0.3162f, 0.3162f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayScythe"),
                        childName = "Chest",
                        localPos = new Vector3(-0.08898f, 0.08614f, -0.19459f),
                        localAngles = new Vector3(293.1983f, 281.6273f, 259.683f),
                        localScale = new Vector3(0.30835f, 0.1884f, 0.32014f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealWhileSafe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
                        childName = "Chest",
                        localPos = new Vector3(-0.11196f, 0.21602f, 0.01362f),
                        localAngles = new Vector3(24.59701f, 9.63536f, 35.11288f),
                        localScale = new Vector3(0.07f, 0.07f, 0.07f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Clover,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
                        childName = "Head",
                        localPos = new Vector3(0.0039f, 0.04673f, 0.2657f),
                        localAngles = new Vector3(85.61921f, 0.0001f, 179.4897f),
                        localScale = new Vector3(0.35f, 0.35f, 0.35f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnOverHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.08954f, 0.0619f, 0.01725f),
                        localAngles = new Vector3(81.85985f, 259.0763f, 155.0281f),
                        localScale = new Vector3(0.2f, 0.2f, 0.2f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GoldOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.15792f, -0.02926f),
                        localAngles = new Vector3(13.43895f, 311.3283f, 343.426f),
                        localScale = new Vector3(1.1754f, 1.1754f, 0.92166f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WarCryOnMultiKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.07401f, 0.15409f, -0.07474f),
                        localAngles = new Vector3(65.18475f, 214.0179f, 355.7895f),
                        localScale = new Vector3(0.7094f, 0.7094f, 0.7094f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
                        childName = "LowerArmR",
                        localPos = new Vector3(0.00659f, 0.25923f, 0.00061f),
                        localAngles = new Vector3(357.2321f, 117.9287f, 90.60644f),
                        localScale = new Vector3(0.25f, 0.25f, 0.25f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IceRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
                        childName = "Weapon",
                        localPos = new Vector3(0.00685f, 0.08912f, 0f),
                        localAngles = new Vector3(274.3965f, 90f, 270f),
                        localScale = new Vector3(0.5f, 0.5f, 1.0828f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
                        childName = "Weapon",
                        localPos = new Vector3(0.0028f, 0.24892f, 1E-05f),
                        localAngles = new Vector3(274.3965f, 90f, 270f),
                        localScale = new Vector3(0.5f, 0.5f, 1.12845f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                        childName = "Chest",
                        localPos = new Vector3(-0.34596f, 0.28343f, -0.00666f),
                        localAngles = new Vector3(313.2421f, 95.99242f, 180f),
                        localScale = new Vector3(0.6f, 0.6f, 0.6f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                        childName = "Chest",
                        localPos = new Vector3(0.34932f, 0.30434f, -0.00161f),
                        localAngles = new Vector3(49.85487f, 108.7415f, 199.3213f),
                        localScale = new Vector3(0.6f, 0.6f, 0.6f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.JumpBoost,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
                        childName = "Head",
                        localPos = new Vector3(0f, -0.20925f, -0.41214f),
                        localAngles = new Vector3(34.52309f, 0f, 0f),
                        localScale = new Vector3(1.09546f, 0.99388f, 1.40369f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorReductionOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
                        childName = "Chest",
                        localPos = new Vector3(0.0513f, 0.0652f, -0.0792f),
                        localAngles = new Vector3(64.189f, 90f, 90f),
                        localScale = new Vector3(0.1722f, 0.1722f, 0.1722f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NearbyDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
                        childName = "Weapon",
                        localPos = new Vector3(-0.002f, -0.03488f, 0f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.1236f, 0.1236f, 0.1236f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorPlate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                        childName = "LowerArmL",
                        localPos = new Vector3(0.06063f, 0.19185f, -0.01304f),
                        localAngles = new Vector3(76.57407f, 81.50978f, 180f),
                        localScale = new Vector3(0.3f, 0.3f, 0.3f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CommandMissile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileRack"),
                        childName = "Chest",
                        localPos = new Vector3(-0.23202f, 0.28282f, 0.01393f),
                        localAngles = new Vector3(342.4002f, 168.7708f, 34.96157f),
                        localScale = new Vector3(0.3362f, 0.3362f, 0.3362f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Feather,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFeather"),
                        childName = "Head",
                        localPos = new Vector3(-0.01522f, 0.13423f, -0.05715f),
                        localAngles = new Vector3(314.8987f, 357.7154f, 11.71727f),
                        localScale = new Vector3(0.03962f, 0.03327f, 0.0285f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Crowbar,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
                        childName = "Pelvis",
                        localPos = new Vector3(-0.1691f, 0.24688f, -0.14963f),
                        localAngles = new Vector3(43.55907f, 11.81575f, 11.82357f),
                        localScale = new Vector3(0.3f, 0.3f, 0.3f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                        childName = "CalfL",
                        localPos = new Vector3(-0.0038f, 0.37291f, -0.01597f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.25f, 0.25f, 0.30579f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                        childName = "CalfR",
                        localPos = new Vector3(-0.0038f, 0.37291f, -0.01597f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.25f, 0.25f, 0.30579f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
                        childName = "ThighR",
                        localPos = new Vector3(-0.15427f, 0.16356f, 0.0345f),
                        localAngles = new Vector3(80.68687f, 279.3683f, 175.5874f),
                        localScale = new Vector3(0.1843f, 0.1843f, 0.1843f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EquipmentMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBattery"),
                        childName = "Weapon",
                        localPos = new Vector3(0.00188f, 0.07925f, -0.00277f),
                        localAngles = new Vector3(272.4369f, 145.8502f, 32.1286f),
                        localScale = new Vector3(0.2f, 0.2f, 0.23894f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                        childName = "Head",
                        localPos = new Vector3(-0.0949f, 0.0945f, 0.0654f),
                        localAngles = new Vector3(6.82254f, 22.89301f, 0f),
                        localScale = new Vector3(-0.5349f, 0.5349f, 0.5349f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                        childName = "Head",
                        localPos = new Vector3(0.0949f, 0.0945f, 0.0654f),
                        localAngles = new Vector3(6.82253f, 337.107f, 0f),
                        localScale = new Vector3(0.5349f, 0.5349f, 0.5349f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Infusion,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayInfusion"),
                        childName = "Chest",
                        localPos = new Vector3(-0.17919f, -0.00327f, 0.06826f),
                        localAngles = new Vector3(20.46128f, 304.1627f, 0f),
                        localScale = new Vector3(0.5253f, 0.5253f, 0.5253f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Medkit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
                        childName = "Pelvis",
                        localPos = new Vector3(0.11193f, 0.30977f, 0.0997f),
                        localAngles = new Vector3(280.1438f, 349.5922f, 38.17199f),
                        localScale = new Vector3(0.6f, 0.6f, 0.6f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bandolier,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
                        childName = "Pelvis",
                        localPos = new Vector3(0.01815f, 0.35131f, -0.02141f),
                        localAngles = new Vector3(270f, 0f, 0f),
                        localScale = new Vector3(0.42224f, 0.59252f, 0.242f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BounceNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
                        childName = "Chest",
                        localPos = new Vector3(0.167f, 0.23155f, -0.0053f),
                        localAngles = new Vector3(358.6279f, 35.35772f, 283.0336f),
                        localScale = new Vector3(0.3f, 0.3f, 0.3f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IgniteOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGasoline"),
                        childName = "ThighL",
                        localPos = new Vector3(0.1586f, 0.09434f, 0.04533f),
                        localAngles = new Vector3(78.44348f, 270f, 270f),
                        localScale = new Vector3(0.65f, 0.65f, 0.65f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StunChanceOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayStunGrenade"),
                        childName = "Chest",
                        localPos = new Vector3(-0.18348f, -0.04668f, 0.05757f),
                        localAngles = new Vector3(8.86498f, 4.53647f, 11.30128f),
                        localScale = new Vector3(0.8f, 0.8f, 0.8f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Firework,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFirework"),
                        childName = "HandR",
                        localPos = new Vector3(0.03615f, 0.09452f, 0.09992f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.1194f, 0.1194f, 0.17403f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarDagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayLunarDagger"),
                        childName = "HandR",
                        localPos = new Vector3(0.01621f, 0.05569f, 0.01211f),
                        localAngles = new Vector3(65.45142f, 346.3312f, 235.2556f),
                        localScale = new Vector3(0.3385f, 1.01916f, 0.3385f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Knurl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayKnurl"),
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.02841f, 0.14421f, -0.01509f),
                        localAngles = new Vector3(78.87074f, 36.6722f, 105.8275f),
                        localScale = new Vector3(0.0848f, 0.1006f, 0.10147f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BeetleGland,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
                        childName = "Pelvis",
                        localPos = new Vector3(-0.25656f, 0.29023f, -0.0332f),
                        localAngles = new Vector3(349.7159f, 177.1544f, 2.7042f),
                        localScale = new Vector3(0.09594f, 0.08604f, 0.09076f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySoda"),
                        childName = "Pelvis",
                        localPos = new Vector3(-0.19025f, 0.25343f, -0.03256f),
                        localAngles = new Vector3(270f, 251.0168f, 0f),
                        localScale = new Vector3(0.2f, 0.2f, 0.2f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SecondarySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                        childName = "LowerArmL",
                        localPos = new Vector3(-0.06727f, 0.18375f, 0.03843f),
                        localAngles = new Vector3(335.0898f, 25.122f, 176.703f),
                        localScale = new Vector3(0.07f, 0.07f, 0.07f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StickyBomb,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayStickyBomb"),
                        childName = "HandR",
                        localPos = new Vector3(0.01519f, 0.0686f, -0.08218f),
                        localAngles = new Vector3(78.27731f, 2E-05f, 171.4936f),
                        localScale = new Vector3(0.2f, 0.2f, 0.2f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TreasureCache,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayKey"),
                        childName = "CalfR",
                        localPos = new Vector3(-0.06349f, -0.13659f, -0.04507f),
                        localAngles = new Vector3(345.2247f, 238.6147f, 228.6344f),
                        localScale = new Vector3(1f, 1f, 1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BossDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayAPRound"),
                        childName = "Pelvis",
                        localPos = new Vector3(0.21936f, 0.36784f, -0.00669f),
                        localAngles = new Vector3(78.75988f, 151.3122f, 84.95592f),
                        localScale = new Vector3(0.7f, 0.7f, 0.46965f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SlowOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBauble"),
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.70539f, 0.12032f, 0.01842f),
                        localAngles = new Vector3(70.31753f, 62.26086f, 287.2839f),
                        localScale = new Vector3(0.6f, 0.6f, 0.6f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExtraLife,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayHippo"),
                        childName = "Chest",
                        localPos = new Vector3(0.00655f, 0.11081f, 0.21627f),
                        localAngles = new Vector3(11.20005f, 351.6793f, 344.3522f),
                        localScale = new Vector3(0.32f, 0.32f, 0.32f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.KillEliteFrenzy,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                        childName = "Head",
                        localPos = new Vector3(0.04038f, 0.12258f, 0.03265f),
                        localAngles = new Vector3(39.83437f, 0f, 0f),
                        localScale = new Vector3(0.38565f, 0.2638f, 0.41101f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RepeatHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayCorpseFlower"),
                        childName = "UpperArmL",
                        localPos = new Vector3(0.15459f, 0.1895f, -0.15276f),
                        localAngles = new Vector3(76.12807f, 336.4276f, 180f),
                        localScale = new Vector3(0.2f, 0.2f, 0.2f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AutoCastEquipment,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFossil"),
                        childName = "Pelvis",
                        localPos = new Vector3(-0.00514f, 0.28757f, 0.1698f),
                        localAngles = new Vector3(0f, 89.42922f, 0f),
                        localScale = new Vector3(0.4208f, 0.4208f, 0.4208f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IncreaseHealing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                        childName = "Head",
                        localPos = new Vector3(0.07414f, 0.19751f, -0.0258f),
                        localAngles = new Vector3(344.5861f, 132.518f, 356.9124f),
                        localScale = new Vector3(0.3395f, 0.3395f, 0.3395f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                        childName = "Head",
                        localPos = new Vector3(-0.07414f, 0.19751f, -0.0258f),
                        localAngles = new Vector3(15.41393f, 42f, 356.9124f),
                        localScale = new Vector3(0.3395f, 0.3395f, -0.3395f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TitanGoldDuringTP,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldHeart"),
                        childName = "Chest",
                        localPos = new Vector3(-0.10856f, 0.05727f, 0.17572f),
                        localAngles = new Vector3(340.0459f, 326.5524f, 1.80304f),
                        localScale = new Vector3(0.2f, 0.2f, 0.2f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintWisp,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
                        childName = "Chest",
                        localPos = new Vector3(0.13391f, 0.05807f, 0.15326f),
                        localAngles = new Vector3(20.49514f, 15.08605f, 357.7958f),
                        localScale = new Vector3(0.1385f, 0.1385f, 0.1385f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBrooch"),
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.0614f, 0.18859f, 0.02649f),
                        localAngles = new Vector3(81.03616f, 314.4032f, 15.99828f),
                        localScale = new Vector3(0.6f, 0.6f, 0.6f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TPHealingNova,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGlowFlower"),
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.15287f, 0.19369f, -0.13507f),
                        localAngles = new Vector3(0f, 222.6754f, 0f),
                        localScale = new Vector3(0.35f, 0.35f, 0.35f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarUtilityReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdFoot"),
                        childName = "CalfR",
                        localPos = new Vector3(0f, 0.76226f, 0.16685f),
                        localAngles = new Vector3(0f, 270f, 272.4676f),
                        localScale = new Vector3(1f, 1.4565f, 1.2945f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
                        childName = "UpperArmL",
                        localPos = new Vector3(0f, 0f, 0f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.4814f, 0.4814f, 0.4814f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
                        childName = "Head",
                        localPos = new Vector3(-0.07555f, 0.12033f, 0.15897f),
                        localAngles = new Vector3(301.2076f, 146.43f, 189.6029f),
                        localScale = new Vector3(0.25567f, 0.23304f, 0.17833f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayJellyGuts"),
                        childName = "Chest",
                        localPos = new Vector3(0.01407f, 0.31906f, 0.0984f),
                        localAngles = new Vector3(47.005f, 278.725f, 140.888f),
                        localScale = new Vector3(0.15f, 0.15f, 0.15f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarTrinket,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBeads"),
                        childName = "UpperArmL",
                        localPos = new Vector3(-0.00329f, 0.16805f, 0.05026f),
                        localAngles = new Vector3(0f, 0f, 99.29353f),
                        localScale = new Vector3(1.13087f, 1.19312f, 1.08218f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Plant,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayInterstellarDeskPlant"),
                        childName = "Chest",
                        localPos = new Vector3(0.17918f, 0.26891f, 0.01079f),
                        localAngles = new Vector3(299.256f, 26.12627f, 249.1656f),
                        localScale = new Vector3(0.07f, 0.07f, 0.07f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bear,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
                        childName = "Chest",
                        localPos = new Vector3(0.01423f, 0.21404f, -0.20479f),
                        localAngles = new Vector3(359.4737f, 180.8153f, 2.00493f),
                        localScale = new Vector3(0.3f, 0.3f, 0.3f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.DeathMark,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.00634f, 0.14811f, -0.02871f),
                        localAngles = new Vector3(277.5254f, 0f, 346.0966f),
                        localScale = new Vector3(-0.05f, -0.05f, -0.06f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExplodeOnDeath,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.00096f, 0.41647f, 0.00463f),
                        localAngles = new Vector3(2.77335f, 359.1895f, 176.0624f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Seed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
                        childName = "Chest",
                        localPos = new Vector3(0.05344f, -0.04829f, 0.15088f),
                        localAngles = new Vector3(306.1523f, 261.2392f, 242.3862f),
                        localScale = new Vector3(0.03394f, 0.03326f, 0.03041f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintOutOfCombat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
                        childName = "Pelvis",
                        localPos = new Vector3(0.23977f, 0.22895f, -0.00201f),
                        localAngles = new Vector3(1.87579f, 354.8801f, 24.10377f),
                        localScale = new Vector3(0.4f, 0.4f, 0.4f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Phasing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayStealthkit"),
                        childName = "ThighR",
                        localPos = new Vector3(-0.08927f, 0.20322f, 0.12008f),
                        localAngles = new Vector3(90f, 139.0103f, 0f),
                        localScale = new Vector3(0.37577f, 0.56422f, 0.35f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.PersonalShield,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldGenerator"),
                        childName = "Chest",
                        localPos = new Vector3(0f, 0.2649f, 0.0689f),
                        localAngles = new Vector3(304.1204f, 90f, 270f),
                        localScale = new Vector3(0.1057f, 0.1057f, 0.1057f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShockNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.10735f, -0.0308f),
                        localAngles = new Vector3(295.9102f, 0f, 0f),
                        localScale = new Vector3(0.3229f, 0.3229f, 0.3229f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShieldOnly,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                        childName = "Head",
                        localPos = new Vector3(-0.0868f, 0.26797f, 0f),
                        localAngles = new Vector3(11.8181f, 268.0985f, 359.6104f),
                        localScale = new Vector3(0.3521f, 0.3521f, -0.3521f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                        childName = "Head",
                        localPos = new Vector3(0.07771f, 0.26797f, 0f),
                        localAngles = new Vector3(348.1819f, 268.0985f, 0.3896f),
                        localScale = new Vector3(0.3521f, 0.3521f, 0.3521f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AlienHead,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
                        childName = "Weapon",
                        localPos = new Vector3(0.02859f, -0.04522f, 0.01442f),
                        localAngles = new Vector3(284.1172f, 243.7966f, 260.89f),
                        localScale = new Vector3(0.8f, 0.8f, 0.8f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HeadHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySkullCrown"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.2442f, 0.03993f),
                        localAngles = new Vector3(15.9298f, 0f, 0f),
                        localScale = new Vector3(0.4851f, 0.1617f, 0.1617f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWarHorn"),
                        childName = "Head",
                        localPos = new Vector3(-0.06565f, 0.01069f, 0.33667f),
                        localAngles = new Vector3(321.7959f, 253.9446f, 1.83296f),
                        localScale = new Vector3(0.2732f, 0.2732f, 0.2732f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySteakCurved"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.18682f, 0.13945f),
                        localAngles = new Vector3(285.4012f, 2E-05f, -1E-05f),
                        localScale = new Vector3(0.1245f, 0.1155f, 0.1155f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Tooth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
                        childName = "Head",
                        localPos = new Vector3(0.04568f, -0.00035f, 0.19319f),
                        localAngles = new Vector3(25.05384f, 17.60428f, 7.65284f),
                        localScale = new Vector3(1.5f, 1.5f, 1.5f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
                        childName = "Head",
                        localPos = new Vector3(-0.02361f, -0.003f, 0.19844f),
                        localAngles = new Vector3(25.68274f, 348.6905f, 355.0463f),
                        localScale = new Vector3(1.5f, 1.5f, 1.5f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Pearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayPearl"),
                        childName = "LowerArmR",
                        localPos = new Vector3(0f, 0f, 0f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShinyPearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
                        childName = "Chest",
                        localPos = new Vector3(0f, 0f, 0f),
                        localAngles = new Vector3(328.163f, 284.6944f, 27.83253f),
                        localScale = new Vector3(0.3f, 0.3f, 0.3f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayTome"),
                        childName = "ThighR",
                        localPos = new Vector3(-0.13437f, 0.02671f, 0.00331f),
                        localAngles = new Vector3(4.74548f, 269.9583f, 359.6995f),
                        localScale = new Vector3(0.08f, 0.08f, 0.08f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Squid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySquidTurret"),
                        childName = "Chest",
                        localPos = new Vector3(-0.0164f, 0.23145f, 0.0095f),
                        localAngles = new Vector3(0f, 90f, 7.10089f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Icicle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
                        childName = "Base",
                        localPos = new Vector3(-0.658f, -1.0806f, 0.015f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(1f, 1f, 1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Talisman,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
                        childName = "Base",
                        localPos = new Vector3(0.8357f, -0.7042f, -0.2979f),
                        localAngles = new Vector3(270f, 0f, 0f),
                        localScale = new Vector3(1f, 1f, 1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LaserTurbine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.27358f, 0.17357f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.25f, 0.25f, 0.25f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FocusConvergence,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
                        childName = "Base",
                        localPos = new Vector3(-0.0554f, -1.6605f, -0.3314f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireballsOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
                        childName = "HandR",
                        localPos = new Vector3(0.01431f, 0.07931f, 0.00529f),
                        localAngles = new Vector3(344.0956f, 176.9312f, 2.92877f),
                        localScale = new Vector3(0.0697f, 0.07056f, 0.074f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SiphonOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySiphonOnLowHealth"),
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.00745f, 0.25453f, 0.03687f),
                        localAngles = new Vector3(354.4601f, 0f, 0f),
                        localScale = new Vector3(0.1f, 0.16051f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBleedOnHitAndExplode"),
                        childName = "ThighR",
                        localPos = new Vector3(-0.11983f, -0.00669f, 0.03566f),
                        localAngles = new Vector3(0f, 0f, 105.2972f),
                        localScale = new Vector3(0.08f, 0.08f, 0.08f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.MonstersOnShrineUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayMonstersOnShrineUse"),
                        childName = "Chest",
                        localPos = new Vector3(-0.08897f, 0.00789f, 0.20548f),
                        localAngles = new Vector3(55.74505f, 264.118f, 355.134f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RandomDamageZone,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
                        childName = "Chest",
                        localPos = new Vector3(-1E-05f, 0.17219f, -0.30708f),
                        localAngles = new Vector3(34.84705f, 0f, 0f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Fruit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
                        childName = "Weapon",
                        localPos = new Vector3(-0.08626f, -0.15387f, 0.21263f),
                        localAngles = new Vector3(16.74619f, 94.02277f, 334.7391f),
                        localScale = new Vector3(0.2118f, 0.2118f, 0.2118f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixRed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                        childName = "Head",
                        localPos = new Vector3(0.09221f, 0.18718f, -0.03466f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.1036f, 0.1036f, 0.1036f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                        childName = "Head",
                        localPos = new Vector3(-0.09221f, 0.18718f, -0.03466f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(-0.1036f, 0.1036f, 0.1036f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixBlue,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[2]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.09709f, 0.19904f),
                        localAngles = new Vector3(315f, 0f, 0f),
                        localScale = new Vector3(0.2f, 0.2f, 0.2f),
                        limbMask = LimbFlags.None
                    },
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.14871f, 0.15627f),
                        localAngles = new Vector3(300f, 0f, 0f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixWhite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.28471f, 0.01687f),
                        localAngles = new Vector3(282.0894f, 0f, 0f),
                        localScale = new Vector3(0.0265f, 0.0265f, 0.0265f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixPoison,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.19891f, 0.0624f),
                        localAngles = new Vector3(298.819f, 0f, 0f),
                        localScale = new Vector3(0.0496f, 0.0496f, 0.0496f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixHaunted,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.26174f, 0.00841f),
                        localAngles = new Vector3(280.0554f, 0f, 0f),
                        localScale = new Vector3(0.065f, 0.065f, 0.065f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CritOnUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
                        childName = "Head",
                        localPos = new Vector3(0f, 0.05455f, 0.41408f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.2326f, 0.2326f, 0.2326f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DroneBackup,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayRadio"),
                        childName = "ThighL",
                        localPos = new Vector3(0.12572f, -0.04588f, -0.01867f),
                        localAngles = new Vector3(11.69349f, 91.28197f, 186.3007f),
                        localScale = new Vector3(0.7f, 0.7f, 0.7f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Lightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayLightningArmRight"),
                        childName = "UpperArmR",
                        localPos = new Vector3(0f, 0f, 0f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.3413f, 0.3413f, 0.3413f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BurnNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayPotion"),
                        childName = "HandR",
                        localPos = new Vector3(0.00713f, 0.10868f, -0.11556f),
                        localAngles = new Vector3(63.45895f, 264.0619f, 232.0915f),
                        localScale = new Vector3(0.05f, 0.05f, 0.05f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CrippleWard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEffigy"),
                        childName = "Pelvis",
                        localPos = new Vector3(0.12859f, 0.21196f, 0.11489f),
                        localAngles = new Vector3(354.6827f, 203.9411f, 339.1208f),
                        localScale = new Vector3(0.4f, 0.4f, 0.4f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBatteryArray"),
                        childName = "Chest",
                        localPos = new Vector3(0f, 0.19107f, -0.15272f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.2188f, 0.2188f, 0.2188f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GainArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayElephantFigure"),
                        childName = "Chest",
                        localPos = new Vector3(-0.17946f, 0.30134f, -0.02533f),
                        localAngles = new Vector3(7.57041f, 0f, 26.79241f),
                        localScale = new Vector3(0.6279f, 0.6279f, 0.6279f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Recycle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayRecycler"),
                        childName = "Chest",
                        localPos = new Vector3(0f, 0.18308f, -0.14788f),
                        localAngles = new Vector3(0f, 90f, 343.8301f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.FireBallDash,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayEgg"),
                        childName = "Pelvis",
                        localPos = new Vector3(0.21155f, 0.33451f, 0.00231f),
                        localAngles = new Vector3(284.2241f, 279.7924f, 78.87299f),
                        localScale = new Vector3(0.25f, 0.25f, 0.25f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Cleanse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWaterPack"),
                        childName = "Chest",
                        localPos = new Vector3(0f, 0.02586f, -0.13308f),
                        localAngles = new Vector3(16.98321f, 180f, 0f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Tonic,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayTonic"),
                        childName = "HandR",
                        localPos = new Vector3(0.01919f, 0.09174f, -0.08501f),
                        localAngles = new Vector3(347.4816f, 264.4327f, 270.9305f),
                        localScale = new Vector3(0.25f, 0.25f, 0.25f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Gateway,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayVase"),
                        childName = "Pelvis",
                        localPos = new Vector3(-0.21348f, 0.3313f, 0f),
                        localAngles = new Vector3(12.8791f, 90f, 0f),
                        localScale = new Vector3(0.2f, 0.2f, 0.11834f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Meteor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
                        childName = "Base",
                        localPos = new Vector3(0f, -1.7606f, -0.9431f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(1f, 1f, 1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Saw,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplaySawmerang"),
                        childName = "Base",
                        localPos = new Vector3(0f, -1.7606f, -0.9431f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Blackhole,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
                        childName = "Base",
                        localPos = new Vector3(0f, -1.7606f, -0.9431f),
                        localAngles = new Vector3(0f, 0f, 0f),
                        localScale = new Vector3(0.5f, 0.5f, 0.5f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Scanner,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayScanner"),
                        childName = "Chest",
                        localPos = new Vector3(-0.2131f, 0.25091f, -0.03601f),
                        localAngles = new Vector3(298.4561f, 249.0693f, 90.00001f),
                        localScale = new Vector3(0.13f, 0.13f, 0.13f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DeathProjectile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathProjectile"),
                        childName = "Pelvis",
                        localPos = new Vector3(-0.18682f, 0.32519f, 0.07876f),
                        localAngles = new Vector3(338.9847f, 300.7574f, 347.3894f),
                        localScale = new Vector3(0.08f, 0.08f, 0.08f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.LifestealOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
                        childName = "Head",
                        localPos = new Vector3(-0.06527f, 0.13382f, 0.20953f),
                        localAngles = new Vector3(38.51534f, 153.1544f, 92.39183f),
                        localScale = new Vector3(0.09f, 0.09f, 0.09f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.TeamWarCry,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[1]
                    {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayTeamWarCry"),
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.31184f, 0.19003f),
                        localAngles = new Vector3(2.97821f, 0f, 0f),
                        localScale = new Vector3(0.1f, 0.1f, 0.1f),
                        limbMask = LimbFlags.None
                    }
                    }
                }
            });
            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(array, 0);
            array[0].defaultMaterial = materials[0];
            array[1].defaultMaterial = materials[1];
            array[bodyRendererIndex].defaultMaterial = materials[2];
            array[4].defaultMaterial = materials[3];
            return array;
        }
    }
}
