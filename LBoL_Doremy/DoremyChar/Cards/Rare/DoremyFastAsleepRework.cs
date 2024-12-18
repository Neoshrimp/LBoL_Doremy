using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Presentation.UI.Widgets;
using LBoL_Doremy.DoremyChar.DreamManagers;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.InputSystem.Controls;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{

    public sealed class DoremyFastAsleepReworkDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };

            con.Mana = new ManaGroup() { Any = 2 };
            con.Value1 = 3;


            con.UpgradedRelativeKeyword = Keyword.TempMorph;

            con.RelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyFastAsleepReworkDef))]
    public sealed class DoremyFastAsleepRework : DCard
    {
        public string UpgradeDesc => "";// IsUpgraded ? LocalizeProperty("UpgradeTxt", true).RuntimeFormat(FormatWrapper) : "";

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var buffAction = (ApplyStatusEffectAction)BuffAction<DoremyFastAsleepReworkSE>(Value1);
            yield return buffAction;
/*            if (IsUpgraded && Battle.Player.TryGetStatusEffect<DoremyFastAsleepReworkSE>(out var se))
                se.IsUpgraded = true;*/
        }
    }

    public sealed class DoremyFastAsleepReworkSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;
            con.HasLevel = true;
            con.HasCount = true;

            con.Order = DC_ExileQueueSE.exileQueuePriority + 1;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyFastAsleepReworkSEDef))]
    public sealed class DoremyFastAsleepReworkSE : DC_ExileQueueSE, IComparer<Card>
    {

        OrderedList<Card> _toBounceQueue = null;
        public OrderedList<Card> ToBounceQueue { 
            get 
            {  
                if(_toBounceQueue == null)
                    _toBounceQueue = new OrderedList<Card>(this);
                return _toBounceQueue; 
            } set => _toBounceQueue = value; 
        }

        public int Compare(Card c1, Card c2)
        {
            return (DoremyComatoseForm.IsPositive(c2) ? 1 : 0) - (DoremyComatoseForm.IsPositive(c1) ? 1 : 0);
        }


        public string QueuedCardsDesc => GetQueuedCardsDesc(ToBounceQueue);


        public NightmareInfo NM2ConsumeDesc => new NightmareInfo(Level, true);

        public NightmareInfo NM2Consume => new NightmareInfo(-Level, true);

        public string UpgradeDesc => "";// IsUpgraded ? LocalizeProperty("UpgradeTxt", true).RuntimeFormat(FormatWrapper) : "";

        public bool IsUpgraded { get => false; /*internal set;*/ }

        public ManaGroup Mana => new ManaGroup() { Any = 2 };

        protected override void OnAdded(Unit unit)
        {
            if (unit is PlayerUnit pu)
            {
                ReactOwnerEvent(Battle.CardMovingToDrawZone, OnShufflingIntoDrawpile, (GameEventPriority)10);
                ReactOwnerEvent(Battle.CardsAddedToDrawZone, OnAddingIntoDrawpile, (GameEventPriority)10);
                ReactOwnerEvent(pu.TurnStarted, TurnStarted, (GameEventPriority)exileQueuePriority + 1);
                ReactOwnerEvent(pu.TurnEnding, TurnEnding, (GameEventPriority)(DreamLayerHandlers.bouncePriority - 2));
            }
        }

        private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        {
            if (Owner.HasStatusEffect<DC_NightmareSE>())
                yield return NightmareAction(Owner, NM2Consume);
        }

        private IEnumerable<BattleAction> OnAddingIntoDrawpile(CardsAddingToDrawZoneEventArgs args)
        {
            if (args.DrawZoneTarget != DrawZoneTarget.Random)
                yield break;


            var exileAction = new ExileManyCardAction(args.Cards);
            yield return exileAction;
            if (!exileAction.IsCanceled)
                foreach(var c in exileAction._cards)
                    ToBounceQueue.Add(c);

            UpdateCount(ToBounceQueue);
        }



        protected override IEnumerable<Card> UpdateQueueContainer(IEnumerable<Card> queue)
        {
            ToBounceQueue = new OrderedList<Card>(queue, this);
            return ToBounceQueue;

        }

        private IEnumerable<BattleAction> OnShufflingIntoDrawpile(CardMovingToDrawZoneEventArgs args)
        {
            if(args.DrawZoneTarget != DrawZoneTarget.Random)
                yield break;

            args.CancelBy(this);

            var exileAction = new ExileCardAction(args.Card);
            yield return exileAction;
            if(!exileAction.IsCanceled)
                ToBounceQueue.Add(args.Card);

            UpdateCount(ToBounceQueue);
        }

        protected override void PostProcessCopy(Card copy)
        {
            if (IsUpgraded && !DoremyComatoseForm.IsPositive(copy))
            {
                copy.IsForbidden = false;
                copy.IsExile = true;
                copy.DecreaseTurnCost(Mana);
                copy.NotifyChanged();
            }
        }

        private IEnumerable<BattleAction> TurnStarted(UnitEventArgs args)
        {
            foreach (var a in ProcessQueue(ToBounceQueue.ToList()))
                yield return a;

            UpdateCount(ToBounceQueue);

        }



        [HarmonyPatch()]
        class CardWidget_ShowMana_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(CardWidget), "SetProperties");

            }

            static void Prefix(CardWidget __instance)
            {
                if (__instance._card == null)
                    return;

                __instance._dontShowMana = __instance.Card.IsForbidden;
                __instance.costWidget.gameObject.SetActive(!__instance._dontShowMana);

            }
        }


    }
}
