﻿using HarmonyLib;
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

namespace NoMetaScaling.Core.Trackers
{
    public static class CardTracker
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
                    reason = BanReason.StatusEffectIsFake;
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

        public static bool IsTainted(this Card card) => Battle == null ? false : GetBanData(Battle).IsTainted(card);

        public static bool IsOnlyTainted(this Card card) => Battle == null ? false : GetBanData(Battle).IsOnlyTainted(card);


        



        public static void RegisterHandlers()
        {

            RegisterCommonHandlers(OnCardCreated);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardUsed, OnCardUsed, null, (GameEventPriority)9999);

        }

        private static void OnCardUsed(CardUsingEventArgs args)
        {
            GetBanData(Battle).BanCard(args.Card, BanReason.WasAlreadyUsed);
        }


        private static void OnCardCreated(Card[] cards, GameEventArgs args)
        {
            if (args.ActionSource.TrickleDownActionSource() is Card sourceCard)
            {
                var copyTargetsToTaint = new HashSet<Card>();
                foreach (var addedCard in cards)
                {

                    Log.LogDebug($"before {addedCard.Name} tainted:{addedCard.IsTainted()};banned:{addedCard.IsBanned(out var r)} {r}");
                    bool doBan = true;
                    BanReason reason = BanReason.CardIsGenerated;

                    // natural echo clause
                    if (!sourceCard.IsBanned(out var _) && sourceCard.InvokedEcho() && sourceCard.IsNaturalEcho())
                        continue;


                    if (PConfig.BanLevel <= BanLevel.NonPooledAndCopiesAllowed)
                    {
                        if (!addedCard.Config.IsPooled)
                        {
                            GetBanData(Battle).TaintCard(addedCard);
                            doBan = false;
                        }
                        else
                            reason = BanReason.GeneratedCardIsPooled;
                    }

                    // copying is superset of echoing
                    if (PConfig.BanLevel <= BanLevel.OnlyCopiesAllowed)
                    {
                        // if Copying happened, that is if a card created by CloneBattleCard was added to battlefield
                        if (GetCopyHistory(Battle).WasCopiedAndForget(addedCard, out var copyPair))
                        {
                            if (!sourceCard.IsBanned(out var _))
                            {
                                if (!copyPair.original.IsTainted())
                                    doBan = false;
                                else
                                {
                                    if (sourceCard.IsOnlyTainted())
                                        reason = BanReason.CopyTargetIsTainted;
                                    else
                                        reason = BanReason.CopyTargetIsBanned;
                                }

                                if (sourceCard.IsTainted())
                                {
                                    copyTargetsToTaint.Add(copyPair.original);
                                }
                                // all copies get tainted
                                GetBanData(Battle).TaintCard(addedCard);
                            }
                            else
                                reason = BanReason.CopySourceIsBanned;

                        }
                    }
                    else if (GetCopyHistory(Battle).WasCopiedAndForget(addedCard, out var _))
                        reason = BanReason.CardIsCopied;



                    if (doBan)
                        GetBanData(Battle).BanCard(addedCard, reason);
                    else
                        reason = BanReason.NotBanned;

                    Log.LogDebug($"after {addedCard.Name} tainted:{addedCard.IsTainted()};banned:{addedCard.IsBanned(out var _)};notbanned{reason}");

                }
                copyTargetsToTaint.Do(c =>
                {
                    GetBanData(Battle).TaintCard(c);
                    Log.LogDebug($"target {c.Name} tainted:{c.IsTainted()};banned:{c.IsBanned(out var r)};notbanned{r}");
                });
                
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
        WasAlreadyUsed,
        StatusEffectIsFake,
        // level 0
        GeneratedCardIsPooled,
        // level 1
        CopySourceIsBanned,
        CopyTargetIsTainted,
        CopyTargetIsBanned,
        // strict
        CardIsCopied,
        CardIsGenerated,
        // should not be there
        Other
    }

    public class BanData
    {
        public void BanCard(Card card, BanReason reason) => bannedCards.AlwaysAdd(card, reason);

        public bool IsBanned(Card card, out BanReason reason)
        {
            reason = BanReason.NotBanned;
            var rez = bannedCards.TryGetValue(card, out var actualReason);
            if (rez)
                reason = actualReason;
            return rez;
        }

        public void TaintCard(Card card) => taintedCards.Add(card);

        // any banned is tainted, tainted is superset of banned
        public bool IsTainted(Card card) => taintedCards.Contains(card) || IsBanned(card, out var _);

        public bool IsOnlyTainted(Card card) => taintedCards.Contains(card);


        Dictionary<Card, BanReason> bannedCards = new Dictionary<Card, BanReason>();

        HashSet<Card> taintedCards = new HashSet<Card>();

    }
}
