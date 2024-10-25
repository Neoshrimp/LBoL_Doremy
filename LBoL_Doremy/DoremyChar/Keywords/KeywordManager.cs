using HarmonyLib;
using LBoL.Base;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.EnemyUnits.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Keywords
{
    public static class KeywordManager
    {
        static ConditionalWeakTable<Card, HashSet<CardKeyword>> cwt_keywords = new ConditionalWeakTable<Card, HashSet<CardKeyword>>();


        static HashSet<CardKeyword> GetKeywords(Card card) {
            return cwt_keywords.GetValue(card, (_) => new HashSet<CardKeyword>());
        }

        public static HashSet<CardKeyword> AllCustomKeywords(this Card card)
        {
            if(cwt_keywords.TryGetValue(card, out HashSet<CardKeyword> keywords))
                return keywords;
            return new HashSet<CardKeyword>();
        }

        public static bool HasCustomKeyword(this Card card, CardKeyword keyword) 
        {
            return GetKeywords(card).Contains(keyword);
        }

        public static void AddCustomKeyword(this Card card, CardKeyword keyword)
        {
            GetKeywords(card).Add(keyword);
        }
        public static bool RemoveCustomKeyword(this Card card, CardKeyword keyword)
        {
            return GetKeywords(card).Remove(keyword);
        }





        [HarmonyPatch(typeof(Card), nameof(Card.EnumerateDisplayWords), MethodType.Enumerator)]
        class Card_Tooltip_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
            {
                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Library), nameof(Library.InternalEnumerateDisplayWords))))
                    .MatchEndBackwards(OpCodes.Ldloc_S)
                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(originalMethod.DeclaringType, "verbose")))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))

                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Card_Tooltip_Patch), nameof(Card_Tooltip_Patch.AppendCustomKeywords))))

                    .InstructionEnumeration();
            }

            private static IEnumerable<string> AppendCustomKeywords(IReadOnlyList<string> keywords, bool displayVerbose, Card card)
            {
                return keywords.Concat(KeywordManager.AllCustomKeywords(card).Where(kw => !kw.isVerbose || displayVerbose).Select(kw => kw.kwSEid));
            }
        }



        [HarmonyPatch(typeof(Card), nameof(Card.EnumerateAutoAppendKeywordNames))]
        class Card_Description_Patch
        {
            static void Postfix(Card __instance, ref IEnumerable<string> __result)
            {
                var card = __instance;
                var kwToAppend = card.AllCustomKeywords().Where(kw => kw.descPos != KwDescPos.DoNotDisplay)
                    .GroupBy(kw => kw.descPos, kw => TypeFactory<StatusEffect>.LocalizeProperty(kw.kwSEid, "Name", true, false) );

                __result = kwToAppend.SelectMany(g => g.Key == KwDescPos.First ? g : Enumerable.Empty<string>())
                    .Concat(__result)
                    .Concat(kwToAppend.SelectMany(g => g.Key == KwDescPos.Last ? g : Enumerable.Empty<string>()));

            }
        }



        [HarmonyPatch(typeof(Card), nameof(Card.Description), MethodType.Getter)]
        class DescCheckCustomKw_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.DeclaredPropertyGetter(typeof(Card), nameof(Card.Keywords))))
                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DescCheckCustomKw_Patch), nameof(DescCheckCustomKw_Patch.CheckCustomKws))))
                    
                    .InstructionEnumeration();
            }

            private static bool CheckCustomKws(Keyword keywords, Card card)
            {
                return keywords != Keyword.None || card.AllCustomKeywords().Count > 0;
            }
        }



    }
}
