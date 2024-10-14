using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dancer.Modules
{

    internal static class Skills
    {
        internal static List<SkillFamily> skillFamilies = new List<SkillFamily>();

        internal static List<SkillDef> skillDefs = new List<SkillDef>();

        internal static void CreateSkillFamilies(GameObject targetPrefab)
        {
            GenericSkill[] componentsInChildren = targetPrefab.GetComponentsInChildren<GenericSkill>();
            foreach (GenericSkill obj in componentsInChildren)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            SkillLocator component = targetPrefab.GetComponent<SkillLocator>();
            component.primary = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            ((UnityEngine.Object)skillFamily).name = targetPrefab.name + "PrimaryFamily";
            skillFamily.variants = new SkillFamily.Variant[0];
            component.primary._skillFamily = skillFamily;
            component.secondary = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily skillFamily2 = ScriptableObject.CreateInstance<SkillFamily>();
            ((UnityEngine.Object)skillFamily2).name = targetPrefab.name + "SecondaryFamily";
            skillFamily2.variants = new SkillFamily.Variant[0];
            component.secondary._skillFamily = skillFamily2;
            component.utility = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily skillFamily3 = ScriptableObject.CreateInstance<SkillFamily>();
            ((UnityEngine.Object)skillFamily3).name = targetPrefab.name + "UtilityFamily";
            skillFamily3.variants = new SkillFamily.Variant[0];
            component.utility._skillFamily = skillFamily3;
            component.special = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily skillFamily4 = ScriptableObject.CreateInstance<SkillFamily>();
            ((UnityEngine.Object)skillFamily4).name = targetPrefab.name + "SpecialFamily";
            skillFamily4.variants = new SkillFamily.Variant[0];
            component.special._skillFamily = skillFamily4;
            skillFamilies.Add(skillFamily);
            skillFamilies.Add(skillFamily2);
            skillFamilies.Add(skillFamily3);
            skillFamilies.Add(skillFamily4);
        }

        internal static void AddPassiveSkill(GameObject targetPrefab)
        {
            SkillLocator component = targetPrefab.GetComponent<SkillLocator>();
            component.passiveSkill.enabled = true;
            component.passiveSkill.skillNameToken = "NDP_DANCER_BODY_PASSIVE_NAME";
            component.passiveSkill.skillDescriptionToken = "NDP_DANCER_BODY_PASSIVE_DESCRIPTION";
            component.passiveSkill.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texBazookaOutIcon");
        }

        internal static void AddPrimarySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator component = targetPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = component.primary.skillFamily;
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, isFolder: false)
            };
        }

        internal static void AddSecondarySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator component = targetPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = component.secondary.skillFamily;
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, isFolder: false)
            };
        }

        internal static void AddSecondarySkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            foreach (SkillDef skillDef in skillDefs)
            {
                AddSecondarySkill(targetPrefab, skillDef);
            }
        }

        internal static void AddUtilitySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator component = targetPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = component.utility.skillFamily;
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, isFolder: false)
            };
        }

        internal static void AddUtilitySkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            foreach (SkillDef skillDef in skillDefs)
            {
                AddUtilitySkill(targetPrefab, skillDef);
            }
        }

        internal static void AddSpecialSkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator component = targetPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = component.special.skillFamily;
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, isFolder: false)
            };
        }

        internal static void AddSpecialSkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            foreach (SkillDef skillDef in skillDefs)
            {
                AddSpecialSkill(targetPrefab, skillDef);
            }
        }

        internal static SkillDef CreatePrimarySkillDef(SerializableEntityStateType state, string stateMachine, string skillNameToken, string skillDescriptionToken, Sprite skillIcon, bool agile)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = skillNameToken;
            skillDef.skillNameToken = skillNameToken;
            skillDef.skillDescriptionToken = skillDescriptionToken;
            skillDef.icon = skillIcon;
            skillDef.activationState = state;
            skillDef.activationStateMachineName = stateMachine;
            skillDef.baseMaxStock = 1;
            skillDef.baseRechargeInterval = 0f;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.canceledFromSprinting = false;
            skillDef.forceSprintDuringState = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.interruptPriority = InterruptPriority.Any;
            skillDef.resetCooldownTimerOnUse = false;
            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;
            skillDef.cancelSprintingOnActivation = !agile;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 0;
            skillDef.stockToConsume = 0;
            if (agile)
            {
                skillDef.keywordTokens = new string[1] { "KEYWORD_AGILE" };
            }
            skillDefs.Add(skillDef);
            return skillDef;
        }

        internal static SkillDef CreateSkillDef(SkillDefInfo skillDefInfo)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;
            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;
            skillDef.resetCooldownTimerOnUse = skillDefInfo.resetCooldownTimerOnUse;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.cancelSprintingOnActivation = skillDefInfo.cancelSprintingOnActivation;
            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;
            skillDef.keywordTokens = skillDefInfo.keywordTokens;
            skillDefs.Add(skillDef);
            return skillDef;
        }
    }
}
