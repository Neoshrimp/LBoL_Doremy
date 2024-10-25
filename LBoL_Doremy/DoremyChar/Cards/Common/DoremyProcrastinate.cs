using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyProcrastinateDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;
            con.Rarity = Rarity.Common;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 2 };

            con.Block = 18;
            con.UpgradedBlock = 22;

            con.Value1 = 2;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyProcrastinateDef))]
    public sealed class DoremyProcrastinate : DCard
    {

        public override Interaction Precondition()
        {
            var hand = Battle.HandZone.Where(c => c != this);
            if (hand.FirstOrDefault() == null)
                return null;
            return new SelectCardInteraction(0, 2, hand);
        }


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            IEnumerable<Card> cards = new List<Card>();
            if (precondition is SelectCardInteraction interaction)
            {
                cards = interaction.SelectedCards;
                yield return new ExileManyCardAction(cards);
                if (cards.FirstOrDefault() != null)
                {
                    yield return BuffAction<DoremyProcrastinateSE>();
                    var status = Battle.Player.StatusEffects.FirstOrDefault(se => se.SourceCard == this) as DoremyProcrastinateSE;
                    if (status != null)
                        status.UpdateCards2Bounce(cards);
                }
            }

        }
    }



    public sealed class DoremyProcrastinateSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = false;
            con.HasCount = true;
            con.HasLevel = false;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyProcrastinateSEDef))]
    public sealed class DoremyProcrastinateSE : DStatusEffect
    {

        List<Card> _cards2Bounce = new List<Card>();
        public IReadOnlyList<Card> Cards2Bounce { get => _cards2Bounce.AsReadOnly(); }

        public void UpdateCards2Bounce(IEnumerable<Card> cards)
        {
            _cards2Bounce.AddRange(cards.Where(c => !c.IsCopy));
            Count = Cards2Bounce.Count;
            NotifyChanged();
        }


        public string QueuedCards
        {
            get
            {
                if (Cards2Bounce.Count == 0)
                    return LocalizeProperty("Nothing");
                return string.Join(", ", Cards2Bounce.Select(c => StringDecorator.Decorate($"|{c.Name}{(c.IsUpgraded ? "+" : "")}|")));
            }
        }

        protected override void OnAdded(Unit unit)
        {
            if (unit is PlayerUnit pu)
            {
                ReactOwnerEvent(pu.TurnStarted, TurnStarted);
            }
            Count = 0;
        }

        private IEnumerable<BattleAction> TurnStarted(UnitEventArgs args)
        {
            if (Battle.BattleShouldEnd)
                yield break;

            NotifyActivating();

            yield return new AddCardsToHandAction(Cards2Bounce.Select(c => c.CloneBattleCard()));
            _cards2Bounce.Clear();
            Count = 0;

            yield return new RemoveStatusEffectAction(this);
        }
    }
}
