using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using NoMetaScalling;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMetaScaling.Core.Trackers
{
    public static class ARTracker
    {
        internal static GameEntity lastActionSource = null;


        [HarmonyPatch(typeof(ActionResolver), nameof(ActionResolver.InternalResolve))]
        class ActionResolver_Patch
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
                lastActionSource = null;
            }
        }



    }
}
