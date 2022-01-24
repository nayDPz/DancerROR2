using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using RoR2.Audio;
using System.Collections.Generic;
using RoR2.UI;
using Dancer.Modules.Components;
using RoR2.Projectile;
using System.Linq;

namespace Dancer.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        internal static GameObject ribbonController;

        internal static GameObject spikeGroundEffect;
        internal static GameObject crosshairPrefab;

        internal static GameObject mageFriendPrefab;
        // particle effects
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

        // networked hit sounds
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

        // cache these and use to create our own materials
        public static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        public static Shader cloud = Resources.Load<Shader>("shaders/fx/hgcloudremap");
        public static Material commandoMat;

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbedshader/deferred/hgstandard", "shaders/deferred/hgstandard"},
            {"stubbedshader/fx/hgcloudremap", "shaders/fx/hgcloudremap"},
            {"stubbedshader/fx/hgopaquecloudremap", "shaders/fx/hgopaquecloudremap"},
        };
        internal static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dancer.dancerassets"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            ShaderConversion(mainAssetBundle);
            AttachControllerFinderToObjects(mainAssetBundle);

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dancer.RidleyBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dancer.DancerBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

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
            /*
            GameObject effect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/impacteffects/beetleguardgroundslam"), "DancerSpikeEffect");
            ShakeEmitter s = effect.GetComponent<ShakeEmitter>();
            GameObject.Destroy(effect.transform.Find("Spikes, Small"));
            GameObject.Destroy(effect.transform.Find("Spikes, Large"));
            if (s) GameObject.Destroy(s);
            spikeGroundEffect = effect;
            */
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>("DancerDAirEffect");
            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            downAirEffect = newEffect;

            stabHitEffect = LoadEffect("DancerStabHitEffect", true);
            bigHitEffect = LoadEffect("DancerBigHitEffect", true);
            hitEffect = LoadEffect("DancerHitEffect", true);

            downAirEndEffect = LoadEffect("DancerDownAirEndEffect", true);
            dashAttackEffect = LoadEffect("DancerDashAttackEffect", true);
            bigSwingEffect = LoadEffect("DancerBigSwingEffect", true);
            dragonLungeEffect = LoadEffect("DancerDragonLungeEffect", true);
            dragonLungePullEffect = LoadEffect("DancerDragonLungePullEffect", true);
            swingEffect = LoadEffect("DancerSwingEffect", true);

            ribbonController = mainAssetBundle.LoadAsset<GameObject>("RibbonController");
            ribbonController.AddComponent<Modules.Components.RibbonController>();
            ribbonController.RegisterNetworkPrefab();
            networkedObjectPrefabs.Add(ribbonController);




        }

        public static void ShaderConversion(AssetBundle assets)
        {
            var materialAssets = assets.LoadAllAssets<Material>().Where(material => material.shader.name.StartsWith("Stubbed"));
            foreach (Material material in materialAssets)
            {
                //Debug.Log("replacing " + material.shader.name.ToLowerInvariant());
                var replacementShader = Resources.Load<Shader>(ShaderLookup[material.shader.name.ToLowerInvariant()]);
                if (replacementShader) { material.shader = replacementShader; }
            }
        }

        public static void AttachControllerFinderToObjects(AssetBundle assetbundle)
        {
            if (!assetbundle) { return; }

            var gameObjects = assetbundle.LoadAllAssets<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                var foundRenderers = gameObject.GetComponentsInChildren<Renderer>().Where(x => x.sharedMaterial && x.sharedMaterial.shader.name.StartsWith("Hopoo Games"));

                foreach (Renderer renderer in foundRenderers)
                {
                    var controller = renderer.gameObject.AddComponent<Components.MaterialControllerComponents.HGControllerFinder>();
                }
            }

            gameObjects = null;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            /*NetworkSoundEventCatalog.getSoundEventDefs += delegate (List<NetworkSoundEventDef> list)
            {
                list.Add(networkSoundEventDef);
            };*/
            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            foreach (MeshRenderer i in objectToConvert.GetComponentsInChildren<MeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }

            foreach (SkinnedMeshRenderer i in objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            return Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }


        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);
            //EffectAPI.AddEffect(newEffect);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return commandoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return Assets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return Assets.CreateMaterial(materialName, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}