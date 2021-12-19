using BepInEx.Configuration;
using UnityEngine;

namespace Ridley.Modules
{
    public static class Config
    {
        public static ConfigEntry<KeyCode> standKeybind;
        public static ConfigEntry<KeyCode> emote1Keybind;
        public static ConfigEntry<KeyCode> emote2Keybind;
        public static void ReadConfig()
        {
            
            emote1Keybind = RidleyPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Emote1"), KeyCode.Alpha2, new ConfigDescription("Keybind used to Emote1"));
            emote2Keybind = RidleyPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Emote1"), KeyCode.Alpha3, new ConfigDescription("Keybind used to Emote2"));
            standKeybind = RidleyPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Stand"), KeyCode.Alpha1, new ConfigDescription("Keybind used to fucking destroy their ego"));
        }

        // this helper automatically makes config entries for disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return RidleyPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this character"));
        }

        internal static ConfigEntry<bool> EnemyEnableConfig(string characterName)
        {
            return RidleyPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this enemy"));
        }
    }
}