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
using System.Reflection;
using LBoL.Presentation.UI.Panels;
using System.IO;
using LBoLEntitySideloader.ReflectionHelpers;
using MonoMod.Utils;
using NoMetaScaling.Core.API;

namespace NoMetaScaling.Core
{
    // 2do actionless reactors lose their source at the end of combat, SanaePowerPotato
    // 2do ban only if bannable actions are performed 
    public static class ActionCancel
    {
        static IEnumerable<BattleAction> DoChat(GameEntity source, string cancelTarget, BanReason reason)
        {
            //Log.LogDebug(reason);
            var player = GameMaster.Instance.CurrentGameRun?.Player;
            if (player == null)
                yield break;


            if (UnityEngine.Random.Range(0, 1f) > 0.98f)
                yield return PerformAction.Chat(player, $"I'M A DEGENERATE", 1.75f);
            else
            {
                var chatString = NoMoreMetaScalingLocSE.GetBanChatString(source, cancelTarget, reason);
                yield return PerformAction.Chat(player, chatString, 6f);
            }


        }

        static void DoYap(GameEntity source, string cancelTarget, BanReason reason)
        {
            foreach (var a in DoChat(source, cancelTarget, reason))
                 BattleCWT.Battle.RequestDebugAction(a, "Yapping");
        }

        static bool PrefixCancel(GameRunController gr, string cancelTarget, GameEntity actionSource = null)
        {
            var battle = gr.Battle;

            if (battle == null)
                return true;

            if (!GrCWT.GetGrState(gr).cancelEnnabled)
                return true;

            if (actionSource == null)
                actionSource = ARTracker.lastActionSource;

            //Log.LogDebug(actionSource);

            if(CardFilter.IsEntityBanned(actionSource, out var reason))
            {
                DoYap(ARTracker.lastActionSource, cancelTarget, reason);
                return false;
            }

            if (actionSource.TrickleDownActionSource() is Card card)
                BattleCWT.GetBanData(battle).QueueBan(card, BanReason.MetaResourcesAlreadyProvided);

            return true;
        }


        private static void CancelAction(GameEventArgs args, string cancelTarget)
        {
            if (!GrCWT.GetGrState(GrCWT.GR).cancelEnnabled)
                return;

            if (CardFilter.IsEntityBanned(args.ActionSource, out var reason))
            {
                // safety check for fringe cases where events are not triggered correctly 
                if (BattleCWT.Battle._resolver._reactors == null)
                    DoYap(args.ActionSource, cancelTarget, reason);
                else
                    BattleCWT.Battle.React(new Reactor(DoChat(args.ActionSource, cancelTarget, reason)), null, ActionCause.None);
                args.CancelBy(args.ActionSource);
            }
            if (args.ActionSource.TrickleDownActionSource() is Card card)
                BattleCWT.GetBanData(BattleCWT.Battle).QueueBan(card, BanReason.MetaResourcesAlreadyProvided);
        }


        public static void RegisterHandlers()
        {
            CHandlerManager.RegisterBattleEventHandler(bt => bt.Player.HealingReceiving, OnPlayerHealing, null, GameEventPriority.Lowest);
            // OnDeckCardAdding maybe. 

        }

        private static void OnPlayerHealing(HealEventArgs args)
        {
            CancelAction(args, NoMoreMetaScalingLocSE.LocalizeProp("Healing"));
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


        [HarmonyPatch]
        class GainMoney_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(GameRunController), nameof(GameRunController.GainMoney));
                // too sweeping
                //yield return AccessTools.Method(typeof(GameRunController), nameof(GameRunController.InternalGainMoney));

            }


            static bool Prefix(GameRunController __instance)
            {
                var rez = PrefixCancel(__instance, NoMoreMetaScalingLocSE.LocalizeProp("Money"));
                return rez;
            }
        }


        [HarmonyPatch(typeof(GainMoneyAction), nameof(GainMoneyAction.ResolvePhase))]
        class GainMoneyAction_Patch
        {
            static FieldInfo f_Money = null;
            static FieldInfo F_Money
            {
                get
                {
                    if (f_Money == null) 
                    {
                        f_Money = AccessTools.Field(typeof(GainMoneyAction), ConfigReflection.BackingWrap(nameof(GainMoneyAction.Money)));
                    }
                    return f_Money;
                }
            }

            static bool Prefix(GainMoneyAction __instance)
            {
                var rez = PrefixCancel(__instance.Battle.GameRun, NoMoreMetaScalingLocSE.LocalizeProp("Money"), __instance.Source);

                if (!rez)
                    F_Money.SetValue(__instance, 0);

                return rez;
            }
        }



        [HarmonyPatch(typeof(GameRunVisualPanel), nameof(GameRunVisualPanel.ViewGainMoney))]
        class ViewGainMoney_Patch
        {
            static bool Prefix(GainMoneyAction action)
            {
                if (action.Money <= 0)
                    return false;
                return true;
            }
        }








        [HarmonyPatch(typeof(GainPowerAction), nameof(GainPowerAction.PreEventPhase))]
        class GainPowerAction_Patch
        {
            static void Postfix(GainPowerAction __instance)
            {

                var args = __instance.Args;
                CancelAction(args, NoMoreMetaScalingLocSE.LocalizeProp("Power"));

            }
        }




        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.GainPower))]
        class GainPower_Patch
        {
            static bool Prefix(GameRunController __instance)
            {
                return PrefixCancel(__instance, NoMoreMetaScalingLocSE.LocalizeProp("Power"));
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

        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.FinalBossInitialDamage), MethodType.Setter)]
        class FinalBossInitialDamage_Patch
        {
            static bool Prefix(GameRunController __instance, int value)
            {
                if(value > 0)
                    return PrefixCancel(__instance, NoMoreMetaScalingLocSE.LocalizeProp("SeijaDmg"));
                return true;
            }
        }

    }








}
