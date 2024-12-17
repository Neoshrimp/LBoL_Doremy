using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyWhiteNoiseDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };


            con.Mana = ManaGroup.Empty;

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;

            con.RelativeKeyword = Keyword.TempMorph | Keyword.Ethereal;
            con.UpgradedRelativeKeyword = Keyword.TempMorph | Keyword.Ethereal;




            return con;
        }
    }


    [EntityLogic(typeof(DoremyWhiteNoiseDef))]

    public sealed class DoremyWhiteNoise : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var card = Battle.RollCard(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), cc => cc.Type == CardType.Ability);

            if (card != null)
            {
                card.SetTurnCost(Mana);
                card.IsEthereal = true;
                yield return new AddCardsToHandAction(new Card[] { card });
            }
        }
    }
}
