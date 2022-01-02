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
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION",  $"melee attack based on aim inputs");

            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SLASH_NAME", "Secondary");
            LanguageAPI.Add(prefix + "SECONDARY_SLASH_DESCRIPTION", $"hit 3 times");

            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_PULL_NAME", "Utility");
            LanguageAPI.Add(prefix + "UTILITY_PULL_DESCRIPTION", $"extend weapon. pulls u and enemies to where it hits the ground");

            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_NAME", "Special");
            LanguageAPI.Add(prefix + "SPECIAL_RIBBON_DESCRIPTION", $"shoot ribbon that holds enemy in place. attacking ribboned targets extends the ribbon");

            #endregion

        }
    }
}
