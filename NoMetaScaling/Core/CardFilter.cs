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
using LBoL.EntityLib.Cards.Neutral.Blue;
using LBoL.EntityLib.Cards.Misfortune;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.Cards.Adventure;
using LBoL.EntityLib.Cards.Enemy;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.Black;

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

            NoMetaScalinAPI.ExemptFromBanIfGenerated(nameof(BuyPeace));
            NoMetaScalinAPI.ExemptFromBanIfGenerated(nameof(Psychedelic));
            NoMetaScalinAPI.ExemptFromBanIfGenerated(nameof(PoisonTeaParty));

            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(VampireShoot1));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(VampireShoot2));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(NewsPositive));

            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(ReimuSakura));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(QingeUpgrade));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(XingMoney));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(Shengyan));


            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(BuyPeace));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(Psychedelic));
            NoMetaScalinAPI.ExemptFromBanIfPlayed(nameof(PoisonTeaParty));

            NoMetaScalinAPI.AddBanByDefault(nameof(FakeMoon));

            

            RegisterCommonHandlers(OnCardCreated);

            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardUsed, OnCardUsedPlayed, null, (GameEventPriority)9999);

            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardPlayed, OnCardUsedPlayed, null, (GameEventPriority)9999);

            //CHandlerManager.RegisterBattleEventHandler(bt => bt.CardUsing, OnCardUsing, null, GameEventPriority.Lowest);
            

        }

        private static void OnCardUsedPlayed(CardUsingEventArgs args)
        {
            var card = args.Card;
            if (card.CardType == LBoL.Base.CardType.Friend
                && card.Summoned
                && !GetBanData(Battle).alreadySummoned.Contains(card))
            {
                GetBanData(Battle).alreadySummoned.Add(card);
            }
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
            var banData = GetBanData(Battle);
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

                // exception since changing this properly is too much work
                if (sourceCard.Id == nameof(BailianMagic) && sourceCard.IsUpgraded && banData.GetCopiedTimes(sourceCard) < 1)
                {
                    banData.IncreaseCopyTimes(sourceCard);
                }
                else
                    banData.QueueBan(sourceCard, BanReason.CardWasAlreadyUsed);
            }
            else
            {
                reason = BanReason.CardWasCopied;
                banData.QueueBan(sourceCard, BanReason.CardWasAlreadyUsed);
            }
            
            return doBan;
        }

        private static void OnCardCreated(Card[] cards, GameEventArgs args)
        {
            if (args.ActionSource.TrickleDownActionSource() is Card sourceCard)
            {
                var banData = GetBanData(Battle);

                if (sourceCard.Id == nameof(Frog))
                    goto end;

                foreach (var addedCard in cards)
                {
                    // will never be true as addCardAction assigns new instance id
                    /*if (!addedCard.WasGenerated())
                        continue;*/

                    if (!sourceCard.IsBanned(out var _) && ExposedStatics.exemptFromGenBan.Contains(addedCard.Id))
                        continue;

                    bool doBan = true;
                    BanReason reason = BanReason.CardWasGenerated;



                    // real gen clause
                    if (PConfig.AllowFirstTimeDeckedGen 
                        //&& args.ActionSource is Card // source is an actual card, not SE
                        && !sourceCard.IsBanned(out var _) && !sourceCard.WasGenerated()
                        && !banData.IsGeneratorBanned(sourceCard)
                        )
                    {
                        banData.QueueGenBan(sourceCard);
                        if (addedCard.Id == nameof(FakeMoon))
                            doBan = true;
                        else
                            continue;
                    }

                    // natural echo clause
                    if (!sourceCard.IsBanned(out var _) && sourceCard.InvokedEcho() 
                        && (sourceCard.IsNaturalEcho() || sourceCard.IsNaturalPermaEcho()))
                    { 
                        if(sourceCard.IsNaturalPermaEcho())
                            banData.QueueBan(sourceCard, BanReason.CardWasAlreadyUsed);

                        continue;
                    }

                    if (GetCopyHistory(Battle).IfWasCopiedForget(addedCard, out var copyPair))
                        doBan = HandleCopying(sourceCard, copyPair.original, out reason);


                    if (doBan)
                    {
                        banData.BanCard(addedCard, reason);
                    }


                }
            }
            end:
                return;
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

            foreach (var c in pendingGeneratorBan)
                BanGenerator(c);

            pendingGeneratorBan.Clear();
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

        public int GetCopiedTimes(Card card)
        {
            if (CopiedTimes.TryGetValue(card, out var times))
                return times;
            return 0;
        }

        public void IncreaseCopyTimes(Card card, int increase = 1)
        {
            if (!CopiedTimes.ContainsKey(card))
                CopiedTimes.Add(card, 0);


            CopiedTimes[card] += increase;
        }

        public void QueueGenBan(Card card)
        {
            pendingGeneratorBan.Add(card);
        }

        public void BanGenerator(Card card)
        {
            bannedGenerators.Add(card);
        }

        public bool IsGeneratorBanned(Card card) => bannedGenerators.Contains(card);


        HashSet<Card> pendingGeneratorBan = new HashSet<Card>();

        HashSet<Card> bannedGenerators = new HashSet<Card>();

        Dictionary<Card, BanReason> pendingBan = new Dictionary<Card, BanReason>();


        Dictionary<Card, BanReason> bannedCards = new Dictionary<Card, BanReason>();

        internal HashSet<Card> alreadySummoned = new HashSet<Card>();

        Dictionary<Card, int> copiedTimes;

        Dictionary<Card, int> CopiedTimes
        {
            get
            {
                if (copiedTimes == null)
                    copiedTimes = new Dictionary<Card, int>();
                return copiedTimes;
            }
        }
    }
}
