using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using LBoL.Core.StatusEffects;
using LBoL.Core;
using LBoLEntitySideloader.CustomHandlers;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using NoMetaScaling.Core;
using LBoL.Presentation;
using LBoL.Core.Battle;
using NoMetaScalling;
using UnityEngine;

namespace NoMetaScaling.Core
{
    public static class ActionCancel
    {
        static IEnumerable<BattleAction> DoChat(GameEntity source, string cancelTarget)
        {
            var player = GameMaster.Instance.CurrentGameRun?.Player;
            if (player == null)
                yield break;

            if (UnityEngine.Random.Range(0, 1f) > 0.96f)
                yield return PerformAction.Chat(player, $"I'M A DEGENERATE", 1.75f);
            else
            {
                var chatString = string.Format(NoMoreMetaScalingLocSE.LocalizeProp("CancelExplain", true), cancelTarget, source.Name);
                yield return PerformAction.Chat(player, chatString, 7f);
            }


        }

        static void DoYap(Card card, string cancelTarget)
        {
            foreach (var a in DoChat(card, cancelTarget))
                 CardTracker.Battle.RequestDebugAction(a, "Yapping");
        }

        static bool PrefixCancel(GameRunController gr, string cancelTarget)
        {
            var battle = gr.Battle;

            if (battle == null)
                return true;

            if (ARTracker.lastActionSource?.TrickleDownActionSource() is Card card && card.ShouldBeBanned())
            {
                DoYap(card, cancelTarget);
                return false;
            }

            return true;
        }


        public static void RegisterHandlers()
        {
            CHandlerManager.RegisterBattleEventHandler(bt => bt.Player.HealingReceiving, OnPlayerHealing, null, GameEventPriority.Lowest);
            // OnDeckCardAdding maybe. 

        }

        private static void OnPlayerHealing(HealEventArgs args)
        {
            if (args.ActionSource?.TrickleDownActionSource() is Card card && card.ShouldBeBanned())
            {
                CardTracker.Battle.React(new Reactor(DoChat(card, NoMoreMetaScalingLocSE.LocalizeProp("Healing"))), null, ActionCause.None);
                args.CancelBy(args.ActionSource);
            }
        }



        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.GainMaxHp))]
        class GainMaxHp_Patch
        {
            static bool Prefix(GameRunController __instance)
            {
                return PrefixCancel(__instance, NoMoreMetaScalingLocSE.LocalizeProp("MaxHp"));
            }
        }

        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.GainMaxHpOnly))]
        class GainMaxHpOnly_Patch
        {
            static bool Prefix(GameRunController __instance)
            {
                return PrefixCancel(__instance, NoMoreMetaScalingLocSE.LocalizeProp("MaxHp"));
            }
        }



        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.GainMoney))]
        class GainMoney_Patch
        {
            static bool Prefix(GameRunController __instance)
            {
                return PrefixCancel(__instance, NoMoreMetaScalingLocSE.LocalizeProp("Money"));
            }
        }




        [HarmonyPatch(typeof(GainPowerAction), nameof(GainPowerAction.PreEventPhase))]
        class GainPowerAction_Patch
        {
            static void Postfix(GainPowerAction __instance)
            {
                var args = __instance.Args;
                if (args.ActionSource?.TrickleDownActionSource() is Card card && card.ShouldBeBanned())
                {
                    __instance.React(new Reactor(DoChat(card, NoMoreMetaScalingLocSE.LocalizeProp("Power"))));
                    args.CancelBy(args.ActionSource);
                }
            }
        }





        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.UpgradeDeckCards))]
        class UpgradeDeckCard_Patch
        {
            static bool Prefix(GameRunController __instance)
            {
                return PrefixCancel(__instance, NoMoreMetaScalingLocSE.LocalizeProp("UpgradeACard"));
            }

            
        }


    }



}
