using HarmonyLib;
using LBoL.Core.Cards;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Keywords
{
    [HarmonyPatch]
    class Clone_Patch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Card), nameof(Card.Clone), new Type[] { typeof(bool) });
            yield return AccessTools.Method(typeof(Card), nameof(Card.CloneBattleCard));
            yield return AccessTools.Method(typeof(Card), nameof(Card.CloneTwiceToken));
        }


        static void Postfix(Card __instance, Card __result)
        {

            foreach (var kw in __instance.AllCustomKeywords())
            {
                var clone = kw.Clone();
                if(clone != null)
                    __result.AddCustomKeyword(clone);
            }
        }
    }

}
