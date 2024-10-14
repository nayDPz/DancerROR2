using Dancer.SkillStates;
using Dancer.SkillStates.Emotes;
using Dancer.SkillStates.InterruptStates;
using Dancer.SkillStates.M1;
using EntityStates;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dancer.Modules
{

    public static class States
    {
        private delegate void set_stateTypeDelegate(ref SerializableEntityStateType self, Type value);

        private delegate void set_typeNameDelegate(ref SerializableEntityStateType self, string value);

        internal static List<Type> entityStates = new List<Type>();

        private static Hook set_stateTypeHook;

        private static Hook set_typeNameHook;

        private static readonly BindingFlags allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }

        internal static void RegisterStates()
        {
            Type typeFromHandle = typeof(SerializableEntityStateType);
            HookConfig val = default(HookConfig);
            val.Priority = int.MinValue;
            set_stateTypeHook = new Hook((MethodBase)typeFromHandle.GetMethod("set_stateType", allFlags), (Delegate)new set_stateTypeDelegate(SetStateTypeHook), val);
            set_typeNameHook = new Hook((MethodBase)typeFromHandle.GetMethod("set_typeName", allFlags), (Delegate)new set_typeNameDelegate(SetTypeName), val);
            AddSkill(typeof(SuspendedState));
            AddSkill(typeof(SkeweredState));
            AddSkill(typeof(RibbonedState));
            AddSkill(typeof(SpikedState));
            AddSkill(typeof(DragonLunge));
            AddSkill(typeof(DragonLunge));
            AddSkill(typeof(DragonLungeButEpic));
            AddSkill(typeof(Pull));
            AddSkill(typeof(Pull2));
            AddSkill(typeof(PullDamage));
            AddSkill(typeof(SpinnyMove));
            AddSkill(typeof(FireChainRibbons));
            AddSkill(typeof(BaseM1));
            AddSkill(typeof(M1Entry));
            AddSkill(typeof(Jab1));
            AddSkill(typeof(Jab2));
            AddSkill(typeof(DashAttack));
            AddSkill(typeof(DownTilt));
            AddSkill(typeof(DownAir));
            AddSkill(typeof(DownAirLand));
            AddSkill(typeof(FAir));
            AddSkill(typeof(UpAir));
            AddSkill(typeof(BaseEmote));
        }

        private static void SetStateTypeHook(this ref SerializableEntityStateType self, Type value)
        {
            self._typeName = value.AssemblyQualifiedName;
        }

        private static void SetTypeName(this ref SerializableEntityStateType self, string value)
        {
            Type typeFromName = GetTypeFromName(value);
            if (typeFromName != null)
            {
                self.SetStateTypeHook(typeFromName);
            }
        }

        private static Type GetTypeFromName(string name)
        {
            Type[] stateIndexToType = EntityStateCatalog.stateIndexToType;
            return Type.GetType(name);
        }
    }
}
