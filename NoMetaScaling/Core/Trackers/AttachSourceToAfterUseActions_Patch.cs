using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoLEntitySideloader.ReflectionHelpers;
using NoMetaScalling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace NoMetaScaling.Core.Trackers
{
    [HarmonyPatch]
    class AttachSourceToAfterUseActions_Patch
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            var useCardDelegateType = typeof(UseCardAction).GetNestedTypes(AccessTools.allDeclared).Single(t => t.Name.Contains("DisplayClass17_0"));
            yield return AccessTools.FirstMethod(useCardDelegateType, mi => mi.Name.Contains("GetPhases>b__4"));


            // formerly DisplayClass18_0
            var playCardDelegateType = typeof(PlayCardAction).GetNestedTypes(AccessTools.allDeclared).Single(t => t.Name.Contains("DisplayClass22_0"));

            yield return AccessTools.FirstMethod(playCardDelegateType, mi => mi.Name.Contains("GetPhases>b__2"));


        }


        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
        {
            var instanceField = originalMethod.DeclaringType.GetFields(AccessTools.allDeclared).First(f => f.Name.Contains("this"));
            var matcher = new CodeMatcher(instructions)
                .Start();


            int i = 0;
            while (matcher.IsValid && i < 2)
            {
                matcher.MatchEndForward(new CodeMatch[] { new CodeMatch(ci => ci.opcode == OpCodes.Newobj && ci.operand is MethodBase mb && mb.DeclaringType == typeof(Reactor)), OpCodes.Ldnull });

                if (!matcher.IsValid)
                    break;
                matcher.SetInstruction(new CodeInstruction(OpCodes.Ldarg_0))
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, instanceField))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AttachSourceToAfterUseActions_Patch), nameof(AttachSourceToAfterUseActions_Patch.GetArgsCard))));
                i++;
            }



            return matcher.InstructionEnumeration();
        }

        private static GameEntity GetArgsCard(BattleAction action)
        {
            if (action is UseCardAction useCardAction)
                return useCardAction.Args.Card;
            else if (action is PlayCardAction playCardAction)
                return playCardAction.Args.Card;
            return null;
        }
    }
}
