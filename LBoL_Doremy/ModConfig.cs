using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy
{
    public static class ModConfig
    {
        private static string gameplaySection = "Gameplay";

        public static void DoBinds(ConfigFile configFile)
        {
            autoSaveAfterLucentGenesis = configFile.Bind(gameplaySection, "AutoSaveAfterLucentGenesis", true, "Save Power committed to use Lucent Genesis to prevent save scumming for favorable outcome.");
        }

        static ConfigEntry<bool> autoSaveAfterLucentGenesis;

        public static bool AutoSaveAfterLucentGenesis => autoSaveAfterLucentGenesis.Value;

    }
}
