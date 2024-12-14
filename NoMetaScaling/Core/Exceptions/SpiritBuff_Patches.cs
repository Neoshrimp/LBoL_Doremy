using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using LBoL.EntityLib.StatusEffects.Enemy;
using NoMetaScaling.Core.Trackers;

namespace NoMetaScaling.Core.Exceptions
{


    [HarmonyPatch(typeof(ShenlingHp), nameof(ShenlingHp.OnEnemyDied))]
    class ShenlingHp_Patch
    {
        static void Prefix(ShenlingHp __instance)
        {
            ARTracker.lastActionSource = __instance;
        }
    }


    [HarmonyPatch]
    class ShenlingGold_Patch
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.FirstMethod(typeof(ShenlingGold), mi => mi.Name.Contains("<OnAdded>"));
        }

        static void Prefix(ShenlingGold __instance)
        {
            ARTracker.lastActionSource = __instance;
        }
    }

}
