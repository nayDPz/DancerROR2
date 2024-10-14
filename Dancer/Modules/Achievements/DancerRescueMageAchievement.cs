using Dancer.Modules.Components;
using RoR2;
using System;
using UnityEngine;

namespace Dancer.Modules.Achievements
{

    internal class DancerRescueMageAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier => "DANCER_RESCUEMAGE_ACHIEVEMENT_ID";

        public override string UnlockableIdentifier => "DANCER_RESCUEMAGE_REWARD_ID";

        public override string AchievementNameToken => "DANCER_RESCUEMAGE_ACHIEVEMENT_NAME";

        public override string PrerequisiteUnlockableIdentifier => "";

        public override string UnlockableNameToken => "DANCER_RESCUEMAGE_UNLOCKABLE_NAME";

        public override string AchievementDescToken => "DANCER_RESCUEMAGE_ACHIEVEMENT_DESC";

        public override Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon");

        public override Func<string> GetHowToUnlock { get; } = () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_NAME"), Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_DESC"));


        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_NAME"), Language.GetString("DANCER_RESCUEMAGE_ACHIEVEMENT_DESC"));


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
            OnGranted();
            Run.onClientGameOverGlobal -= OnClientGameOverGlobal;
        }

        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if ((bool)runReport.gameEnding && runReport.gameEnding == GameEndingCatalog.FindGameEndingDef("MainEnding") && LockedMageTracker.mageFreed)
            {
                Grant();
            }
        }
    }
}
