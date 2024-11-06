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
using NoMetaScaling.Main;
using LBoL.Presentation;
using LBoL.Core.Battle;

namespace NoMetaScaling.Handlers
{
    public static class ActionCancelHandlers
    {
        [return: MaybeNull]
        public static GameEntity TrickleDownActionSource(this GameEntity actionSource)
        {
            var rez = actionSource;
            if (actionSource is StatusEffect se && se.SourceCard != null)
                rez = se.SourceCard;
            return rez;

        }

        public static void RegisterHandlers()
        {
/*            CHandlerManager.RegisterGameEventHandler(gr => gr.DeckCardsAdding, OnDeck);
            CHandlerManager.RegisterGameEventHandler(gr => gr.DeckCardsUpgraded, OnDeck);

            CHandlerManager.RegisterGameEventHandler(gr => gr., OnDeck);*/


        }


        static IEnumerable<BattleAction> DoChat(GameEntity source)
        {
            var player = GameMaster.Instance.CurrentGameRun?.Player;
            if (player == null)
                yield break;

            yield return PerformAction.Chat(player, $"Deeznuts {source?.Name}", 3f);
        }

        [HarmonyPatch(typeof(GainPowerAction), nameof(GainPowerAction.PreEventPhase))]
        class GainPowerAction_Patch
        {
            static void Postfix(GainPowerAction __instance)
            {
                var args = __instance.Args;
                if (args.ActionSource?.TrickleDownActionSource() is Card card && card.WasGenerated())
                {
                    __instance.React(new Reactor(DoChat(card)), null, ActionCause.None);
                    args.CancelCause = CancelCause.Reaction;
                    args.CancelBy(card);
                }
            }
        }


        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.UpgradeDeckCard))]
        class GameRunController_Patch
        {
            static void Prefix()
            {

            }
            static void Postfix()
            {

            }
        }


    }



}
