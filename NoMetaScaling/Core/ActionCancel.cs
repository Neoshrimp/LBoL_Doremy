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
using LBoL.Presentation;
using LBoL.Core.Battle;
using NoMetaScalling;
using UnityEngine;
using NoMetaScaling.Core.Trackers;
using NoMetaScaling.Core.Loc;

namespace NoMetaScaling.Core
{
    public static class ActionCancel
    {
        static IEnumerable<BattleAction> DoChat(GameEntity source, string cancelTarget, BanReason reason)
        {
            Log.LogDebug(reason);
            var player = GameMaster.Instance.CurrentGameRun?.Player;
            if (player == null)
                yield break;


            if (UnityEngine.Random.Range(0, 1f) > 0.96f)
                yield return PerformAction.Chat(player, $"I'M A DEGENERATE", 1.75f);
            else
            {
                //var chatString = string.Format(NoMoreMetaScalingLocSE.LocalizeProp("CancelExplain", true), cancelTarget, source.Name);
                var chatString = $"{source.Name}: {reason}";
                yield return PerformAction.Chat(player, chatString, 3f/*7f*/);
            }


        }

        static void DoYap(GameEntity source, string cancelTarget, BanReason reason)
        {
            foreach (var a in DoChat(source, cancelTarget, reason))
                 BattleCWT.Battle.RequestDebugAction(a, "Yapping");
        }

        static bool PrefixCancel(GameRunController gr, string cancelTarget)
        {
            var battle = gr.Battle;

            if (battle == null)
                return true;
            if(CardTracker.IsEntityBanned(ARTracker.lastActionSource, out var reason))
            {
                DoYap(ARTracker.lastActionSource, cancelTarget, reason);
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
            if (CardTracker.IsEntityBanned(ARTracker.lastActionSource, out var reason))
            {
                BattleCWT.Battle.React(new Reactor(DoChat(ARTracker.lastActionSource, NoMoreMetaScalingLocSE.LocalizeProp("Healing"), reason)), null, ActionCause.None);
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

                if (CardTracker.IsEntityBanned(args.ActionSource, out var reason))
                {
                    __instance.React(new Reactor(DoChat(args.ActionSource, NoMoreMetaScalingLocSE.LocalizeProp("Power"), reason)));
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
