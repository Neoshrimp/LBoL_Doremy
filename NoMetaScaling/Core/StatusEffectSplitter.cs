using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Misfortune;
using LBoL.EntityLib.Cards.Neutral.Green;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.StatusEffects.Neutral.Red;
using NoMetaScaling.Core.Trackers;
using NoMetaScalling;

namespace NoMetaScaling.Core
{
    public static class StatusEffectSplitter
    {

        internal static HashSet<string> splitableSE_CARD_ids = new HashSet<string>() { nameof(RangziFanshu), nameof(MeihongPower) };

        [HarmonyPatch(typeof(ApplyStatusEffectAction), nameof(ApplyStatusEffectAction.PreEventPhase))]
        class ApplyStatusEffectAction_PreEvent_Patch
        {
            static void Prefix(ApplyStatusEffectAction __instance)
            {
                var action = __instance;
                var args = action.Args;

                if (!args.Effect.IsStackable)
                    return;


                if (action.Args.ActionSource?.TrickleDownActionSource() is Card card)
                {
                    if (splitableSE_CARD_ids.Contains(card.Id))
                    {
                        AttachIsSEReal(args.Effect, isReal: !card.IsBanned(out var _));
                    }

                }

            }
        }

        static ConditionalWeakTable<StatusEffect, IsSEReal> cwt_isReal = new ConditionalWeakTable<StatusEffect, IsSEReal>();


        internal static IsSEReal AttachIsSEReal(StatusEffect se, bool isReal)
        {
            return cwt_isReal.GetValue(se, _ => new IsSEReal(isReal));
        }

        internal static bool TryGetIsSEReal(StatusEffect se, out IsSEReal isSEReal)
        {
            return cwt_isReal.TryGetValue(se, out isSEReal);
        }

        internal static IsSEReal GetIsSEReal(StatusEffect se)
        {
            if (cwt_isReal.TryGetValue(se, out var rez))
                return rez;
            return null;
        }

        [HarmonyPatch(typeof(Unit), nameof(Unit.TryAddStatusEffect))]
        class TryAddStatusEffect_Mathching_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                     .MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Unit), nameof(Unit.GetStatusEffect), new Type[] { typeof(Type) })))
                     .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))

                     .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
                     .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TryAddStatusEffect_Mathching_Patch), nameof(TryAddStatusEffect_Mathching_Patch.MatchByIsReal))))
                     .InstructionEnumeration();
            }

            private static StatusEffect MatchByIsReal(Unit unit, StatusEffect otherSE)
            {
                if (TryGetIsSEReal(otherSE, out var isSEOtherReal))
                {
                    return unit.StatusEffects.FirstOrDefault(se => se.GetType() == otherSE.GetType() 
                    && isSEOtherReal.Equals(GetIsSEReal(se)));
                }
                return unit.GetStatusEffect(otherSE.GetType());
            }
        }



    }


    public class IsSEReal
    {
        public bool isReal;

        public IsSEReal(bool isReal)
        {
            this.isReal = isReal;
        }



        public static bool operator ==(IsSEReal a, IsSEReal b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;
            return a.Equals(b);
        }
        public static bool operator !=(IsSEReal a, IsSEReal b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is IsSEReal real &&
                   isReal == real.isReal;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(isReal);
        }
    }
}
