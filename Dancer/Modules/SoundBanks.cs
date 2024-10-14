using RoR2;
using UnityEngine;
using Path = System.IO.Path;
using MonoMod.RuntimeDetour;
using System;
using R2API.Utils;
namespace Dancer
{
    public static class SoundBanks
    {
        public static string soundBankDirectory
        {
            get
            {
                return Path.GetDirectoryName(DancerPlugin.instance.Info.Location);
            }
        }
        public static void Init()
        {
            var hook = new Hook(
            typeof(AkSoundEngineInitialization).GetMethodCached(nameof(AkSoundEngineInitialization.InitializeSoundEngine)),
            typeof(SoundBanks).GetMethodCached(nameof(AddBanks)));
        }
        private static bool AddBanks(Func<AkSoundEngineInitialization, bool> orig, AkSoundEngineInitialization self)
        {
            var res = orig(self);
            LoadBanks();
            return res;
        }

        private static void LoadBanks()
        {
            AkSoundEngine.AddBasePath(soundBankDirectory);
            AkSoundEngine.LoadBank("DancerBank", /*-1,*/ out var bank);
            AkSoundEngine.LoadBank("RidleyBank", /*-1,*/ out var bitch);
        }

    }
}