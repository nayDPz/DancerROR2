using BepInEx.Configuration;
using UnityEngine;

namespace Dancer.Modules
{
    public static class Config
    {
        public static ConfigEntry<KeyCode> standKeybind;
        public static ConfigEntry<KeyCode> emote1Keybind;
        public static ConfigEntry<KeyCode> emote2Keybind;
        public static void ReadConfig()
        {
            
            emote1Keybind = DancerPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Emote1"), KeyCode.Alpha2, new ConfigDescription("Keybind used to Emote1"));
            emote2Keybind = DancerPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Emote1"), KeyCode.Alpha3, new ConfigDescription("Keybind used to Emote2"));
            standKeybind = DancerPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Stand"), KeyCode.Alpha1, new ConfigDescription("Keybind used to fucking destroy their ego"));
        }

        // this helper automatically makes config entries for disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return DancerPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this character"));
        }

        internal static ConfigEntry<bool> EnemyEnableConfig(string characterName)
        {
            return DancerPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this enemy"));
        }
    }
}