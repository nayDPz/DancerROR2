using BepInEx.Configuration;
using EntityStates;
using Dancer.SkillStates;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace Dancer.Modules.Survivors
{
    public static class Dancer
	{
		internal static SkillDef lockedSkillDef;
		internal static void CreateCharacter()
		{
			Dancer.characterEnabled = Config.CharacterEnableConfig("Dancer");
			bool value = Dancer.characterEnabled.Value;
			if (value)
			{
				Dancer.characterPrefab = Prefabs.CreatePrefab("DancerBody", "mdlDancer", new BodyInfo
				{
					armor = 20f,
					armorGrowth = 0f,
					bodyName = "DancerBody",
					bodyNameToken = "NDP_DANCER_BODY_NAME",
					bodyColor = Color.magenta,
					characterPortrait = Assets.LoadCharacterIcon("Dancer"),
					crosshair = Assets.LoadCrosshair("Standard"),
					damage = 12f,
					healthGrowth = 48f,
					healthRegen = 2.5f,
					jumpCount = 1,
					jumpPower = 17f,
					acceleration = 80f,
					moveSpeed = 7f,
					maxHealth = 160f,
					subtitleNameToken = "NDP_DANCER_BODY_SUBTITLE",
					podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
				});
				Dancer.characterPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<FootstepHandler>().baseFootstepString = "Play_treeBot_step";
				//Dancer.characterPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<FootstepHandler>().footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericLargeFootstepDust");
				//Dancer.characterPrefab.GetComponent<SfxLocator>().fallDamageSound = "RidleyLandFallDamage";
				Dancer.characterPrefab.GetComponent<Interactor>().maxInteractionDistance = 5f;
				Dancer.characterPrefab.GetComponent<CameraTargetParams>().cameraParams = CameraParams.defaultCameraParams;
				Dancer.characterPrefab.GetComponent<SfxLocator>().landingSound = "DancerLand";
				Dancer.characterPrefab.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));
				characterPrefab.AddComponent<DancerComponent>();

				Material material = Assets.CreateMaterial("matDancer");
				Dancer.bodyRendererIndex = 0;
				Prefabs.SetupCharacterModel(Dancer.characterPrefab, new CustomRendererInfo[]
				{
					new CustomRendererInfo
					{
						childName = "Body",
						material = material
					},
					new CustomRendererInfo
					{
						childName = "Ribbons",
						material = material
					},
					new CustomRendererInfo
					{
						childName = "Lance",
						material = material
					}
				}, Dancer.bodyRendererIndex);
				Dancer.displayPrefab = Prefabs.CreateDisplayPrefab("mdlDancer", Dancer.characterPrefab);
				Prefabs.RegisterNewSurvivor(Dancer.characterPrefab, Dancer.displayPrefab, Color.magenta, "DANCER");
				Dancer.CreateHitboxes();
				Dancer.CreateSkills();
				Dancer.CreateSkins();
				Dancer.InitializeItemDisplays();
				Dancer.CreateDoppelganger();
			}
		}

		private static void CreateDoppelganger()
		{
			Prefabs.CreateGenericDoppelganger(Dancer.characterPrefab, "DancerMonsterMaster", "Merc");
		}

		private static void CreateHitboxes()
		{
			ChildLocator componentInChildren = Dancer.characterPrefab.GetComponentInChildren<ChildLocator>();
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
		}

		private static void CreateSkills()
		{
			Skills.CreateSkillFamilies(Dancer.characterPrefab);
			string str = "NDP";
			//Skills.AddPassiveSkill(Dancer.characterPrefab);
			SkillDef skillDef = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_PRIMARY_SLASH_NAME",
				skillNameToken = str + "_DANCER_BODY_PRIMARY_SLASH_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_PRIMARY_SLASH_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
				activationState = new SerializableEntityStateType(typeof(M1Entry)),
				activationStateMachineName = "Weapon",
				baseMaxStock = 1,
				baseRechargeInterval = 0f,
				beginSkillCooldownOnSkillEnd = true,
				canceledFromSprinting = false,
				forceSprintDuringState = false,
				fullRestockOnAssign = true,
				interruptPriority = InterruptPriority.Any,
				resetCooldownTimerOnUse = false,
				isCombatSkill = true,
				mustKeyPress = false,
				cancelSprintingOnActivation = false,
				rechargeStock = 1,
				requiredStock = 1,
				stockToConsume = 1,
				keywordTokens = new string[] { "KEYWORD_DANCER_CANCELS","KEYWORD_DANCER_JAB","KEYWORD_DANCER_DASH","KEYWORD_DANCER_DOWNTILT","KEYWORD_DANCER_UPAIR","KEYWORD_DANCER_FORWARDAIR","KEYWORD_DANCER_DOWNAIR"  }
			});

			SkillDef easySkillDef = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_PRIMARY_SLASH2_NAME",
				skillNameToken = str + "_DANCER_BODY_PRIMARY_SLASH2_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_PRIMARY_SLASH2_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
				activationState = new SerializableEntityStateType(typeof(EasyM1Entry)),
				activationStateMachineName = "Weapon",
				baseMaxStock = 1,
				baseRechargeInterval = 0f,
				beginSkillCooldownOnSkillEnd = true,
				canceledFromSprinting = false,
				forceSprintDuringState = false,
				fullRestockOnAssign = true,
				interruptPriority = InterruptPriority.Any,
				resetCooldownTimerOnUse = false,
				isCombatSkill = true,
				mustKeyPress = false,
				cancelSprintingOnActivation = true,
				rechargeStock = 1,
				requiredStock = 1,
				stockToConsume = 1
			});

			Skills.AddPrimarySkill(Dancer.characterPrefab, skillDef);
			//Skills.AddPrimarySkill(Dancer.characterPrefab, easySkillDef);

			SkillDef skillDef2 = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_SECONDARY_SLASH_NAME",
				skillNameToken = str + "_DANCER_BODY_SECONDARY_SLASH_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_SECONDARY_SLASH_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
				activationState = new SerializableEntityStateType(typeof(SpinnyMove)),
				activationStateMachineName = "Body",
				baseMaxStock = 1,
				baseRechargeInterval = 5f,
				beginSkillCooldownOnSkillEnd = true,
				canceledFromSprinting = false,
				forceSprintDuringState = false,
				fullRestockOnAssign = true,
				interruptPriority = InterruptPriority.PrioritySkill,
				resetCooldownTimerOnUse = false,
				isCombatSkill = true,
				mustKeyPress = false,
				cancelSprintingOnActivation = true,
				rechargeStock = 1,
				requiredStock = 1,
				stockToConsume = 1,
			});
			SkillDef skillDef22 = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_SECONDARY_PARRY_NAME",
				skillNameToken = str + "_DANCER_BODY_SECONDARY_PARRY_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_SECONDARY_PARRY_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
				activationState = new SerializableEntityStateType(typeof(ChargeParry)),
				activationStateMachineName = "Body",
				baseMaxStock = 1,
				baseRechargeInterval = 5f,
				beginSkillCooldownOnSkillEnd = true,
				canceledFromSprinting = true,
				forceSprintDuringState = false,
				fullRestockOnAssign = true,
				interruptPriority = InterruptPriority.PrioritySkill,
				resetCooldownTimerOnUse = false,
				isCombatSkill = true,
				mustKeyPress = true,
				cancelSprintingOnActivation = true,
				rechargeStock = 1,
				requiredStock = 1,
				stockToConsume = 1,
			});
			Skills.AddSecondarySkills(Dancer.characterPrefab, new SkillDef[]
			{
				skillDef2,
				//skillDef22
			});
			SkillDef skillDef3 = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_UTILITY_PULL_NAME",
				skillNameToken = str + "_DANCER_BODY_UTILITY_PULL_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_UTILITY_PULL_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon"),
				activationState = new SerializableEntityStateType(typeof(DragonLunge2)),
				activationStateMachineName = "Body",
				baseMaxStock = 1,
				baseRechargeInterval = 6f,
				beginSkillCooldownOnSkillEnd = true,
				canceledFromSprinting = false,
				forceSprintDuringState = false,
				fullRestockOnAssign = true,
				interruptPriority = InterruptPriority.PrioritySkill,
				resetCooldownTimerOnUse = false,
				isCombatSkill = true,
				mustKeyPress = false,
				cancelSprintingOnActivation = true,
				rechargeStock = 1,
				requiredStock = 1,
				stockToConsume = 1
			});
			SkillDef skillDef32 = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_UTILITY_PULL2_NAME",
				skillNameToken = str + "_DANCER_BODY_UTILITY_PULL2_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_UTILITY_PULL2_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon"),
				activationState = new SerializableEntityStateType(typeof(DragonLunge2)),
				activationStateMachineName = "Body",
				baseMaxStock = 1,
				baseRechargeInterval = 6f,
				beginSkillCooldownOnSkillEnd = true,
				canceledFromSprinting = false,
				forceSprintDuringState = false,
				fullRestockOnAssign = true,
				interruptPriority = InterruptPriority.PrioritySkill,
				resetCooldownTimerOnUse = false,
				isCombatSkill = true,
				mustKeyPress = false,
				cancelSprintingOnActivation = true,
				rechargeStock = 1,
				requiredStock = 1,
				stockToConsume = 1
			});
			Skills.AddUtilitySkills(Dancer.characterPrefab, new SkillDef[]
			{
				skillDef3,
			});
			SkillDef skillDef4 = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_SPECIAL_RIBBON_NAME",
				skillNameToken = str + "_DANCER_BODY_SPECIAL_RIBBON_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_SPECIAL_RIBBON_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
				activationState = new SerializableEntityStateType(typeof(FireChainRibbons)),
				activationStateMachineName = "Weapon",
				baseMaxStock = 1,
				baseRechargeInterval = 12f,
				beginSkillCooldownOnSkillEnd = true,
				canceledFromSprinting = false,
				forceSprintDuringState = false,
				fullRestockOnAssign = true,
				interruptPriority = InterruptPriority.PrioritySkill,
				resetCooldownTimerOnUse = false,
				isCombatSkill = true,
				mustKeyPress = false,
				cancelSprintingOnActivation = true,
				rechargeStock = 1,
				requiredStock = 1,
				stockToConsume = 1,
				keywordTokens = new string[] { "KEYWORD_DANCER_RIBBON" }
			});
			Skills.AddSpecialSkills(Dancer.characterPrefab, new SkillDef[]
			{
				skillDef4
			});
			lockedSkillDef = Skills.CreateSkillDef(new SkillDefInfo
			{
				skillName = str + "_DANCER_BODY_SPECIAL_RIBBON_LOCK_NAME",
				skillNameToken = str + "_DANCER_BODY_SPECIAL_RIBBON_LOCK_NAME",
				skillDescriptionToken = str + "_DANCER_BODY_SPECIAL_RIBBON_LOCK_DESCRIPTION",
				skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
				activationState = new SerializableEntityStateType(typeof(LockSkill)),
				activationStateMachineName = "Weapon",
				baseMaxStock = 0,
				baseRechargeInterval = 0f,
				beginSkillCooldownOnSkillEnd = false,
				canceledFromSprinting = false,
				forceSprintDuringState = false,
				fullRestockOnAssign = false,
				interruptPriority = InterruptPriority.Any,
				resetCooldownTimerOnUse = false,
				isCombatSkill = false,
				mustKeyPress = false,
				cancelSprintingOnActivation = false,
				rechargeStock = 0,
				requiredStock = 999,
				stockToConsume = 0,
			});
		}

		private static void CreateSkins()
        {
            GameObject model = characterPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();
            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(DancerPlugin.developerPrefix + "_DANCER_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

			defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
			{
				new SkinDef.MeshReplacement
				{
					mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDancerBody"),
					renderer = defaultRenderers[0].renderer
				},
				new SkinDef.MeshReplacement
				{
					mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDancerRibbons"),
					renderer = defaultRenderers[1].renderer
				},
				new SkinDef.MeshReplacement
				{
					mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDancerLance"),
					renderer = defaultRenderers[2].renderer
				},
			};


			skins.Add(defaultSkin);
			#endregion

			Material hornetMat = Modules.Assets.CreateMaterial("matHornet");
			Material capeMat = Modules.Assets.CreateMaterial("matHornetCape");

			CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[]
			{
                
                new CharacterModel.RendererInfo
				{
					defaultMaterial = hornetMat,
					defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
					ignoreOverlays = false,
                    //Which renderer(mesh) to replace.
                    renderer = defaultRenderers[0].renderer
				},
				new CharacterModel.RendererInfo
				{
					defaultMaterial = capeMat,
					defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
					ignoreOverlays = false,
                    //Which renderer(mesh) to replace.
                    renderer = defaultRenderers[1].renderer
				},
				new CharacterModel.RendererInfo
				{
					defaultMaterial = hornetMat,
					defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
					ignoreOverlays = false,
                    //Which renderer(mesh) to replace.
                    renderer = defaultRenderers[2].renderer
				},
			};

			SkinDef bugSkin = Modules.Skins.CreateSkinDef(DancerPlugin.developerPrefix + "_DANCER_HORNET_SKIN_NAME",
				Assets.mainAssetBundle.LoadAsset<Sprite>("texHornetSkin"),
				rendererInfos,
				mainRenderer,
				model);

			bugSkin.meshReplacements = new SkinDef.MeshReplacement[]
			{
				new SkinDef.MeshReplacement
				{
					mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBugBody"),
					renderer = defaultRenderers[0].renderer
				},
				new SkinDef.MeshReplacement
				{
					mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBugCape"),
					renderer = defaultRenderers[1].renderer
				},
				new SkinDef.MeshReplacement
				{
					mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBugNeedle"),
					renderer = defaultRenderers[2].renderer
				},
			};

			skins.Add(bugSkin);

			skinController.skins = skins.ToArray();
        }

		private static void InitializeItemDisplays()
		{
			CharacterModel componentInChildren = Dancer.characterPrefab.GetComponentInChildren<CharacterModel>();
			Dancer.itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
			Dancer.itemDisplayRuleSet.name = "idrsDancer";
			componentInChildren.itemDisplayRuleSet = Dancer.itemDisplayRuleSet;
		}
		internal static void SetItemDisplays()
		{
			Dancer.itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Jetpack,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
							childName = "Chest",
localPos = new Vector3(0.00477F, 0.10516F, -0.07228F),
localAngles = new Vector3(341.0195F, 0F, 0F),
localScale = new Vector3(0.35593F, 0.15914F, 0.12779F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.GoldGat,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
							childName = "Chest",
localPos = new Vector3(0.19943F, 0.36153F, 0.04903F),
localAngles = new Vector3(58.84191F, 250.4249F, 142.6867F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.BFG,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
							childName = "Chest",
localPos = new Vector3(0.10222F, 0.20518F, 0.05978F),
localAngles = new Vector3(0F, 0F, 327.4607F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.CritGlasses,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
							childName = "Head",
localPos = new Vector3(0F, 0.12999F, 0.16031F),
localAngles = new Vector3(9.15538F, 0F, 0F),
localScale = new Vector3(0.3215F, 0.3034F, 0.3034F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Syringe,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
							childName = "UpperArmR",
localPos = new Vector3(-0.00565F, 0.18895F, -0.04959F),
localAngles = new Vector3(352.4485F, 266.223F, 79.63069F),
localScale = new Vector3(0.15F, 0.15F, 0.15F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Behemoth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
							childName = "Chest",
localPos = new Vector3(0F, 0.2158F, -0.19895F),
localAngles = new Vector3(6.223F, 180F, 330.578F),
localScale = new Vector3(0.05801F, 0.07173F, 0.05726F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Missile,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
							childName = "Chest",
localPos = new Vector3(-0.42134F, 0.52133F, 0.02885F),
localAngles = new Vector3(2.35642F, 3.01076F, 51.98444F),
localScale = new Vector3(0.1F, 0.12268F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Dagger,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
							childName = "Chest",
localPos = new Vector3(0.27055F, 0.31367F, -0.03923F),
localAngles = new Vector3(305.3635F, 11.31594F, 355.1549F),
localScale = new Vector3(0.7F, 0.7F, 0.7F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
							childName = "Chest",
localPos = new Vector3(-0.26105F, 0.30321F, -0.044F),
localAngles = new Vector3(56.48328F, 175.5279F, 179.5179F),
localScale = new Vector3(-0.7F, -0.7F, -0.7F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Hoof,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
							childName = "CalfL",
localPos = new Vector3(-0.01997F, 0.78193F, 0.05326F),
localAngles = new Vector3(87.04044F, 76.51312F, 243.9886F),
localScale = new Vector3(0.13069F, 0.1303F, 0.13955F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ChainLightning,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
							childName = "Chest",
localPos = new Vector3(-0.0011F, 0.1031F, -0.16005F),
localAngles = new Vector3(9.29676F, 174.7346F, 24.99983F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.GhostOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
							childName = "Head",
localPos = new Vector3(0.00301F, 0.12038F, 0.056F),
localAngles = new Vector3(7.80833F, 0F, 0F),
localScale = new Vector3(0.73147F, 0.6313F, 1.33377F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Mushroom,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
							childName = "UpperArmR",
localPos = new Vector3(-0.00688F, 0.15733F, -0.04649F),
localAngles = new Vector3(359.4526F, 271.4876F, 89.98575F),
localScale = new Vector3(0.07F, 0.071F, 0.0701F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
							childName = "Head",
localPos = new Vector3(0F, 0.14479F, 0.09794F),
localAngles = new Vector3(13.53955F, 0F, 0F),
localScale = new Vector3(0.5666F, 0.5666F, 0.5666F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BleedOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
							childName = "Pelvis",
localPos = new Vector3(0.19175F, 0.34523F, 0.0177F),
localAngles = new Vector3(36.59357F, 177.5122F, 178.7256F),
localScale = new Vector3(0.5F, 0.5F, 0.68416F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.WardOnLevel,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWarbanner"),
							childName = "Chest",
localPos = new Vector3(0.0168F, 0.26039F, -0.21587F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.3162F, 0.3162F, 0.3162F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.HealOnCrit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayScythe"),
							childName = "Chest",
localPos = new Vector3(-0.08898F, 0.08614F, -0.19459F),
localAngles = new Vector3(293.1983F, 281.6273F, 259.683F),
localScale = new Vector3(0.30835F, 0.1884F, 0.32014F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.HealWhileSafe,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
							childName = "Chest",
localPos = new Vector3(-0.11196F, 0.21602F, 0.01362F),
localAngles = new Vector3(24.59701F, 9.63536F, 35.11288F),
localScale = new Vector3(0.07F, 0.07F, 0.07F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Clover,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
							childName = "Head",
localPos = new Vector3(0.0039F, 0.04673F, 0.2657F),
localAngles = new Vector3(85.61921F, 0.0001F, 179.4897F),
localScale = new Vector3(0.35F, 0.35F, 0.35F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BarrierOnOverHeal,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
							childName = "LowerArmR",
localPos = new Vector3(-0.08954F, 0.0619F, 0.01725F),
localAngles = new Vector3(81.85985F, 259.0763F, 155.0281F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.GoldOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
							childName = "Head",
localPos = new Vector3(0F, 0.15792F, -0.02926F),
localAngles = new Vector3(13.43895F, 311.3283F, 343.426F),
localScale = new Vector3(1.1754F, 1.1754F, 0.92166F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.WarCryOnMultiKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
							childName = "UpperArmR",
localPos = new Vector3(-0.07401F, 0.15409F, -0.07474F),
localAngles = new Vector3(65.18475F, 214.0179F, 355.7895F),
localScale = new Vector3(0.7094F, 0.7094F, 0.7094F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintArmor,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
							childName = "LowerArmR",
localPos = new Vector3(0.00659F, 0.25923F, 0.00061F),
localAngles = new Vector3(357.2321F, 117.9287F, 90.60644F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.IceRing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
							childName = "Weapon",
localPos = new Vector3(0.00685F, 0.08912F, 0F),
localAngles = new Vector3(274.3965F, 90F, 270F),
localScale = new Vector3(0.5F, 0.5F, 1.0828F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FireRing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
							childName = "Weapon",
localPos = new Vector3(0.0028F, 0.24892F, 0.00001F),
localAngles = new Vector3(274.3965F, 90F, 270F),
localScale = new Vector3(0.5F, 0.5F, 1.12845F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.UtilitySkillMagazine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
							childName = "Chest",
localPos = new Vector3(-0.34596F, 0.28343F, -0.00666F),
localAngles = new Vector3(313.2421F, 95.99242F, 180F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
							childName = "Chest",
localPos = new Vector3(0.34932F, 0.30434F, -0.00161F),
localAngles = new Vector3(49.85487F, 108.7415F, 199.3213F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.JumpBoost,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
							childName = "Head",
localPos = new Vector3(0F, -0.20925F, -0.41214F),
localAngles = new Vector3(34.52309F, 0F, 0F),
localScale = new Vector3(1.09546F, 0.99388F, 1.40369F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ArmorReductionOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.NearbyDamageBonus,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
							childName = "Weapon",
localPos = new Vector3(-0.002F, -0.03488F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1236F, 0.1236F, 0.1236F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ArmorPlate,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
							childName = "LowerArmL",
localPos = new Vector3(0.06063F, 0.19185F, -0.01304F),
localAngles = new Vector3(76.57407F, 81.50978F, 180F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.CommandMissile,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileRack"),
							childName = "Chest",
localPos = new Vector3(-0.23202F, 0.28282F, 0.01393F),
localAngles = new Vector3(342.4002F, 168.7708F, 34.96157F),
localScale = new Vector3(0.3362F, 0.3362F, 0.3362F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Feather,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFeather"),
							childName = "Head",
localPos = new Vector3(-0.01522F, 0.13423F, -0.05715F),
localAngles = new Vector3(314.8987F, 357.7154F, 11.71727F),
localScale = new Vector3(0.03962F, 0.03327F, 0.0285F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Crowbar,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
							childName = "Pelvis",
localPos = new Vector3(-0.1691F, 0.24688F, -0.14963F),
localAngles = new Vector3(43.55907F, 11.81575F, 11.82357F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FallBoots,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
							childName = "CalfL",
localPos = new Vector3(-0.0038F, 0.37291F, -0.01597F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.30579F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
							childName = "CalfR",
localPos = new Vector3(-0.0038F, 0.37291F, -0.01597F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.30579F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
							childName = "ThighR",
localPos = new Vector3(-0.15427F, 0.16356F, 0.0345F),
localAngles = new Vector3(80.68687F, 279.3683F, 175.5874F),
localScale = new Vector3(0.1843F, 0.1843F, 0.1843F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.EquipmentMagazine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBattery"),
							childName = "Weapon",
localPos = new Vector3(0.00188F, 0.07925F, -0.00277F),
localAngles = new Vector3(272.4369F, 145.8502F, 32.1286F),
localScale = new Vector3(0.2F, 0.2F, 0.23894F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.NovaOnHeal,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
							childName = "Head",
localPos = new Vector3(-0.0949F, 0.0945F, 0.0654F),
localAngles = new Vector3(6.82254F, 22.89301F, 0F),
localScale = new Vector3(-0.5349F, 0.5349F, 0.5349F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
							childName = "Head",
localPos = new Vector3(0.0949F, 0.0945F, 0.0654F),
localAngles = new Vector3(6.82253F, 337.107F, 0F),
localScale = new Vector3(0.5349F, 0.5349F, 0.5349F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Infusion,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayInfusion"),
							childName = "Chest",
localPos = new Vector3(-0.17919F, -0.00327F, 0.06826F),
localAngles = new Vector3(20.46128F, 304.1627F, 0F),
localScale = new Vector3(0.5253F, 0.5253F, 0.5253F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Medkit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
							childName = "Pelvis",
localPos = new Vector3(0.11193F, 0.30977F, 0.0997F),
localAngles = new Vector3(280.1438F, 349.5922F, 38.17199F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Bandolier,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
							childName = "Pelvis",
localPos = new Vector3(0.01815F, 0.35131F, -0.02141F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.42224F, 0.59252F, 0.242F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BounceNearby,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
							childName = "Chest",
localPos = new Vector3(0.167F, 0.23155F, -0.0053F),
localAngles = new Vector3(358.6279F, 35.35772F, 283.0336F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.IgniteOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGasoline"),
							childName = "ThighL",
localPos = new Vector3(0.1586F, 0.09434F, 0.04533F),
localAngles = new Vector3(78.44348F, 270F, 270F),
localScale = new Vector3(0.65F, 0.65F, 0.65F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.StunChanceOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayStunGrenade"),
							childName = "Chest",
localPos = new Vector3(-0.18348F, -0.04668F, 0.05757F),
localAngles = new Vector3(8.86498F, 4.53647F, 11.30128F),
localScale = new Vector3(0.8F, 0.8F, 0.8F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Firework,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFirework"),
							childName = "HandR",
localPos = new Vector3(0.03615F, 0.09452F, 0.09992F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1194F, 0.1194F, 0.17403F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarDagger,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayLunarDagger"),
							childName = "HandR",
localPos = new Vector3(0.01621F, 0.05569F, 0.01211F),
localAngles = new Vector3(65.45142F, 346.3312F, 235.2556F),
localScale = new Vector3(0.3385F, 1.01916F, 0.3385F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Knurl,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayKnurl"),
							childName = "UpperArmR",
localPos = new Vector3(-0.02841F, 0.14421F, -0.01509F),
localAngles = new Vector3(78.87074F, 36.6722F, 105.8275F),
localScale = new Vector3(0.0848F, 0.1006F, 0.10147F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BeetleGland,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
							childName = "Pelvis",
localPos = new Vector3(-0.25656F, 0.29023F, -0.0332F),
localAngles = new Vector3(349.7159F, 177.1544F, 2.7042F),
localScale = new Vector3(0.09594F, 0.08604F, 0.09076F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintBonus,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySoda"),
							childName = "Pelvis",
localPos = new Vector3(-0.19025F, 0.25343F, -0.03256F),
localAngles = new Vector3(270F, 251.0168F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SecondarySkillMagazine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
							childName = "LowerArmL",
localPos = new Vector3(-0.06727F, 0.18375F, 0.03843F),
localAngles = new Vector3(335.0898F, 25.122F, 176.703F),
localScale = new Vector3(0.07F, 0.07F, 0.07F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.StickyBomb,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayStickyBomb"),
							childName = "HandR",
localPos = new Vector3(0.01519F, 0.0686F, -0.08218F),
localAngles = new Vector3(78.27731F, 0.00002F, 171.4936F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.TreasureCache,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayKey"),
							childName = "CalfR",
localPos = new Vector3(-0.06349F, -0.13659F, -0.04507F),
localAngles = new Vector3(345.2247F, 238.6147F, 228.6344F),
localScale = new Vector3(1F, 1F, 1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BossDamageBonus,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAPRound"),
							childName = "Pelvis",
localPos = new Vector3(0.21936F, 0.36784F, -0.00669F),
localAngles = new Vector3(78.75988F, 151.3122F, 84.95592F),
localScale = new Vector3(0.7F, 0.7F, 0.46965F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SlowOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBauble"),
							childName = "UpperArmR",
localPos = new Vector3(-0.70539F, 0.12032F, 0.01842F),
localAngles = new Vector3(70.31753F, 62.26086F, 287.2839F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ExtraLife,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayHippo"),
							childName = "Chest",
localPos = new Vector3(0.00655F, 0.11081F, 0.21627F),
localAngles = new Vector3(11.20005F, 351.6793F, 344.3522F),
localScale = new Vector3(0.32F, 0.32F, 0.32F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.KillEliteFrenzy,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
							childName = "Head",
localPos = new Vector3(0.04038F, 0.12258F, 0.03265F),
localAngles = new Vector3(39.83437F, 0F, 0F),
localScale = new Vector3(0.38565F, 0.2638F, 0.41101F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.RepeatHeal,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayCorpseFlower"),
							childName = "UpperArmL",
localPos = new Vector3(0.15459F, 0.1895F, -0.15276F),
localAngles = new Vector3(76.12807F, 336.4276F, 180F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.AutoCastEquipment,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFossil"),
							childName = "Pelvis",
localPos = new Vector3(-0.00514F, 0.28757F, 0.1698F),
localAngles = new Vector3(0F, 89.42922F, 0F),
localScale = new Vector3(0.4208F, 0.4208F, 0.4208F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.IncreaseHealing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
							childName = "Head",
localPos = new Vector3(0.07414F, 0.19751F, -0.0258F),
localAngles = new Vector3(344.5861F, 132.518F, 356.9124F),
localScale = new Vector3(0.3395F, 0.3395F, 0.3395F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
							childName = "Head",
localPos = new Vector3(-0.07414F, 0.19751F, -0.0258F),
localAngles = new Vector3(15.41393F, 42F, 356.9124F),
localScale = new Vector3(0.3395F, 0.3395F, -0.3395F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.TitanGoldDuringTP,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldHeart"),
							childName = "Chest",
localPos = new Vector3(-0.10856F, 0.05727F, 0.17572F),
localAngles = new Vector3(340.0459F, 326.5524F, 1.80304F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintWisp,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
							childName = "Chest",
localPos = new Vector3(0.13391F, 0.05807F, 0.15326F),
localAngles = new Vector3(20.49514F, 15.08605F, 357.7958F),
localScale = new Vector3(0.1385F, 0.1385F, 0.1385F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BarrierOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBrooch"),
							childName = "LowerArmR",
localPos = new Vector3(-0.0614F, 0.18859F, 0.02649F),
localAngles = new Vector3(81.03616F, 314.4032F, 15.99828F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.TPHealingNova,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGlowFlower"),
							childName = "UpperArmR",
localPos = new Vector3(-0.15287F, 0.19369F, -0.13507F),
localAngles = new Vector3(0F, 222.6754F, 0F),
localScale = new Vector3(0.35F, 0.35F, 0.35F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarUtilityReplacement,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdFoot"),
							childName = "CalfR",
localPos = new Vector3(0F, 0.76226F, 0.16685F),
localAngles = new Vector3(0F, 270F, 272.4676F),
localScale = new Vector3(1F, 1.4565F, 1.2945F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Thorns,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
							childName = "Head",
localPos = new Vector3(-0.07555F, 0.12033F, 0.15897F),
localAngles = new Vector3(301.2076F, 146.43F, 189.6029F),
localScale = new Vector3(0.25567F, 0.23304F, 0.17833F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.NovaOnLowHealth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayJellyGuts"),
							childName = "Chest",
localPos = new Vector3(0.01407F, 0.31906F, 0.0984F),
localAngles = new Vector3(47.005F, 278.725F, 140.888F),
localScale = new Vector3(0.15F, 0.15F, 0.15F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarTrinket,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBeads"),
							childName = "UpperArmL",
localPos = new Vector3(-0.00329F, 0.16805F, 0.05026F),
localAngles = new Vector3(0F, 0F, 99.29353F),
localScale = new Vector3(1.13087F, 1.19312F, 1.08218F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Plant,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayInterstellarDeskPlant"),
							childName = "Chest",
localPos = new Vector3(0.17918F, 0.26891F, 0.01079F),
localAngles = new Vector3(299.256F, 26.12627F, 249.1656F),
localScale = new Vector3(0.07F, 0.07F, 0.07F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Bear,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
							childName = "Chest",
localPos = new Vector3(0.01423F, 0.21404F, -0.20479F),
localAngles = new Vector3(359.4737F, 180.8153F, 2.00493F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.DeathMark,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
							childName = "UpperArmR",
localPos = new Vector3(-0.00634F, 0.14811F, -0.02871F),
localAngles = new Vector3(277.5254F, 0F, 346.0966F),
localScale = new Vector3(-0.05F, -0.05F, -0.06F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ExplodeOnDeath,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
							childName = "LowerArmR",
localPos = new Vector3(-0.00096F, 0.41647F, 0.00463F),
localAngles = new Vector3(2.77335F, 359.1895F, 176.0624F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Seed,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
							childName = "Chest",
localPos = new Vector3(0.05344F, -0.04829F, 0.15088F),
localAngles = new Vector3(306.1523F, 261.2392F, 242.3862F),
localScale = new Vector3(0.03394F, 0.03326F, 0.03041F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintOutOfCombat,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
							childName = "Pelvis",
localPos = new Vector3(0.23977F, 0.22895F, -0.00201F),
localAngles = new Vector3(1.87579F, 354.8801F, 24.10377F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.CooldownOnCrit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySkull"),
							childName = "Chest",
							localPos = new Vector3(0f, 0.3997f, 0f),
							localAngles = new Vector3(270f, 0f, 0f),
							localScale = new Vector3(0.2789f, 0.2789f, 0.2789f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Phasing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayStealthkit"),
							childName = "ThighR",
localPos = new Vector3(-0.08927F, 0.20322F, 0.12008F),
localAngles = new Vector3(90F, 139.0103F, 0F),
localScale = new Vector3(0.37577F, 0.56422F, 0.35F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.PersonalShield,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ShockNearby,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
							childName = "Head",
localPos = new Vector3(0F, 0.10735F, -0.0308F),
localAngles = new Vector3(295.9102F, 0F, 0F),
localScale = new Vector3(0.3229F, 0.3229F, 0.3229F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ShieldOnly,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
							childName = "Head",
localPos = new Vector3(-0.0868F, 0.26797F, 0F),
localAngles = new Vector3(11.8181F, 268.0985F, 359.6104F),
localScale = new Vector3(0.3521F, 0.3521F, -0.3521F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
							childName = "Head",
localPos = new Vector3(0.07771F, 0.26797F, 0F),
localAngles = new Vector3(348.1819F, 268.0985F, 0.3896F),
localScale = new Vector3(0.3521F, 0.3521F, 0.3521F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.AlienHead,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
							childName = "Weapon",
localPos = new Vector3(0.02859F, -0.04522F, 0.01442F),
localAngles = new Vector3(284.1172F, 243.7966F, 260.89F),
localScale = new Vector3(0.8F, 0.8F, 0.8F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.HeadHunter,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySkullCrown"),
							childName = "Head",
localPos = new Vector3(0F, 0.2442F, 0.03993F),
localAngles = new Vector3(15.9298F, 0F, 0F),
localScale = new Vector3(0.4851F, 0.1617F, 0.1617F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWarHorn"),
							childName = "Head",
localPos = new Vector3(-0.06565F, 0.01069F, 0.33667F),
localAngles = new Vector3(321.7959F, 253.9446F, 1.83296F),
localScale = new Vector3(0.2732F, 0.2732F, 0.2732F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FlatHealth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySteakCurved"),
							childName = "Head",
localPos = new Vector3(0F, 0.18682F, 0.13945F),
localAngles = new Vector3(285.4012F, 0.00002F, -0.00001F),
localScale = new Vector3(0.1245F, 0.1155F, 0.1155F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Tooth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
							childName = "Head",
localPos = new Vector3(0.04568F, -0.00035F, 0.19319F),
localAngles = new Vector3(25.05384F, 17.60428F, 7.65284F),
localScale = new Vector3(1.5F, 1.5F, 1.5F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
							childName = "Head",
localPos = new Vector3(-0.02361F, -0.003F, 0.19844F),
localAngles = new Vector3(25.68274F, 348.6905F, 355.0463F),
localScale = new Vector3(1.5F, 1.5F, 1.5F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Pearl,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ShinyPearl,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
							childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(328.163F, 284.6944F, 27.83253F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayTome"),
							childName = "ThighR",
localPos = new Vector3(-0.13437F, 0.02671F, 0.00331F),
localAngles = new Vector3(4.74548F, 269.9583F, 359.6995F),
localScale = new Vector3(0.08F, 0.08F, 0.08F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Squid,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySquidTurret"),
							childName = "Chest",
localPos = new Vector3(-0.0164F, 0.23145F, 0.0095F),
localAngles = new Vector3(0F, 90F, 7.10089F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Icicle,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Talisman,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LaserTurbine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
							childName = "Pelvis",
localPos = new Vector3(0F, 0.27358F, 0.17357F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FocusConvergence,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Incubator,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAncestralIncubator"),
							childName = "UpperArmL",
localPos = new Vector3(0.03967F, 0.23859F, 0.00643F),
localAngles = new Vector3(353.0521F, 317.2421F, 263.0239F),
localScale = new Vector3(0.03F, 0.03F, 0.03F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FireballsOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
							childName = "HandR",
localPos = new Vector3(0.01431F, 0.07931F, 0.00529F),
localAngles = new Vector3(344.0956F, 176.9312F, 2.92877F),
localScale = new Vector3(0.0697F, 0.07056F, 0.074F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SiphonOnLowHealth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySiphonOnLowHealth"),
							childName = "LowerArmR",
localPos = new Vector3(-0.00745F, 0.25453F, 0.03687F),
localAngles = new Vector3(354.4601F, 0F, 0F),
localScale = new Vector3(0.1F, 0.16051F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBleedOnHitAndExplode"),
							childName = "ThighR",
localPos = new Vector3(-0.11983F, -0.00669F, 0.03566F),
localAngles = new Vector3(0F, 0F, 105.2972F),
localScale = new Vector3(0.08F, 0.08F, 0.08F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.MonstersOnShrineUse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMonstersOnShrineUse"),
							childName = "Chest",
localPos = new Vector3(-0.08897F, 0.00789F, 0.20548F),
localAngles = new Vector3(55.74505F, 264.118F, 355.134F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.RandomDamageZone,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
							childName = "Chest",
localPos = new Vector3(-0.00001F, 0.17219F, -0.30708F),
localAngles = new Vector3(34.84705F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Fruit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
							childName = "Weapon",
localPos = new Vector3(-0.08626F, -0.15387F, 0.21263F),
localAngles = new Vector3(16.74619F, 94.02277F, 334.7391F),
localScale = new Vector3(0.2118F, 0.2118F, 0.2118F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixRed,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
							childName = "Head",
localPos = new Vector3(0.09221F, 0.18718F, -0.03466F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1036F, 0.1036F, 0.1036F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
							childName = "Head",
localPos = new Vector3(-0.09221F, 0.18718F, -0.03466F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(-0.1036F, 0.1036F, 0.1036F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixBlue,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
							childName = "Head",
localPos = new Vector3(0F, 0.09709F, 0.19904F),
localAngles = new Vector3(315F, 0F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
							childName = "Head",
localPos = new Vector3(0F, 0.14871F, 0.15627F),
localAngles = new Vector3(300F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixWhite,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
							childName = "Head",
localPos = new Vector3(0F, 0.28471F, 0.01687F),
localAngles = new Vector3(282.0894F, 0F, 0F),
localScale = new Vector3(0.0265F, 0.0265F, 0.0265F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixPoison,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
							childName = "Head",
localPos = new Vector3(0F, 0.19891F, 0.0624F),
localAngles = new Vector3(298.819F, 0F, 0F),
localScale = new Vector3(0.0496F, 0.0496F, 0.0496F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixHaunted,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
							childName = "Head",
localPos = new Vector3(0F, 0.26174F, 0.00841F),
localAngles = new Vector3(280.0554F, 0F, 0F),
localScale = new Vector3(0.065F, 0.065F, 0.065F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.CritOnUse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
							childName = "Head",
localPos = new Vector3(0F, 0.05455F, 0.41408F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2326F, 0.2326F, 0.2326F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.DroneBackup,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayRadio"),
							childName = "ThighL",
localPos = new Vector3(0.12572F, -0.04588F, -0.01867F),
localAngles = new Vector3(11.69349F, 91.28197F, 186.3007F),
localScale = new Vector3(0.7F, 0.7F, 0.7F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Lightning,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.BurnNearby,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayPotion"),
							childName = "HandR",
localPos = new Vector3(0.00713F, 0.10868F, -0.11556F),
localAngles = new Vector3(63.45895F, 264.0619F, 232.0915F),
localScale = new Vector3(0.05F, 0.05F, 0.05F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.CrippleWard,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEffigy"),
							childName = "Pelvis",
localPos = new Vector3(0.12859F, 0.21196F, 0.11489F),
localAngles = new Vector3(354.6827F, 203.9411F, 339.1208F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBatteryArray"),
							childName = "Chest",
localPos = new Vector3(0F, 0.19107F, -0.15272F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2188F, 0.2188F, 0.2188F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.GainArmor,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayElephantFigure"),
							childName = "Chest",
localPos = new Vector3(-0.17946F, 0.30134F, -0.02533F),
localAngles = new Vector3(7.57041F, 0F, 26.79241F),
localScale = new Vector3(0.6279F, 0.6279F, 0.6279F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Recycle,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayRecycler"),
							childName = "Chest",
localPos = new Vector3(0F, 0.18308F, -0.14788F),
localAngles = new Vector3(0F, 90F, 343.8301F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.FireBallDash,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayEgg"),
							childName = "Pelvis",
localPos = new Vector3(0.21155F, 0.33451F, 0.00231F),
localAngles = new Vector3(284.2241F, 279.7924F, 78.87299F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Cleanse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWaterPack"),
							childName = "Chest",
localPos = new Vector3(0F, 0.02586F, -0.13308F),
localAngles = new Vector3(16.98321F, 180F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Tonic,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayTonic"),
							childName = "HandR",
localPos = new Vector3(0.01919F, 0.09174F, -0.08501F),
localAngles = new Vector3(347.4816F, 264.4327F, 270.9305F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Gateway,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayVase"),
							childName = "Pelvis",
localPos = new Vector3(-0.21348F, 0.3313F, 0F),
localAngles = new Vector3(12.8791F, 90F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.11834F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Meteor,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Saw,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Blackhole,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
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
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Scanner,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayScanner"),
							childName = "Chest",
localPos = new Vector3(-0.2131F, 0.25091F, -0.03601F),
localAngles = new Vector3(298.4561F, 249.0693F, 90.00001F),
localScale = new Vector3(0.13F, 0.13F, 0.13F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.DeathProjectile,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathProjectile"),
							childName = "Pelvis",
localPos = new Vector3(-0.18682F, 0.32519F, 0.07876F),
localAngles = new Vector3(338.9847F, 300.7574F, 347.3894F),
localScale = new Vector3(0.08F, 0.08F, 0.08F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.LifestealOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
							childName = "Head",
localPos = new Vector3(-0.06527F, 0.13382F, 0.20953F),
localAngles = new Vector3(38.51534F, 153.1544F, 92.39183F),
localScale = new Vector3(0.09F, 0.09F, 0.09F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.TeamWarCry,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayTeamWarCry"),
							childName = "Pelvis",
localPos = new Vector3(0F, 0.31184F, 0.19003F),
localAngles = new Vector3(2.97821F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			Dancer.itemDisplayRuleSet.keyAssetRuleGroups = Dancer.itemDisplayRules.ToArray();
			Dancer.itemDisplayRuleSet.GenerateRuntimeValues();
		}

		private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
		{
			CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[defaultRenderers.Length];
			defaultRenderers.CopyTo(array, 0);
			array[0].defaultMaterial = materials[0];
			array[1].defaultMaterial = materials[1];
			array[Dancer.bodyRendererIndex].defaultMaterial = materials[2];
			array[4].defaultMaterial = materials[3];
			return array;
		}

		internal static GameObject characterPrefab;

		internal static GameObject displayPrefab;

		internal static ConfigEntry<bool> characterEnabled;

		public const string bodyName = "NdpDancerBody";

		public static int bodyRendererIndex;

		internal static ItemDisplayRuleSet itemDisplayRuleSet;

		internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;
	}
}
