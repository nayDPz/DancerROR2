using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
namespace Dancer.Modules
{
    public class nothowconfigswork
    {

        public static float primaryAimUpAngle = 0.575f;
        public static float primaryAimDownAngle = -0.425f;
        public static float primaryAimDownAirAngle = -0.74f;

        public static float jab1DamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Jab 1 Damage"), 2.5f, new ConfigDescription("Damage coefficient for Jab 1")).Value;
        public static float jab2Damagecoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Jab 2 Damage"), 2.5f, new ConfigDescription("Damage coefficient for Jab 2")).Value;
        public static float dashAttackDamagecoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Dash Attack Damage"), 3.5f, new ConfigDescription("Damage coefficient for Dash Attack")).Value;
        public static float downTiltDamagecoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Down Tilt Damage"), 2.5f, new ConfigDescription("Damage coefficient for Down Tilt")).Value;
        public static float downAirDamagecoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Down Air Damage"), 1.6f, new ConfigDescription("Damage coefficient for Down AIr")).Value;
        public static float forwardAirDamagecoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Forward Air Damage"), 2.5f, new ConfigDescription("Damage coefficient for Forward Air")).Value;
        public static float upAir1Damagecoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Up Air 1 Damage"), 2f, new ConfigDescription("Damage coefficient for Up Air 1")).Value;
        public static float upAir2Damagecoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Up Air 2 Damage"), 3f, new ConfigDescription("Damage coefficient for Up Air 2")).Value;

        public static float forwardAirSpikeDamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Primary", "Spike Damage"), 2f, new ConfigDescription("Maximum damage coefficient for when spiked enemies hit the ground.")).Value;


        public static float directionalForwardDamageCoefficient = 2f;
        public static float directionalBackDamageCoefficient = 2f;
        public static float directionalRightDamageCoefficient = 2f;
        public static float directionalLeftDamageCoefficient = 2f;
        public static float directionalSprintDamageCoefficient = 2f;
        public static float directionalJumpDamageCoefficient = 2f;
        public static float directionalDownDamageCoefficient = 2f;


        public static float parryPerfectDamageCoefficient = 24f;
        public static float parryMaxDamageCoefficient = 18f;
        public static float parryMinDamageCoefficient = 12f;
        public static float perfectParryTime = 0.25f;
        public static float parryInvincibilityDuration = 1.5f;

        public static float secondaryDamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Secondary", "Secondary Damage"), 3.5f, new ConfigDescription("Damage coefficient for Secondary")).Value;


        public static float dragonLungeDamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Utility", "Utility (Pull) Damage"), 6.5f, new ConfigDescription("Damage coefficient for Utility (Pull)")).Value;
        public static float dragonLungeRange = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Utility", "Utility (Pull) Range"), 70f, new ConfigDescription("Range for Utility (Pull)")).Value;

        public static float spinDashDamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Utility", "Utility (Drill) Damage"), 5.4f, new ConfigDescription("Total damage coefficient for Utility (Drill)")).Value;

        public static float ribbonDuration = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Duration"), 8f, new ConfigDescription("Ribbon duration for Special")).Value;
        public static float ribbonDotDamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Bleed Damage"), .5f, new ConfigDescription("Damage coefficient for ribbon bleed")).Value;
        public static float ribbonDamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Impact Damage"), .5f, new ConfigDescription("Damage coefficient for ribbon impact")).Value;
        public static float ribbonChainDamageCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Chain Damage"), 0f, new ConfigDescription("Percent of damage to be shared between ribboned enemies (VALUES THAT AREN'T ZERO ARE VERY UNSTABLE, EDIT AT YOUR OWN RISK)")).Value;

        public static float ribbonChainTime = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Spread Time"), .25f, new ConfigDescription("Time it takes for ribbons to spread to a new enemy")).Value;
        public static float ribbonBarrierFraction = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Barrier Fraction"), .03f, new ConfigDescription("Percent of maximum health to grant as barrier when attacking ribboned enemies")).Value;
        public static float ribbonSpreadRange = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Spread Range"), .03f, new ConfigDescription("Radius in which ribboned enemies search for new targets to spread to")).Value;
        public static int ribbonInitialTargets = DancerPlugin.instance.Config.Bind<int>(new ConfigDefinition("Special", "Special Initial Extra Targets"), 2, new ConfigDescription("Number of enemies (beyond the first) that ribbons will instantly spread to.")).Value;
        public static float ribbonPullForce = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Pull Force"), 1000f, new ConfigDescription("Strength of the pull when the ribbons attach to an enemy")).Value;
        public static float ribbonMovespeedCoefficient = DancerPlugin.instance.Config.Bind<float>(new ConfigDefinition("Special", "Special Slow"), 0.75f, new ConfigDescription("Multiplier for enemy movespeed while under the effects of ribbons")).Value;

        public static float ribbonNaturalChainTime = 2f;
        public static bool ribbonNaturalSpread = false;
    }
}
