using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Dancer.Modules
{

    public static class Buffs
    {
        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static BuffDef ribbonDebuff;

        internal static BuffDef parryBuff;

        public static DotController.DotIndex ribbonDotIndex;

        internal static float ribbonDebuffDuration = 8f;

        internal static float ribbonBossCCDuration = 4f;

        internal static float ribbonDotCoefficient = 1f;

        internal static void RegisterBuffs()
        {
            ribbonDebuffDuration = 8f;
            ribbonDotCoefficient = 0f;
            ribbonDebuff = AddNewBuff("RibbonDebuff", Assets.mainAssetBundle.LoadAsset<Sprite>("texRibbonDebuffIcon"), Color.magenta, canStack: false, isDebuff: true);
            parryBuff = AddNewBuff("ParryBuff", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.white, canStack: false, isDebuff: false);
        }

        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDefs.Add(buffDef);
            return buffDef;
        }
    }
}
