using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMetaScaling
{
    public static class PConfig
    {
        private static string banSection = "Ban rules";

        public static void DoBinds(ConfigFile configFile)
        {
            banLevel = configFile.Bind(banSection, "BanLevel", BanLevel.NonPooledAndCopiesAllowed, "Ban strictness.");
        }

        static ConfigEntry<BanLevel> banLevel;


        public static BanLevel BanLevel => banLevel.Value;

        //public static BanLevel BanLevel => BanLevel.NonPooledAndCopiesAllowed;


    }

    public enum BanLevel
    {
        NonPooledAndCopiesAllowed,
        OnlyCopiesAllowed,
        Strict
    }
}
