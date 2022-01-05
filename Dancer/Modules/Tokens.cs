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

            string  outro = "..and so";
            string  outroFailure = "..and so";

            LanguageAPI.Add(prefix + "NAME", "Dancer");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "idk");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Primary");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION",  $"Melee attack based on aim inputs.");

            LanguageAPI.Add(prefix + "PRIMARY_SLASH2_NAME", "Primary2");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH2_DESCRIPTION", $"Simpler melee attacks based on aim inputs. (gameplay is designed around regular primary but u can use this if u want)");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SLASH_NAME", "Secondary");
            LanguageAPI.Add(prefix + "SECONDARY_SLASH_DESCRIPTION", $"Dash in a target direction and strike <style=cIsDamage>3</style> times, each dealing <style=cIsDamage>{250}% damage</style>");

            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_PULL_NAME", "Utility");
            LanguageAPI.Add(prefix + "UTILITY_PULL_DESCRIPTION", $"Extend your lance in a target direction, dealing <style=cIsDamage>{375}% damage to all enemies hit, </style> <style=cIsUtility>pulling them and yourself to the tip.</style>");

            LanguageAPI.Add(prefix + "UTILITY_PULL2_NAME", "UtilityForMultiplayerBecauseIHateNetworking");
            LanguageAPI.Add(prefix + "UTILITY_PULL2_DESCRIPTION", $"Extend your lance in a target direction, dealing <style=cIsDamage>{500}% damage to an enemy, </style> <style=cIsUtility>pulling yourself to them.</style> (ideal in multiplayer if you arent the host)");

            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_NAME", "Special");
            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_DESCRIPTION", $"Fire a ribbon that leaves enemies </style> <style=cIsUtility>unable to act.</style> Attacking ribboned enemies <style=cIsHealing>heals you </style>and <style=cIsDamage>extends the ribbon.</style>");

            #endregion

        }
    }
}
