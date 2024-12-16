using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{

    public sealed class DoremyThreeLayeredPhantasmDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 1 };

            con.GunName = "Sweet01";


            con.Damage = 9;
            con.Value1 = 1;
            con.UpgradedValue1 = 2;

            con.Value2 = 3;
            con.UpgradedValue2 = 5;


            con.RelativeKeyword = Keyword.Ethereal | Keyword.Exile;
            con.UpgradedRelativeKeyword = Keyword.Ethereal | Keyword.Exile;




            return con;
        }
    }


    [EntityLogic(typeof(DoremyThreeLayeredPhantasmDef))]
    public sealed class DoremyThreeLayeredPhantasm : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            var cards = Battle.RollCards(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), Value2, cc => cc.Type == CardType.Attack);

            if (cards.Length != 0)
            {
                var interaction = new SelectCardInteraction(1, Value1, cards)
                {
                    Source = this
                };
                yield return new InteractionAction(interaction, false);

                foreach (var c in interaction.SelectedCards)
                {
                    c.IsEthereal = true;
                    c.IsExile = true;

                    if (c.Cost.Amount > 0)
                    {
                        ManaColor[] randomMana = c.Cost.EnumerateComponents().SampleManyOrAll(1, GameRun.BattleRng);
                        c.DecreaseBaseCost(ManaGroup.FromComponents(randomMana));
                    }
                    yield return new AddCardsToHandAction(new Card[] { c });
                
                }
            }

        }
    }
}
