using RoR2;
using System;
using UnityEngine;

namespace Dancer.Modules
{

    internal static class Skins
    {
        internal struct SkinDefInfo
        {
            internal SkinDef[] BaseSkins;

            internal Sprite Icon;

            internal string NameToken;

            internal UnlockableDef UnlockableDef;

            internal GameObject RootObject;

            internal CharacterModel.RendererInfo[] RendererInfos;

            internal SkinDef.MeshReplacement[] MeshReplacements;

            internal SkinDef.GameObjectActivation[] GameObjectActivations;

            internal SkinDef.ProjectileGhostReplacement[] ProjectileGhostReplacements;

            internal SkinDef.MinionSkinReplacement[] MinionSkinReplacements;

            internal string Name;
        }

        internal static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root)
        {
            return CreateSkinDef(skinName, skinIcon, rendererInfos, mainRenderer, root, null);
        }

        internal static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root, UnlockableDef unlockableDef)
        {
            SkinDefInfo skinDefInfo = default(SkinDefInfo);
            skinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            skinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            skinDefInfo.Icon = skinIcon;
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[0];
            skinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            skinDefInfo.Name = skinName;
            skinDefInfo.NameToken = skinName;
            skinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            skinDefInfo.RendererInfos = rendererInfos;
            skinDefInfo.RootObject = root;
            skinDefInfo.UnlockableDef = unlockableDef;
            SkinDefInfo skinDefInfo2 = skinDefInfo;
            On.RoR2.SkinDef.Awake += DoNothing;
            SkinDef skinDef = ScriptableObject.CreateInstance<SkinDef>();
            skinDef.baseSkins = skinDefInfo2.BaseSkins;
            skinDef.icon = skinDefInfo2.Icon;
            skinDef.unlockableDef = skinDefInfo2.UnlockableDef;
            skinDef.rootObject = skinDefInfo2.RootObject;
            skinDef.rendererInfos = skinDefInfo2.RendererInfos;
            skinDef.gameObjectActivations = skinDefInfo2.GameObjectActivations;
            skinDef.meshReplacements = skinDefInfo2.MeshReplacements;
            skinDef.projectileGhostReplacements = skinDefInfo2.ProjectileGhostReplacements;
            skinDef.minionSkinReplacements = skinDefInfo2.MinionSkinReplacements;
            skinDef.nameToken = skinDefInfo2.NameToken;
            skinDef.name = skinDefInfo2.Name;
            On.RoR2.SkinDef.Awake -= DoNothing;
            return skinDef;
        }

        private static void DoNothing(On.RoR2.SkinDef.orig_Awake orig, RoR2.SkinDef self)
        {
        }
    }
}
