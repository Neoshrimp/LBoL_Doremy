using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Units;
using LBoL.Core.Randoms;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyGatherDreamsDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 2 };
            con.UpgradedCost = new ManaGroup() { White = 1 };


            con.Value1 = 3;

            con.Keywords = Keyword.Initial;
            con.UpgradedKeywords = Keyword.Initial;



            return con;
        }
    }


    [EntityLogic(typeof(DoremyGatherDreamsDef))]
    public sealed class DoremyGatherDreams : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyGatherDreamsSE>(1, count: Value1);
        }
    }


    public sealed class DoremyGatherDreamsSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;

            con.HasCount = true;
            con.CountStackType = StackType.Max;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyGatherDreamsSEDef))]
    public sealed class DoremyGatherDreamsSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(unit.TurnEnding, OnTurnEnding);
        }

        private IEnumerable<BattleAction> OnTurnEnding(UnitEventArgs args)
        {
            if (Battle.BattleShouldEnd)
                yield break;
            for (int i = 0; i < Level; i++)
            {
                var cards = Battle.RollCards(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), Count);

                if (cards.Length != 0)
                {
                    MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(cards, isAddCardToDeck: true, canSkip: true)
                    {
                        Source = this
                    };
                    yield return new InteractionAction(interaction, false);
                    Card selectedCard = interaction.SelectedCard;
                    if(selectedCard != null)
                        yield return new AddCardsToDrawZoneAction(new Card[] { selectedCard }, DrawZoneTarget.Random);
                }

            }
        }
    }
}
