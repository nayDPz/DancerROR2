using Dancer.SkillStates;
using Dancer.SkillStates.Emotes;
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
        internal static List<Type> entityStates = new List<Type>();

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }

        internal static void RegisterStates()
        {           
            #region DirectionalM1
            AddSkill(typeof(SkillStates.DirectionalM1.AttackBackwards));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackDown));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackDown2));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackDown2End));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackForward));
            //AddSkill(typeof(SkillStates.DirectionalM1.AttackForward2)); // unused
            AddSkill(typeof(SkillStates.DirectionalM1.AttackJump));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackJump2));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackLeft));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackRight));
            AddSkill(typeof(SkillStates.DirectionalM1.AttackSprint));
            //AddSkill(typeof(SkillStates.DirectionalM1.BaseDirectionalM1)); // base state, no need to register
            //AddSkill(typeof(SkillStates.DirectionalM1.BaseInputEvaluation)); // base state, no need to register
            AddSkill(typeof(SkillStates.DirectionalM1.EnterDirectionalAttack));
            #endregion
            #region M1
            //AddSkill(typeof(SkillStates.M1.BaseM1)); // base state, no need to register
            AddSkill(typeof(SkillStates.M1.DashAttack));
            AddSkill(typeof(SkillStates.M1.DownAir));
            AddSkill(typeof(SkillStates.M1.DownAirLand));
            //AddSkill(typeof(SkillStates.M1.DownTilt)); // unused
            AddSkill(typeof(SkillStates.M1.FAir));
            AddSkill(typeof(SkillStates.M1.Jab1));
            AddSkill(typeof(SkillStates.M1.Jab2));
            AddSkill(typeof(SkillStates.M1.M1Entry));
            AddSkill(typeof(SkillStates.M1.UpAir));
            #endregion
            #region InterruptStates
            //AddSkill(typeof(SkillStates.InterruptStates.RibbonedState)); // unused
            AddSkill(typeof(SkillStates.InterruptStates.SkeweredState));
            AddSkill(typeof(SkillStates.InterruptStates.SpikedState));
            AddSkill(typeof(SkillStates.InterruptStates.SuspendedState));
            #endregion

            //AddSkill(typeof(ChargeParry)); // doesn't work
            AddSkill(typeof(DragonLunge));
            //AddSkill(typeof(DragonLungeButEpic)); // unused
            AddSkill(typeof(FireChainRibbons));
            AddSkill(typeof(Pull));
            AddSkill(typeof(Pull2));
            AddSkill(typeof(PullDamage));
            //AddSkill(typeof(Riposte)); // doesn't work
            AddSkill(typeof(SpinDash));
            AddSkill(typeof(SpinDashEnd));
            AddSkill(typeof(SpinnyMove));
            //AddSkill(typeof(BaseEmote)); // no emotes are added
        }
    }
}
