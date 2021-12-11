using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Ridley.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef armorBuff;
        internal static BuffDef invulChargeStacks;
        internal static BuffDef invulCharge;

        internal static BuffDef vital4;
        internal static BuffDef vital3;
        internal static BuffDef vital2;
        internal static BuffDef vital1;

        internal static BuffDef speedBuff;
        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void RegisterBuffs()
        {
            // fix the buff catalog to actually register our buffs

            invulChargeStacks = AddNewBuff("RidleyStacks", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.gray, true, false);
            invulCharge = AddNewBuff("RidleyInvulnerabilityCharge", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.magenta, false, false);

            armorBuff = AddNewBuff("HenryArmorBuff", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.white, false, false);
            speedBuff = AddNewBuff("speedBuff", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.red, false, false);
            vital1 = AddNewBuff("v1", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.red, false, false);
            vital2 = AddNewBuff("v2", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.red, false, false);
            vital3 = AddNewBuff("v3", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.red, false, false);
            vital4 = AddNewBuff("v4", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.red, false, false);
        }

        // simple helper method
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