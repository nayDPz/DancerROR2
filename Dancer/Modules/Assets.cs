using Dancer.Modules.Components;
using R2API;
using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace Dancer.Modules
{
    internal static class Assets
    {
        internal const string ASSET_BUNDLE_NAME = "Dancer.dancerassets";

        internal const string SOUNDBANK_DANCER_NAME = "Dancer.DancerBank.bnk";
        internal const string SOUNDBANK_RIDLEY_NAME = "Dancer.RidleyBank.bnk";

        internal static AssetBundle mainAssetBundle;

        internal static GameObject ribbonController;

        internal static GameObject spikeGroundEffect;

        internal static GameObject crosshairPrefab;

        internal static GameObject mageFriendPrefab;

        internal static GameObject stabHitEffect;

        internal static GameObject hitEffect;

        internal static GameObject bigHitEffect;

        internal static GameObject downAirEffect;

        internal static GameObject downAirEndEffect;

        internal static GameObject dashAttackEffect;

        internal static GameObject bigSwingEffect;

        internal static GameObject dragonLungeEffect;

        internal static GameObject dragonLungePullEffect;

        internal static GameObject swingEffect;

        internal static NetworkSoundEventDef grabGroundSoundEvent;

        internal static NetworkSoundEventDef lungeHitSoundEvent;

        internal static NetworkSoundEventDef whip1HitSoundEvent;

        internal static NetworkSoundEventDef whip2HitSoundEvent;

        internal static NetworkSoundEventDef sword1HitSoundEvent;

        internal static NetworkSoundEventDef sword2HitSoundEvent;

        internal static NetworkSoundEventDef sword3HitSoundEvent;

        internal static NetworkSoundEventDef hit2SoundEvent;

        internal static NetworkSoundEventDef jab1HitSoundEvent;

        internal static NetworkSoundEventDef jab2HitSoundEvent;

        internal static NetworkSoundEventDef jab3HitSoundEvent;

        internal static NetworkSoundEventDef punchHitSoundEvent;

        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        internal static List<GameObject> networkedObjectPrefabs = new List<GameObject>();

        internal static List<EffectDef> effectDefs = new List<EffectDef>();

        public static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");

        public static Shader cloud = Resources.Load<Shader>("shaders/fx/hgcloudremap");

        public static Material commandoMat;

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>
    {
        { "stubbedshader/deferred/hgstandard", "RoR2/Base/Shaders/HGStandard.shader" },
        { "stubbedshader/fx/hgcloudremap", "RoR2/Base/Shaders/HGCloudRemap.shader" },
        { "stubbedshader/fx/hgopaquecloudremap", "RoR2/Base/Shaders/HGOpaqueCloudRemap.shader" }
    };

        internal static void PopulateAssets()
        {
            var pathToDll = System.IO.Path.GetDirectoryName(typeof(DancerPlugin).Assembly.Location);

            if (mainAssetBundle == null)
            {
                mainAssetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(pathToDll, ASSET_BUNDLE_NAME));
            }
            ShaderConversion(mainAssetBundle);
            SoundAPI.SoundBanks.Add(System.IO.Path.Combine(pathToDll, SOUNDBANK_RIDLEY_NAME));
            SoundAPI.SoundBanks.Add(System.IO.Path.Combine(pathToDll, SOUNDBANK_DANCER_NAME));

            crosshairPrefab = mainAssetBundle.LoadAsset<GameObject>("DancerCrosshair");
            crosshairPrefab.AddComponent<HudElement>();
            crosshairPrefab.AddComponent<CrosshairController>();
            crosshairPrefab.AddComponent<DancerCrosshairController>();
            grabGroundSoundEvent = CreateNetworkSoundEventDef("GrabHitGround");
            sword1HitSoundEvent = CreateNetworkSoundEventDef("SwordHit1");
            sword2HitSoundEvent = CreateNetworkSoundEventDef("SwordHit2");
            whip1HitSoundEvent = CreateNetworkSoundEventDef("WhipHit1");
            whip2HitSoundEvent = CreateNetworkSoundEventDef("WhipHit2");
            sword3HitSoundEvent = CreateNetworkSoundEventDef("SwordHit3");
            jab1HitSoundEvent = CreateNetworkSoundEventDef("JabHit1");
            jab2HitSoundEvent = CreateNetworkSoundEventDef("JabHit2");
            jab3HitSoundEvent = CreateNetworkSoundEventDef("JabHit3");
            hit2SoundEvent = CreateNetworkSoundEventDef("Hit2");
            punchHitSoundEvent = CreateNetworkSoundEventDef("PunchHit");
            lungeHitSoundEvent = CreateNetworkSoundEventDef("LungeHit");
            GameObject gameObject = mainAssetBundle.LoadAsset<GameObject>("DancerDAirEffect");
            gameObject.AddComponent<DestroyOnTimer>().duration = 12f;
            gameObject.AddComponent<NetworkIdentity>();
            gameObject.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            downAirEffect = gameObject;
            stabHitEffect = LoadEffect("DancerStabHitEffect", parentToTransform: true);
            bigHitEffect = LoadEffect("DancerBigHitEffect", parentToTransform: true);
            hitEffect = LoadEffect("DancerHitEffect", parentToTransform: true);
            downAirEndEffect = LoadEffect("DancerDownAirEndEffect", parentToTransform: true);
            dashAttackEffect = LoadEffect("DancerDashAttackEffect", parentToTransform: true);
            bigSwingEffect = LoadEffect("DancerBigSwingEffect", parentToTransform: true);
            dragonLungeEffect = LoadEffect("DancerDragonLungeEffect", parentToTransform: true);
            swingEffect = LoadEffect("DancerSwingEffect", parentToTransform: true);
            ribbonController = mainAssetBundle.LoadAsset<GameObject>("RibbonController");
            ribbonController.AddComponent<RibbonController>();
            PrefabAPI.RegisterNetworkPrefab(ribbonController);
            networkedObjectPrefabs.Add(ribbonController);
        }

        public static void ShaderConversion(AssetBundle assets)
        {
            IEnumerable<Material> enumerable = from material in assets.LoadAllAssets<Material>()
                                               where material.shader.name.StartsWith("Stubbed")
                                               select material;
            foreach (Material item in enumerable)
            {
                Shader shader = Addressables.LoadAssetAsync<Shader>(ShaderLookup[item.shader.name.ToLowerInvariant()]).WaitForCompletion();
                if ((bool)shader)
                {
                    item.shader = shader;
                }
            }
        }

        public static void AttachControllerFinderToObjects(AssetBundle assetbundle)
        {
            if (!assetbundle)
            {
                return;
            }
            GameObject[] array = assetbundle.LoadAllAssets<GameObject>();
            GameObject[] array2 = array;
            foreach (GameObject gameObject in array2)
            {
                IEnumerable<Renderer> enumerable = from x in gameObject.GetComponentsInChildren<Renderer>()
                                                   where (bool)x.sharedMaterial && x.sharedMaterial.shader.name.StartsWith("Hopoo Games")
                                                   select x;
                foreach (Renderer item in enumerable)
                {
                    MaterialControllerComponents.HGControllerFinder hGControllerFinder = item.gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
                }
            }
            array = null;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;
            networkSoundEventDefs.Add(networkSoundEventDef);
            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            MeshRenderer[] componentsInChildren = objectToConvert.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in componentsInChildren)
            {
                if ((bool)meshRenderer && (bool)meshRenderer.material)
                {
                    meshRenderer.material.shader = hotpoo;
                }
            }
            SkinnedMeshRenderer[] componentsInChildren2 = objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
            {
                if ((bool)skinnedMeshRenderer && (bool)skinnedMeshRenderer.material)
                {
                    skinnedMeshRenderer.material.shader = hotpoo;
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[componentsInChildren.Length];
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                array[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = componentsInChildren[i].material,
                    renderer = componentsInChildren[i],
                    defaultShadowCastingMode = ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }
            return array;
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", parentToTransform: false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, parentToTransform: false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject gameObject = mainAssetBundle.LoadAsset<GameObject>(resourceName);
            gameObject.AddComponent<DestroyOnTimer>().duration = 12f;
            gameObject.AddComponent<NetworkIdentity>();
            gameObject.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            EffectComponent effectComponent = gameObject.AddComponent<EffectComponent>();
            effectComponent.applyScale = false;
            effectComponent.effectIndex = EffectIndex.Invalid;
            effectComponent.parentToReferencedTransform = parentToTransform;
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.soundName = soundName;
            AddNewEffectDef(gameObject, soundName);
            return gameObject;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef effectDef = new EffectDef();
            effectDef.prefab = effectPrefab;
            effectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            effectDef.prefabName = effectPrefab.name;
            effectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            effectDef.spawnSoundEventName = soundName;
            effectDefs.Add(effectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat)
            {
                commandoMat = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponentInChildren<CharacterModel>()
                    .baseRendererInfos[0].defaultMaterial;
            }
            Material material = Object.Instantiate(commandoMat);
            Material material2 = mainAssetBundle.LoadAsset<Material>(materialName);
            if (!material2)
            {
                return commandoMat;
            }
            material.name = materialName;
            material.SetColor("_Color", material2.GetColor("_Color"));
            material.SetTexture("_MainTex", material2.GetTexture("_MainTex"));
            material.SetColor("_EmColor", emissionColor);
            material.SetFloat("_EmPower", emission);
            material.SetTexture("_EmTex", material2.GetTexture("_EmissionMap"));
            material.SetFloat("_NormalStrength", normalStrength);
            return material;
        }

        public static Material CreateMaterial(string materialName)
        {
            return CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return CreateMaterial(materialName, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}
