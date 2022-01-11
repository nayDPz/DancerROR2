﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;

namespace Dancer.Modules
{
    public static class Buffs
    {
        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static BuffDef ribbonDebuff;
        public static DotController.DotIndex ribbonDotIndex;


        internal static float ribbonDebuffDuration = 8f;
        internal static float ribbonSpreadRange = 50f;
        internal static float ribbonDotCoefficient = 1f;

        internal static void RegisterBuffs()
        {
            ribbonDebuffDuration = StaticValues.ribbonDuration;
            ribbonDotCoefficient = StaticValues.ribbonDamageCoefficient;
            ribbonDebuff = AddNewBuff("RibbonDebuff", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.magenta, false, true);
            ribbonDotIndex = DotAPI.RegisterDotDef(1f, ribbonDotCoefficient, DamageColorIndex.Bleed, ribbonDebuff, null, null);
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