using System;
using R2API;
using Dancer.SkillStates;

namespace Dancer.Modules
{
	// Token: 0x02000026 RID: 38
	internal static class Tokens
	{
		// Token: 0x060000BE RID: 190 RVA: 0x0000908C File Offset: 0x0000728C
		internal static void AddTokens()
		{
            string prefix = DancerPlugin.developerPrefix + "_DANCER_BODY_";

            string  desc = "lole<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;

            string  outro = "..and so you beat the game with a borken unfinished character ur not even good lol i havent finished the lore because you dont deserver it";
            string  outroFailure = "..and so";

            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");

            LanguageAPI.Add(prefix + "NAME", "Dancer");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "idk");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Primary
            LanguageAPI.Add("KEYWORD_DANCER_CANCELS", $"Sprinting <style=cIsUtility>cancels</style> Jabs into a Dash Attack. Jumping <style=cIsUtility>cancels</style> ground moves into aerials.");
            LanguageAPI.Add("KEYWORD_DANCER_JAB", $"<style=cKeywordName>Jab</style> <style=cIsUtility>Input: ground, aim forward.</style> <style=cSub>Dash forward and deal <style=cIsDamage>{StaticValues.jab1DamageCoefficient * 100}%</style> damage");
            LanguageAPI.Add("KEYWORD_DANCER_DASH", $"<style=cKeywordName>Dash Attack</style> <style=cIsUtility>Input: ground, sprinting.</style> <style=cSub>Dash forward, interrupt enemies and deal <style=cIsDamage>{StaticValues.dashAttackDamageCoefficient * 100}%</style> damage.");
            LanguageAPI.Add("KEYWORD_DANCER_DOWNTILT", $"<style=cKeywordName>Down Tilt</style> <style=cIsUtility>Input: ground, aim down.</style> <style=cSub>Knock enemies upwards and deal <style=cIsDamage>{StaticValues.downTiltDamageCoefficient * 100}%</style> damage.</style>");
            LanguageAPI.Add("KEYWORD_DANCER_UPAIR", $"<style=cKeywordName>Up Air</style> <style=cIsUtility>Input: aim up.</style> <style=cSub>Two attacks that deal <style=cIsDamage>{StaticValues.upAir1DamageCoefficient * 100}%</style> and <style=cIsDamage>{StaticValues.upAir2DamageCoefficient * 100}%</style> damage. Second attack is narrow and deals up to <style=cIsDamage>3x</style> damage to low health enemies.</style>");
            LanguageAPI.Add("KEYWORD_DANCER_FORWARDAIR", $"<style=cKeywordName>Forward Air</style> <style=cIsUtility>Input: air, aim forward.</style> <style=cSub>Heavy swing that deals <style=cIsDamage>{StaticValues.forwardAirDamageCoefficient * 100}%</style> damage and spikes enemies to the ground, <style=cIsDamage>stunning</style> and dealing up to <style=cIsDamage>{StaticValues.forwardAirSpikeDamageCoefficient * 100}%</style> damage</style>");
            LanguageAPI.Add("KEYWORD_DANCER_DOWNAIR", $"<style=cKeywordName>Down Air</style> <style=cIsUtility>Input: air, aim down.</style> <style=cSub>Dive downwards and repeatedly deal <style=cIsDamage>{StaticValues.downAirDamageCoefficient * 100}%</style> damage. If you hit the ground, start a <style=cIsDamage>Down Tilt</style></style>");
            
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Primary (Aim Inputs)");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION",  $"<style=cIsDamage>Melee attack</style> with effects based on your aim input. <style=cIsUtility>(Hover for more details)</style>");

            LanguageAPI.Add("KEYWORD_DANCER_INPUTS", $"<style=cKeywordName>Inputs</style> <style=cSub>Movement Direction, Sprint, and Jump have input attacks. Some inputs have different attacks if pressed repeatedly. ill make a better explanation soon sry.");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH2_NAME", "Primary (Movement Inputs)");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH2_DESCRIPTION", $"Hold to turn your <style=cIsUtility>movement inputs</style> into <style=cIsDamage>melee attacks</style>. <style=cIsUtility>(Hover for more details)</style>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SLASH_NAME", "Secondary");
            LanguageAPI.Add(prefix + "SECONDARY_SLASH_DESCRIPTION", $"Dash in a target direction and strike <style=cIsDamage>3</style> times, each dealing <style=cIsDamage>{StaticValues.secondaryDamageCoefficient * 100}% damage</style>.");

            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_PULL_NAME", "Utility (Pull)");
            LanguageAPI.Add(prefix + "UTILITY_PULL_DESCRIPTION", $"Extend your lance, dealing <style=cIsDamage>{StaticValues.dragonLungeDamageCoefficient * 100}% damage</style>. Hold to <style=cIsUtility>pull</style> yourself and enemies to the tip. <style=cIsUtility>Jump</style> to exit early");

            LanguageAPI.Add(prefix + "UTILITY_DRILL_NAME", "Utility (Drill)");
            LanguageAPI.Add(prefix + "UTILITY_DRILL_DESCRIPTION", $"Lunge horizontally, dealing <style=cIsDamage>3x{StaticValues.spinDashDamageCoefficient / 3 * 100}% damage</style>, then start a <style=cIsDamage>Down Tilt.</style> Can store up two <style=cIsUtility>2</style> charges.");
            #endregion

            LanguageAPI.Add("KEYWORD_DANCER_RIBBON", $"<color=#611221><style=cKeywordName>Ribbon</style></color><style=cSub>Ribboned enemies bleed for <style=cIsDamage>{StaticValues.ribbonDotDamageCoefficient * 100}%</style> damage per second and are <style=cIsDamage>unable to act.</style>");
            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_NAME", "Special");
            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_DESCRIPTION", $"Fire a projectile that <color=#b50727>ribbons</color> enemies for <style=cIsDamage>{StaticValues.ribbonDuration}</style> seconds. Attacking <color=#b50727>ribboned</color> enemies grants a <style=cIsHealing>temporary barrier</style> and <style=cIsDamage>extends</style> the ribbon.");

            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_LOCK_NAME", "Ribboned");
            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_LOCK_DESCRIPTION", $"You cannot use this skill.");
            #endregion

            #region Achievements

            LanguageAPI.Add(prefix + "DANCER_RESCUEMAGE_ACHIEVEMENT_NAME", "Dancer: Rescue Mission");
            LanguageAPI.Add(prefix + "DANCER_RESCUEMAGE_ACHIEVEMENT_DESC", "As Dancer, successfully recover the lost ENV Suit");

            LanguageAPI.Add(prefix + "DANCER_RESCUEMAGE_UNLOCKABLE_NAME", "Dancer: Rescue Mission");
            #endregion
        }
    }
}
