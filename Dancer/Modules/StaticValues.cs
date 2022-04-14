using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
namespace Dancer.Modules
{
    public class StaticValues
    {
        public const float primaryAimUpAngle = 0.575f;
        public const float primaryAimDownAngle = -0.425f;
        public const float primaryAimDownAirAngle = -0.74f;

        public const float jab1DamageCoefficient = 2.5f;
        public const float jab2DamageCoefficient = 2.5f;
        public const float dashAttackDamageCoefficient = 3.5f;
        public const float downTiltDamageCoefficient = 2.5f;
        public const float downAirDamageCoefficient = 1.6f;
        public const float forwardAirDamageCoefficient = 2.5f;
        public const float upAir1DamageCoefficient = 2f;
        public const float upAir2DamageCoefficient = 3f;

        public const float forwardAirSpikeDamageCoefficient = 4f;


        public const float directionalForwardDamageCoefficient = 2f;
        public const float directionalBackDamageCoefficient = 2f;
        public const float directionalRightDamageCoefficient = 2f;
        public const float directionalLeftDamageCoefficient = 2f;
        public const float directionalSprintDamageCoefficient = 2f;
        public const float directionalJumpDamageCoefficient = 2f;
        public const float directionalDownDamageCoefficient = 2f;


        public const float parryPerfectDamageCoefficient = 24f;
        public const float parryMaxDamageCoefficient = 18f;
        public const float parryMinDamageCoefficient = 12f;
        public const float perfectParryTime = 0.25f;
        public const float parryInvincibilityDuration = 1.5f;

        public const float secondaryDamageCoefficient = 3.5f;
        

        public const float dragonLungeDamageCoefficient = 6.5f;
        public const float dragonLungeRange = 70f;

        public const float spinDashDamageCoefficient = 5.4f;

        public const float ribbonDuration = 8f;
        public const float ribbonDotDamageCoefficient = 0.0f;
        public const float ribbonDamageCoefficient = 0.75f;
        public const float ribbonChainDamageCoefficient = 0f;
        
        public const float ribbonChainTime = 0.25f;
        public const float ribbonBarrierFraction = 0.04f;
        public const float ribbonSpreadRange = 50f;
        public const int ribbonInitialTargets = 2;
        public const float ribbonPullForce = 800f;
        public const float ribbonMovespeedCoefficient = 0.75f;

        public const float ribbonNaturalChainTime = 2f;
        public const bool ribbonNaturalSpread = false;
    }
}
