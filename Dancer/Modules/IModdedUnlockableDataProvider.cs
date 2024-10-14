using System;
using UnityEngine;

namespace Dancer.Modules
{

    internal interface IModdedUnlockableDataProvider
    {
        string AchievementIdentifier { get; }

        string UnlockableIdentifier { get; }

        string AchievementNameToken { get; }

        string PrerequisiteUnlockableIdentifier { get; }

        string UnlockableNameToken { get; }

        string AchievementDescToken { get; }

        Sprite Sprite { get; }

        Func<string> GetHowToUnlock { get; }

        Func<string> GetUnlocked { get; }
    }
}
