using HarmonyLib;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using LBoL.Presentation;
using LBoLEntitySideloader.CustomHandlers;
using NoMetaScaling.Core;
using LBoL.Core.StatusEffects;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using NoMetaScalling;

namespace NoMetaScaling.Core
{
    public static class CardTracker
    {
        static int startingId = 1000;


        //2do incorect and wonky on stackble status effects
        [return: MaybeNull]
        public static GameEntity TrickleDownActionSource(this GameEntity actionSource)
        {
            var rez = actionSource;
            if (actionSource is StatusEffect se && se.SourceCard != null)
                rez = se.SourceCard;
            return rez;

        }

        public static bool WasGenerated(this Card card) => card.InstanceId > startingId;

        public static bool IsBanned(this Card card) => Battle == null ? false : GetBanData(Battle).bannedCards.Contains(card);


        static ConditionalWeakTable<BattleController, BanData> cwt_banData = new ConditionalWeakTable<BattleController, BanData>();

        static WeakReference<BattleController> battle_ref;

        public static BattleController Battle
        {
            get
            {
                var rez = GameMaster.Instance.CurrentGameRun?.Battle;
                if(rez == null)
                    battle_ref.TryGetTarget(out rez);
                return rez;
            }
        }

        public static BanData GetBanData(BattleController battle) => cwt_banData.GetOrCreateValue(battle);


        [HarmonyPatch(typeof(BattleController), MethodType.Constructor, new Type[] { typeof(GameRunController), typeof(EnemyGroup), typeof(IEnumerable<Card>) })]
        [HarmonyPriority(HarmonyLib.Priority.Last)]
        class BattleController_Patch
        {

            static void Prefix(BattleController __instance)
            {
                var battle = __instance;
                battle_ref = new WeakReference<BattleController>(battle);
                cwt_banData.Add(battle, new BanData());

            }
            static void Postfix(BattleController __instance)
            {
                startingId = __instance._cardInstanceId;
            }
        }



        public static void RegisterHandlers()
        {
            RegisterCommonHandlers(OnCardCreated);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardMoved, OnMovedFromExile);
        }

        private static void OnMovedFromExile(CardMovingEventArgs args)
        {
            if (args.ActionSource.TrickleDownActionSource() is Card card)
            {
                GetBanData(Battle).bannedCards.Add(args.Card);
            }
        }

        private static void OnCardCreated(Card[] cards, GameEventArgs args)
        {

            Log.LogDebug(args.ActionSource);
            Log.LogDebug(args.Cause);


            if (args.ActionSource.TrickleDownActionSource() is Card sourceCard)
            {
                foreach (var c in cards)
                {
                    if (!sourceCard.IsBanned() && sourceCard.InvokedEcho() && sourceCard.IsNaturalEcho())
                        continue;

                    GetBanData(Battle).bannedCards.Add(c);
                }
            }
        }

        static void RegisterCommonHandlers(Action<Card[], GameEventArgs> handler, GameEventPriority priority = GameEventPriority.ConfigDefault)
        {
            GameEventHandler<CardsAddingToDrawZoneEventArgs> drawZoneReactor = args => handler(args.Cards, args);
            GameEventHandler<CardsEventArgs> otherReactors = args => handler(args.Cards, args);

            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToExile, otherReactors, null, priority);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToDrawZone, drawZoneReactor, null, priority);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToHand, otherReactors, null, priority);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToDiscard, otherReactors, null, priority);
        }

    }

    public class BanData
    {
        public HashSet<Card> bannedCards = new HashSet<Card>();
    }
}
