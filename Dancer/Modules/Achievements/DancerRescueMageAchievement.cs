using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Achievements;
using R2API;
using UnityEngine;

namespace Dancer.Modules.Achievements
{
    class DancerRescueMageAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier => "DANCER_RESCUEMAGE_ACHIEVEMENT_ID";

        public override string UnlockableIdentifier => "DANCER_RESCUEMAGE_REWARD_ID";

        public override string AchievementNameToken => "DANCER_RESCUEMAGE_ACHIEVEMENT_NAME";

        public override string PrerequisiteUnlockableIdentifier => "";

        public override string UnlockableNameToken => "DANCER_RESCUEMAGE_UNLOCKABLE_NAME";

        public override string AchievementDescToken => "DANCER_RESCUEMAGE_ACHIEVEMENT_DESC";

        public override Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_NAME"),
                                Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_DESC")
                            }));

        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_NAME"),
                                Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("DancerBody");
        }

        public override void OnInstall()
        {
            base.OnInstall();
            Run.onClientGameOverGlobal += OnClientGameOverGlobal;
        }

        public override void OnUninstall()
        {

            base.OnGranted();
            Run.onClientGameOverGlobal -= this.OnClientGameOverGlobal;
        }

        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (!runReport.gameEnding)
            {
                return;
            }
            if (runReport.gameEnding == GameEndingCatalog.FindGameEndingDef("MainEnding"))
            {
                if (Modules.Components.LockedMageTracker.mageFreed)
                    base.Grant();
            }
        }
    
    }
}
