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
using LBoL.Core.StatusEffects;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using NoMetaScalling;
using System.Linq;
using static NoMetaScaling.Core.BattleCWT;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader;
using NoMetaScaling.Core.Trackers;
using NoMetaScaling.Core.API;

namespace NoMetaScaling.Core
{
    public static class CardFilter
    {


        [return: MaybeNull]
        public static GameEntity TrickleDownActionSource(this GameEntity actionSource)
        {
            var rez = actionSource;
            if (actionSource is StatusEffect se && se.SourceCard != null)
                rez = se.SourceCard;
            return rez;

        }

        public static bool IsEntityBanned(GameEntity gameEntity, out BanReason reason)
        {
            reason = BanReason.NotBanned;
            if (gameEntity is StatusEffect se && StatusEffectSplitter.TryGetIsSEReal(se, out var isSEReal))
            {
                if (!isSEReal.isReal)
                {
                    reason = BanReason.StatusEffectWasFake;
                    return true;
                }
                return false;
            }

            if (gameEntity?.TrickleDownActionSource() is Card card)
                return card.IsBanned(out reason);

            return false;

        }

        public static bool WasGenerated(this Card card) => card.InstanceId > startingId;

        public static bool IsBanned(this Card card, out BanReason reason)
        {
            if (Battle == null)
            {
                reason = BanReason.NotBanned;
                return false;
            }
            return GetBanData(Battle).IsBanned(card, out reason);
        }




        public static void RegisterHandlers()
        {

            RegisterCommonHandlers(OnCardCreated);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardUsed, OnCardUsed, null, (GameEventPriority)9999);

        }

        private static void OnCardUsed(CardUsingEventArgs args)
        {
            var card = args.Card;
            if (card.CardType == LBoL.Base.CardType.Friend 
                && card.Summoned 
                && !GetBanData(Battle).alreadySummoned.Contains(card))
            { 
                GetBanData(Battle).alreadySummoned.Add(card);
                return;
            }

            GetBanData(Battle).BanCard(args.Card, BanReason.CardWasAlreadyUsed);
        }

        private static void OnCardCreated(Card[] cards, GameEventArgs args)
        {
            if (args.ActionSource.TrickleDownActionSource() is Card sourceCard)
            {
                foreach (var addedCard in cards)
                {
                    if (ExposedStatics.exemptFromBan.Contains(addedCard.Id))
                        continue;

                    bool doBan = true;
                    BanReason reason = BanReason.CardWasGenerated;

                    // natural echo clause
                    if (!sourceCard.IsBanned(out var _) && sourceCard.InvokedEcho() && sourceCard.IsNaturalEcho())
                        continue;


                    // copying is superset of echoing
                    if (PConfig.BanLevel <= BanLevel.RealCopiesAllowed)
                    {
                        // if Copying happened, that is if a card created by CloneBattleCard was added to battlefield
                        if (GetCopyHistory(Battle).IfWasCopiedForget(addedCard, out var copyPair))
                        {
                            if (!sourceCard.IsBanned(out var _))
                            {
                                if (!copyPair.original.IsBanned(out var _))
                                    doBan = false;
                                else
                                    reason = BanReason.CopyTargetWasBanned;
                            }
                            else
                                reason = BanReason.CopySourceWasBanned;

                        }
                    }
                    else if (GetCopyHistory(Battle).IfWasCopiedForget(addedCard, out var _))
                        reason = BanReason.CardWasCopied;



                    if (doBan)
                    {
                        if (ExposedStatics.dontBanUnlessCopied.Contains(addedCard.Id) && !BanData.CopiedReasons.Contains(reason))
                        { }
                        else
                            GetBanData(Battle).BanCard(addedCard, reason);
                    }


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

    public enum BanReason
    {
        // common
        NotBanned,
        CardWasAlreadyUsed,
        StatusEffectWasFake,
        // level 1
        CopySourceWasBanned,
        CopyTargetWasBanned,
        // strict
        CardWasCopied,
        CardWasGenerated,
        // should not be there
        Other
    }

    public class BanData
    {
        public static readonly BanReason[] CopiedReasons = new BanReason[] {BanReason.CopySourceWasBanned, BanReason.CopyTargetWasBanned, BanReason.CardWasCopied};
        public void BanCard(Card card, BanReason reason) => bannedCards.AlwaysAdd(card, reason);


        public bool IsBanned(Card card, out BanReason reason)
        {
            reason = BanReason.NotBanned;
            var rez = bannedCards.TryGetValue(card, out var actualReason);
            if (rez)
                reason = actualReason;
            return rez;
        }


        Dictionary<Card, BanReason> bannedCards = new Dictionary<Card, BanReason>();

        internal HashSet<Card> alreadySummoned = new HashSet<Card>();


    }
}
