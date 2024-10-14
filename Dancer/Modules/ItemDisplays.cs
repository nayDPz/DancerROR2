using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dancer.Modules
{

    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static void PopulateDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponent<ModelLocator>()
                .modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
            ItemDisplayRuleSet.KeyAssetRuleGroup[] keyAssetRuleGroups = itemDisplayRuleSet.keyAssetRuleGroups;
            for (int i = 0; i < keyAssetRuleGroups.Length; i++)
            {
                ItemDisplayRule[] rules = keyAssetRuleGroups[i].displayRuleGroup.rules;
                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if ((bool)followerPrefab)
                    {
                        string key = followerPrefab.name?.ToLower();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        internal static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()) && (bool)itemDisplayPrefabs[name.ToLower()])
            {
                return itemDisplayPrefabs[name.ToLower()];
            }
            return null;
        }
    }
}
