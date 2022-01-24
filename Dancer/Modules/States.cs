using R2API;
using Dancer.SkillStates;
using Dancer.SkillStates.Emotes;
using Dancer.SkillStates;
using System;
using MonoMod.RuntimeDetour;
using EntityStates;
using System.Collections.Generic;
using RoR2;
using System.Reflection;

namespace Dancer.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        private static Hook set_stateTypeHook;
        private static Hook set_typeNameHook;
        private static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
        private delegate void set_stateTypeDelegate(ref SerializableEntityStateType self, Type value);
        private delegate void set_typeNameDelegate(ref SerializableEntityStateType self, String value);

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }

        internal static void RegisterStates()
        {
            Type type = typeof(SerializableEntityStateType);
            HookConfig cfg = default;
            cfg.Priority = Int32.MinValue;
            set_stateTypeHook = new Hook(type.GetMethod("set_stateType", allFlags), new set_stateTypeDelegate(SetStateTypeHook), cfg);
            set_typeNameHook = new Hook(type.GetMethod("set_typeName", allFlags), new set_typeNameDelegate(SetTypeName), cfg);

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

        private static void SetStateTypeHook(ref this SerializableEntityStateType self, Type value)
        {
            self._typeName = value.AssemblyQualifiedName;
        }

        private static void SetTypeName(ref this SerializableEntityStateType self, String value)
        {
            Type t = GetTypeFromName(value);
            if (t != null)
            {
                self.SetStateTypeHook(t);
            }
        }

        private static Type GetTypeFromName(String name)
        {
            Type[] types = EntityStateCatalog.stateIndexToType;
            return Type.GetType(name);
        }
    }
}