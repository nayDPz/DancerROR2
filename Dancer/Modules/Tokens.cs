using R2API;
using System;

namespace Dancer.Modules
{

    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string text = "NDP_DANCER_BODY_";
            string text2 = "lole<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            string text3 = "..and so you beat the game with a borken unfinished character ur not even good lol i havent finished the lore because you dont deserver it";
            string text4 = "..and so";
            LanguageAPI.Add(text + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(text + "NAME", "Dancer");
            LanguageAPI.Add(text + "DESCRIPTION", text2);
            LanguageAPI.Add(text + "SUBTITLE", "idk");
            LanguageAPI.Add(text + "LORE", "sample lore");
            LanguageAPI.Add(text + "OUTRO_FLAVOR", text3);
            LanguageAPI.Add(text + "OUTRO_FAILURE", text4);
            LanguageAPI.Add("KEYWORD_DANCER_CANCELS", "Sprinting <style=cIsUtility>cancels</style> Jabs into a Dash Attack. Jumping <style=cIsUtility>cancels</style> ground moves into aerials.");
            LanguageAPI.Add("KEYWORD_DANCER_JAB", $"<style=cKeywordName>Jab</style> <style=cIsUtility>Input: ground, aim forward.</style> <style=cSub>Dash forward and deal <style=cIsDamage>{180f}%</style> damage");
            LanguageAPI.Add("KEYWORD_DANCER_DASH", $"<style=cKeywordName>Dash Attack</style> <style=cIsUtility>Input: ground, sprinting.</style> <style=cSub>Dash forward, interrupt enemies and deal <style=cIsDamage>{250f}%</style> damage.");
            LanguageAPI.Add("KEYWORD_DANCER_DOWNTILT", $"<style=cKeywordName>Down Tilt</style> <style=cIsUtility>Input: ground, aim down.</style> <style=cSub>Knock enemies upwards and deal <style=cIsDamage>{200f}%</style> damage.</style>");
            LanguageAPI.Add("KEYWORD_DANCER_UPAIR", $"<style=cKeywordName>Up Air</style> <style=cIsUtility>Input: aim up.</style> <style=cSub>Two attacks that deal <style=cIsDamage>{150f}%</style> and <style=cIsDamage>{200f}%</style> damage. Second attack is narrow and deals up to <style=cIsDamage>3x</style> damage to low health enemies.</style>");
            LanguageAPI.Add("KEYWORD_DANCER_FORWARDAIR", $"<style=cKeywordName>Forward Air</style> <style=cIsUtility>Input: air, aim forward.</style> <style=cSub>Heavy swing that deals <style=cIsDamage>{180f}%</style> damage and spikes enemies to the ground, <style=cIsDamage>stunning</style> and dealing up to <style=cIsDamage>{300f}%</style> damage</style>");
            LanguageAPI.Add("KEYWORD_DANCER_DOWNAIR", $"<style=cKeywordName>Down Air</style> <style=cIsUtility>Input: air, aim down.</style> <style=cSub>Dive downwards and repeatedly deal <style=cIsDamage>{125f}%</style> damage. If you hit the ground, start a <style=cIsDamage>Down Tilt</style></style>");
            LanguageAPI.Add(text + "PRIMARY_SLASH_NAME", "Primary (Aim Inputs)");
            LanguageAPI.Add(text + "PRIMARY_SLASH_DESCRIPTION", "<style=cIsDamage>Melee attack</style> with effects based on your aim input. <style=cIsUtility>(Hover for more details)</style>");
            LanguageAPI.Add("KEYWORD_DANCER_INPUTS", "<style=cKeywordName>Inputs</style> <style=cSub>Movement Direction, Sprint, and Jump have input attacks. Some inputs have different attacks if pressed repeatedly. ill make a better explanation soon sry.");
            LanguageAPI.Add(text + "PRIMARY_SLASH2_NAME", "Primary (Movement Inputs)");
            LanguageAPI.Add(text + "PRIMARY_SLASH2_DESCRIPTION", "Hold to turn your <style=cIsUtility>movement inputs</style> into <style=cIsDamage>melee attacks</style>. <style=cIsUtility>(Hover for more details)</style>");
            LanguageAPI.Add(text + "SECONDARY_SLASH_NAME", "Secondary");
            LanguageAPI.Add(text + "SECONDARY_SLASH_DESCRIPTION", $"Dash in a target direction and strike <style=cIsDamage>3</style> times, each dealing <style=cIsDamage>{250f}% damage</style>.");
            LanguageAPI.Add(text + "UTILITY_PULL_NAME", "Utility (Pull)");
            LanguageAPI.Add(text + "UTILITY_PULL_DESCRIPTION", $"Extend your lance, dealing <style=cIsDamage>{550f}% damage</style>. Hold to <style=cIsUtility>pull</style> yourself and enemies to the tip. <style=cIsUtility>Jump</style> to exit early");
            LanguageAPI.Add(text + "UTILITY_DRILL_NAME", "Utility (Drill)");
            LanguageAPI.Add(text + "UTILITY_DRILL_DESCRIPTION", $"Lunge horizontally, dealing <style=cIsDamage>3x{180f}% damage</style>, then start a <style=cIsDamage>Down Tilt.</style> Can store up two <style=cIsUtility>2</style> charges.");
            LanguageAPI.Add("KEYWORD_DANCER_RIBBON", $"<color=#611221><style=cKeywordName>Ribbon</style></color><style=cSub>Ribboned enemies bleed for <style=cIsDamage>{0f}%</style> damage per second and are <style=cIsDamage>unable to act.</style>");
            LanguageAPI.Add(text + "SPECIAL_RIBBON_NAME", "Special");
            LanguageAPI.Add(text + "SPECIAL_RIBBON_DESCRIPTION", $"<style=cIsDamage>Stunning.</style> Fire a projectile that <color=#b50727>ribbons</color> enemies for <style=cIsDamage>{8f}</style> seconds. Attacking <color=#b50727>ribboned</color> enemies grants a <style=cIsHealing>temporary barrier</style> and <style=cIsDamage>extends</style> the ribbon.");
            LanguageAPI.Add(text + "SPECIAL_RIBBON_LOCK_NAME", "Ribboned");
            LanguageAPI.Add(text + "SPECIAL_RIBBON_LOCK_DESCRIPTION", "You cannot use this skill.");
            LanguageAPI.Add(text + "DANCER_RESCUEMAGE_ACHIEVEMENT_NAME", "Dancer: Rescue Mission");
            LanguageAPI.Add(text + "DANCER_RESCUEMAGE_ACHIEVEMENT_DESC", "As Dancer, successfully recover the lost ENV Suit");
            LanguageAPI.Add(text + "DANCER_RESCUEMAGE_UNLOCKABLE_NAME", "Dancer: Rescue Mission");
        }
    }
}
