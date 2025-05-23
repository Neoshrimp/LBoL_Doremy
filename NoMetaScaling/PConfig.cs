﻿using BepInEx.Configuration;
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
            banLevel = configFile.Bind(banSection, "BanLevel", BanLevel.RealCopiesAllowed, "Ban strictness. Copies are considered generated cards and thus banned unless BanLevel option is `RealCopiesAllowed`.");

            allowFirstTimeDeckedGen = configFile.Bind(banSection, "AllowFirstTimeDeckedGen", true, "Cards generated by legal, decked card will be legal when that card performs generation for the first time in combat.");

            morePissResources = configFile.Bind(banSection, "MoreClownpieceResources", false, "Should second wave of Clownpiece summons drop P and money?");


        }

        static ConfigEntry<BanLevel> banLevel;

        static ConfigEntry<bool> allowFirstTimeDeckedGen;

        static ConfigEntry<bool> morePissResources;



        public static BanLevel BanLevel => banLevel.Value;

        public static bool AllowFirstTimeDeckedGen => allowFirstTimeDeckedGen.Value;

        public static bool MorePissResources => morePissResources.Value;


        //public static BanLevel BanLevel => BanLevel.NonPooledAndCopiesAllowed;



    }

    public enum BanLevel
    {
        //NonPooledAndCopiesAllowed,
        RealCopiesAllowed,
        Strict
    }
}
