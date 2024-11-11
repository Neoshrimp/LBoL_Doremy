using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using NoMetaScalling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoMetaScaling.Core.Trackers
{
    public static class ARTracker
    {
        internal static GameEntity lastActionSource = null;



        [HarmonyPatch(typeof(ActionResolver), nameof(ActionResolver.Resolve))]
        class Resolve_Patch
        {
            static void Prefix(BattleAction root)
            {
                lastActionSource = root.Source;
            }
        }


        [HarmonyPatch(typeof(ActionResolver), nameof(ActionResolver.InternalResolve))]
        class InternalResolve_Patch
        {
            static void Prefix(BattleAction action)
            {
                lastActionSource = action.Source;
            }
        }


        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Leave))]
        class BattleController_Patch
        {
            static void Postfix()
            {
                Log.LogDebug("end deez");
                lastActionSource = null;
            }
        }



    }
}
