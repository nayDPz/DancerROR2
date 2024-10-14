using EmotesAPI;
using RoR2;
using UnityEngine;

namespace Dancer.SoftDependencies
{

    public static class CustomEmotesAPICompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (!_enabled.HasValue)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
                }
                return _enabled.Value;
            }
        }

        public static void SetupSkeleton()
        {
            CustomEmotesAPI.animChanged += delegate (string newAnimation, BoneMapper mapper)
            {
                if ((bool)mapper && mapper.transform.parent.name.Contains("Dancer"))
                {
                    Transform transform = mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Lance");
                    if ((bool)transform)
                    {
                        if (newAnimation != "none")
                        {
                            transform.gameObject.SetActive(value: false);
                        }
                        else
                        {
                            transform.gameObject.SetActive(value: true);
                        }
                    }
                }
            };

            On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
        }

        private static void SurvivorCatalog_Init(On.RoR2.SurvivorCatalog.orig_Init orig)
        {
            orig.Invoke();
            foreach (RoR2.SurvivorDef allSurvivorDef in SurvivorCatalog.allSurvivorDefs)
            {
                if (allSurvivorDef.bodyPrefab.name == "DancerBody")
                {
                    GameObject gameObject = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DancerHumanoid");
                    CustomEmotesAPI.ImportArmature(allSurvivorDef.bodyPrefab, gameObject, 0, true);
                }
            }
        }
    }
}
