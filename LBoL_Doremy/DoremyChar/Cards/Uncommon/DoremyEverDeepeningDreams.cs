using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.DoremyChar.Actions;

using LBoL_Doremy.DoremyChar.DreamManagers;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.CustomKeywords;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{

    public sealed class DoremyEverDeepeningDreamsDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { White = 1, Blue = 1 };

            con.UpgradedCost = new ManaGroup() { Hybrid = 1, HybridColor = 0 };



            con.Value1 = 1;
            con.Mana = ManaGroup.Empty;

            con.Value2 = 1;
            con.UpgradedValue2 = 1;


            con.RelativeKeyword = Keyword.TempMorph;
            con.UpgradedRelativeKeyword = Keyword.TempMorph;

            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremyEverDeepeningDreamsDef))]
    public sealed class DoremyEverDeepeningDreams : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var cards = Battle.RollCards(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), 3, cc => NaturalDreamLayerCard.AllDreamLayerCards.Contains(cc.Id));

            cards.Do(card => {
                if (card is NaturalDreamLayerCard)
                {
                    var dl = card.GetCustomKeyword<DLKeyword>(DoremyKw.dLId);
                    dl.DreamLevel += Value2;
                }

            });

            if (cards.Length != 0)
            {
                MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(cards)
                {
                    Source = this
                };
                yield return new InteractionAction(interaction);
                Card card = interaction.SelectedCard;
                card.SetTurnCost(Mana);

                yield return new AddCardsToHandAction(new Card[] { card });

            }
        }
    }
}
