using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using NoMetaScalling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoMetaScaling.Core.Trackers
{
    // 2do ActionResolver does not capture actionless handler source
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

            static IEnumerator<object> Postfix(IEnumerator<object> values)
            {
                while (values.MoveNext())
                    yield return values.Current;

                //Log.LogDebug($"{lastActionSource?.Name} reset AR");
                BattleCWT.GetBanData(BattleCWT.Battle).FlushPendingBan();
                lastActionSource = null;
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
                lastActionSource = null;
            }
        }



    }
}
