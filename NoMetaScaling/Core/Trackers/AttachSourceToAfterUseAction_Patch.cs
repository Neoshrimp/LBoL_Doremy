using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace NoMetaScaling.Core.Trackers
{
    [HarmonyPatch]
    class AttachSourceToAfterUseAction_Patch
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            var delegateType = typeof(UseCardAction).GetNestedTypes(AccessTools.allDeclared).First(t => t.Name.Contains("DisplayClass16_0"));
            yield return AccessTools.FirstMethod(delegateType, mi => mi.Name.Contains("GetPhases>b__4"));
        }


        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
        {
            var instanceField = originalMethod.DeclaringType.GetFields(AccessTools.allDeclared).First(f => f.Name.Contains("this"));
            return new CodeMatcher(instructions)
                .MatchEndForward(new CodeMatch[] { OpCodes.Newobj, OpCodes.Ldnull })
                .SetInstruction(new CodeInstruction(OpCodes.Ldarg_0))
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, instanceField))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AttachSourceToAfterUseAction_Patch), nameof(AttachSourceToAfterUseAction_Patch.GetArgsCard))))

                //extra patch for frost exile source


                .InstructionEnumeration();
        }

        private static GameEntity GetArgsCard(UseCardAction action)
        {
            return action.Args.Card;
        }
    }
}
