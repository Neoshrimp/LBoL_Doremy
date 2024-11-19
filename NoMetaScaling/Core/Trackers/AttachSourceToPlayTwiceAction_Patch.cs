using HarmonyLib;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;
using System.Linq;
using NoMetaScalling;
using LBoL.Core.Cards;

namespace NoMetaScaling.Core.Trackers
{


    //[HarmonyPatch(typeof(PlayTwiceAction), MethodType.Constructor, new Type[] { typeof(Card), typeof(CardUsingEventArgs) })]
    class PlayTwiceAction_Patch
    {
        static void Postfix(PlayTwiceAction __instance, CardUsingEventArgs args)
        {
            __instance.Args._modifiers.AddRange(args._modifiers);
        }
    }



    //[HarmonyPatch]
    class AttachSourceToPlayTwiceAction_Patch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.FirstMethod(typeof(PlayTwiceAction), mi => mi.Name.Contains("GetPhases>b__2_1"));

        }


        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
        {
            var matcher = new CodeMatcher(instructions)
                .Start();

            matcher.MatchEndForward(new CodeMatch[] { OpCodes.Ldnull });
            matcher.SetInstruction(new CodeInstruction(OpCodes.Ldarg_0))
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AttachSourceToPlayTwiceAction_Patch), nameof(AttachSourceToPlayTwiceAction_Patch.GetDoublePlaySource))));


            return matcher.InstructionEnumeration();
        }

        private static GameEntity GetDoublePlaySource(PlayTwiceAction action)
        {
            var args = action.Args;
            if (args._modifiers.LastOrDefault()?.TryGetTarget(out var lastMod) != null)
            {
                Log.LogDebug(lastMod);
                return lastMod;
            }
            return null;
        }
    }

}

