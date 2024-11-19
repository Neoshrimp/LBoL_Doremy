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
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.EnemyUnits.Lore;
using LBoL.Core.Battle.BattleActions;
using System.Reflection.Emit;

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
            NoMetaScalinAPI.ExemptFromBanIfGenerated(nameof(PurpleMogu));

            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(VampireShoot1));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(VampireShoot2));



            RegisterCommonHandlers(OnCardCreated);
            //CHandlerManager.RegisterBattleEventHandler(bt => bt.CardUsed, OnCardUsed, null, (GameEventPriority)9999);

            //CHandlerManager.RegisterBattleEventHandler(bt => bt.CardPlaying, OnCardPlaying, null, (GameEventPriority)9999);

            //CHandlerManager.RegisterBattleEventHandler(bt => bt.CardUsing, OnCardUsing, null, GameEventPriority.Lowest);
            

        }


        [HarmonyPatch(typeof(UseCardAction), nameof(UseCardAction.GetPhases), MethodType.Enumerator)]
        class OnCardUsing_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(CardUsingEventArgs), nameof(CardUsingEventArgs.PlayTwice))))
                    .MatchEndForward(new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(Card), nameof(Card.PlayTwiceSourceCard))))

                    .Advance(1)


                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CardFilter), nameof(CardFilter.OnCardUsing))))

                    .InstructionEnumeration();
            }

        }



        private static void OnCardUsing(UseCardAction action)
        {
            CardUsingEventArgs args = action.Args;
            var playingCard = args.Card;
            var copy = action._twiceTokenCard;
            if (args.PlayTwice && (args._modifiers.LastOrDefault()?.TryGetTarget(out var lastMod) ?? false))
            {
                if (lastMod.TrickleDownActionSource() is Card sourceCard)
                {
                    var doBan = HandleCopying(sourceCard, playingCard, out var reason);
                    if (doBan)
                        GetBanData(Battle).BanCard(copy, reason);
                }

            }
        }



        internal static bool HandleCopying(Card sourceCard, Card original, out BanReason reason)
        {
            reason = BanReason.NotBanned;
            bool doBan = true;
            if (PConfig.BanLevel <= BanLevel.RealCopiesAllowed)
            {
                if (!sourceCard.IsBanned(out var _))
                {
                    if (!original.IsBanned(out var _))
                        doBan = false;
                    else
                        reason = BanReason.CopyTargetWasBanned;
                }
                else
                    reason = BanReason.CopySourceWasBanned;

                GetBanData(Battle).QueueBan(sourceCard, BanReason.CardWasAlreadyUsed);
            }
            else
            {
                reason = BanReason.CardWasCopied;
                GetBanData(Battle).QueueBan(sourceCard, BanReason.CardWasAlreadyUsed);
            }
            
            return doBan;
        }

        private static void OnCardCreated(Card[] cards, GameEventArgs args)
        {
            if (args.ActionSource.TrickleDownActionSource() is Card sourceCard)
            {
                foreach (var addedCard in cards)
                {
                    if (!sourceCard.IsBanned(out var _) && ExposedStatics.exemptFromGenBan.Contains(addedCard.Id))
                        continue;

                    bool doBan = true;
                    BanReason reason = BanReason.CardWasGenerated;

                    // natural echo clause
                    if (!sourceCard.IsBanned(out var _) && sourceCard.InvokedEcho() && sourceCard.IsNaturalEcho())
                        continue;


                    if (GetCopyHistory(Battle).IfWasCopiedForget(addedCard, out var copyPair))
                        doBan = HandleCopying(sourceCard, copyPair.original, out reason);


                    if (doBan)
                    {
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
        MetaResourcesAlreadyProvided,
        CardWasAlreadyUsed,
        StatusEffectWasFake,
        // level 1
        CopySourceWasBanned,
        CopyTargetWasBanned,
        // strict
        CardWasCopied,
        CardWasGenerated,
        BannedByDefault,
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
            if (ExposedStatics.alwaysBanned.Contains(card.Id))
            {
                reason = BanReason.BannedByDefault;
                return true;
            }


            var rez = bannedCards.TryGetValue(card, out var actualReason);
            if (rez)
                reason = actualReason;
            return rez;
        }


        internal void FlushPendingBan()
        {
            foreach (var kv in pendingBan)
                BanCard(kv.Key, kv.Value);
            pendingBan.Clear();
        }

        public void QueueBan(Card card, BanReason reason)
        {
            if (ExposedStatics.exemptFromPlayBan.Contains(card.Id))
                return;
            if (card.CardType == LBoL.Base.CardType.Friend
                && card.Summoned
                && !GetBanData(Battle).alreadySummoned.Contains(card))
            {
                GetBanData(Battle).alreadySummoned.Add(card);
                return;
            }
            pendingBan.AlwaysAdd(card, reason);
        }

        Dictionary<Card, BanReason> pendingBan = new Dictionary<Card, BanReason>();


        public Dictionary<Card, BanReason> bannedCards = new Dictionary<Card, BanReason>();

        internal HashSet<Card> alreadySummoned = new HashSet<Card>();


    }
}
