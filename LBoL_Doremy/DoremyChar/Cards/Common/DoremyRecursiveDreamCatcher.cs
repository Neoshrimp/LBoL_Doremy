using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyRecursiveDreamCatcherDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1 };

            con.Mana = new ManaGroup() { White = 1 };

            con.Block = 8;
            con.UpgradedBlock = 11;


            con.Keywords = Keyword.Exile | Keyword.Echo;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Echo;

            con.RelativeKeyword = Keyword.CopyHint;
            con.UpgradedRelativeKeyword = Keyword.CopyHint;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyRecursiveDreamCatcherDef))]
    [HarmonyDebug]
    public sealed class DoremyRecursiveDreamCatcher : DCard
    {


        [HarmonyPatch(typeof(Card), nameof(Card.EchoCloneAction))]
        class EchoClone_Patch
        {
            static void Postfix(Card __instance, BattleAction __result)
            {
                if (__instance is DoremyRecursiveDreamCatcher dc && __result is AddCardsToHandAction addAction)
                {
                    var copy = addAction.Args.Cards.First();
                    copy.TurnCostDelta = ManaGroup.Empty;
                    copy.BaseCost = dc.Mana;
                    copy.FreeCost = false;
                }
            }
        }

        [HarmonyPatch(typeof(Card), nameof(Card.IsEcho), MethodType.Setter)]
        class Echo_Patch
        {
            static void Postfix(Card __instance)
            {
                if (__instance is DoremyRecursiveDreamCatcher)
                    __instance.SetKeyword(Keyword.Echo, true);
            }
        }

        [HarmonyPatch(typeof(Card), nameof(Card.IsCopy), MethodType.Setter)]
        class Copy_Patch
        {
            static void Postfix(Card __instance)
            {
                if(__instance is DoremyRecursiveDreamCatcher)
                    __instance.SetKeyword(Keyword.Copy, false);

            }
        }


    }
}
