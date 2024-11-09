using HarmonyLib;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace NoMetaScaling.Core.Trackers
{
    public static class EchoTracker
    {
        public static Card lastEchoSource = null;

        public static bool IsNaturalEcho(this Card card) => card.IsUpgraded ? card.Config.UpgradedKeywords.HasFlag(LBoL.Base.Keyword.Echo) : card.Config.Keywords.HasFlag(LBoL.Base.Keyword.Echo);

        public static bool InvokedEcho(this Card card) => lastEchoSource == card;

        [HarmonyPatch(typeof(Card), nameof(Card.EchoCloneAction))]
        class EchoCloneAction_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeMatch(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.IsEcho))))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EchoCloneAction_Patch), nameof(EchoCloneAction_Patch.SetLastEcho))))


                    .InstructionEnumeration();

            }


            private static void SetLastEcho(Card card)
            {
                lastEchoSource = card;
            }
        }




        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Leave))]
        class BattleController_Patch
        {
            static void Postfix()
            {
                lastEchoSource = null;
            }
        }
    }
}
