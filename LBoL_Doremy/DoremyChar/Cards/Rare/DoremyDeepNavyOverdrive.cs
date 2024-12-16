using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Adventures.Shared12;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyDeepNavyOverdriveDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue, ManaColor.Black };
            con.Cost = new ManaGroup() { White = 1,  Blue = 1, Black = 1 };

            con.Mana = ManaGroup.Empty;

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Retain;

            con.RelativeKeyword = Keyword.TempMorph;
            con.UpgradedRelativeKeyword = Keyword.TempMorph;


            con.SubIllustrator = new string[] { Artists.Deeznuts };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyDeepNavyOverdriveDef))]
    public sealed class DoremyDeepNavyOverdrive : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyDeepNavyOverdriveSE>();
        }

        public override IEnumerable<BattleAction> AfterFollowPlayAction() => AfterUse(base.AfterFollowPlayAction());
        public override IEnumerable<BattleAction> AfterUseAction() => AfterUse(base.AfterUseAction());

        public IEnumerable<BattleAction> AfterUse(IEnumerable<BattleAction> baseActions)
        {
            foreach(var a in baseActions)
                yield return a;

            if (this.WasGenerated())
                this.SetTurnCost(Mana);
        }
    }


    public sealed class DoremyDeepNavyOverdriveSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = true;
            con.HasLevel = false;

            con.HasDuration = true;
            con.DurationDecreaseTiming = DurationDecreaseTiming.Custom;
            con.DurationStackType = StackType.Keep;


            return con;
        }
    }




    [EntityLogic(typeof(DoremyDeepNavyOverdriveSEDef))]
    public sealed class DoremyDeepNavyOverdriveSE : DStatusEffect
    {
        public ManaGroup Mana => ManaGroup.Empty;

        //List<Card> discountedCards = new List<Card>();

        protected override void OnAdded(Unit unit)
        {
            Duration = 1;
            SetTempCost(Battle.EnumerateAllCards().Where(c => c.WasGenerated()));
            ReactOnCardsAddedEvents(OnCardsCreated, (GameEventPriority)(-20));
            ReactOwnerEvent(unit.TurnStarted, OnTurnStarted, (GameEventPriority)999);
            ReactOwnerEvent(unit.TurnStarting, OnTurnStarting, (GameEventPriority)(-999));

        }

        private IEnumerable<BattleAction> OnTurnStarting(UnitEventArgs args)
        {
            ResetCreatedCardsCost();
            return Enumerable.Empty<BattleAction>();
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            Duration--;
            if (Duration <= 0)
                yield return new RemoveStatusEffectAction(this);
        }

        private IEnumerable<BattleAction> OnCardsCreated(Card[] cards, GameEventArgs args)
        {
            SetTempCost(cards);
            //discountedCards.AddRange(cards);
            return Enumerable.Empty<BattleAction>();
        }

        private static void SetTempCost(IEnumerable<Card> cards)
        {
            foreach (var c in cards)
                c.SetTurnCost(ManaGroup.Empty);
        }

        private void ResetCreatedCardsCost()
        {
            Battle.EnumerateAllCards().Where(c => c.WasGenerated()).Do(c => c.TurnCostDelta = ManaGroup.Empty);
            //discountedCards.Clear();
        }
    }
}
