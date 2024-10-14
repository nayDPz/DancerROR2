using BepInEx.Configuration;

namespace Dancer.Modules
{

    public static class Config
    {
        public static ConfigEntry<bool> artiBuddy;

        public static void ReadConfig()
        {
            artiBuddy = DancerPlugin.instance.Config.Bind<bool>("Artificer Ally", "Enabled", true, "Enables a purchasable Artificer ally in the Bazaar");
        }
    }
}
