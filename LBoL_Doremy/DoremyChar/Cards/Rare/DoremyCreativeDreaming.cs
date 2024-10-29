using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyCreativeDreamingDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 3 };
            con.UpgradedCost = new ManaGroup() { White = 1, Any = 1 };

            con.Value1 = 1;

            con.RelativeKeyword = Keyword.Ethereal;
            con.UpgradedRelativeKeyword = Keyword.Ethereal;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyCreativeDreamingDef))]
    public sealed class DoremyCreativeDreaming : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyCreativeDreamingSE>(Value1);
        }
    }

    public sealed class DoremyCreativeDreamingSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyCreativeDreamingSEDef))]
    public sealed class DoremyCreativeDreamingSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(unit.TurnStarting, OnTurnStarting);
        }

        private IEnumerable<BattleAction> OnTurnStarting(UnitEventArgs args)
        {

            for (int i = 0; i <Level; i++)
            {
                var card = Battle.RollCard(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), cc => cc.Type == CardType.Ability);

                if (card != null)
                {
                    card.IsEthereal = true;
                    yield return new AddCardsToHandAction(card);
                }
            }

        }
    }
}
