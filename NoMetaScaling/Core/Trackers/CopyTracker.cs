using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NoMetaScaling.Core.Trackers
{
    internal static class CopyTracker
    {
        [HarmonyPatch]
        class CloneBattleCard_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(Card), nameof(Card.CloneBattleCard));
                // could be used as shortcut for copying
                // yield return AccessTools.Method(typeof(Card), nameof(Card.CloneTwiceToken));
            }


            static void Postfix(Card __instance, Card __result)
            {
                BattleCWT.CopyHistory.AddCopyPair(new CopyPair(__instance, __result));
            }
        }


        //[HarmonyPatch]
        class CloneTwiceToken_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(Card), nameof(Card.CloneTwiceToken));

            }


            static void Postfix(Card __instance, Card __result)
            {
                if(__instance.IsBanned(out var _))
                    BattleCWT.GetBanData(BattleCWT.Battle).BanCard(__result, BanReason.CopySourceWasBanned);
            }
        }



    }

    public class CopyHistory
    {
        HashSet<CopyPair> cardsCopied = new HashSet<CopyPair>();

        public void AddCopyPair(CopyPair copyPair) => cardsCopied.Add(copyPair);

        public bool WasCopied(Card potentialCopy, out CopyPair copyPair)
        {
            var rez = cardsCopied.TryGetValue(new CopyPair(null, potentialCopy), out copyPair);
            return rez;
        }

        public bool IfWasCopiedForget(Card potentialCopy, out CopyPair copyPair)
        {
            var rez = WasCopied(potentialCopy, out copyPair);
            if (rez)
                cardsCopied.Remove(copyPair);
            return rez;
        }


        /*        internal List<CopyPair> LastSlice(int range)
                {
                    int lower = Math.Max(cardsCopied.Count - range, 0);
                    int count = Math.Min(range, cardsCopied.Count - lower);
                    return cardsCopied.GetRange(lower, count);
                }*/




    }

    public struct CopyPair : IEquatable<CopyPair>
    {
        private readonly bool nonDefault;
        public Card original;
        public Card copy;

        public CopyPair(Card original, Card copy)
        {
            this.original = original;
            this.copy = copy;
            this.nonDefault = true;
        }

        public bool IsNonDefault => nonDefault;

        public bool Equals(CopyPair other)
        {
            return this.copy == other.copy;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nonDefault, copy);
        }
    }
}
