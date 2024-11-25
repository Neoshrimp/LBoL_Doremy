using HarmonyLib;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using LBoL.Presentation;
using LBoLEntitySideloader.CustomHandlers;

namespace LBoL_Doremy.CreatedCardTracking
{
    public static class CreatedIdTracker
    {
        static int startingId = 1000;

        public static bool WasGenerated(this Card card) => card.InstanceId > startingId || card.IsPlayTwiceToken;


        [HarmonyPatch(typeof(BattleController), MethodType.Constructor, new Type[] { typeof(GameRunController), typeof(EnemyGroup), typeof(IEnumerable<Card>) })]
        [HarmonyPriority(HarmonyLib.Priority.Last)]
        class BattleController_Patch
        {
            static void Postfix(BattleController __instance)
            {
                startingId = __instance._cardInstanceId;
            }
        }
    }
}
