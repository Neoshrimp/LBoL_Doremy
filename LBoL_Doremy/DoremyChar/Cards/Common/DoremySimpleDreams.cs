using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremySimpleDreamsDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 1 };


            con.RelativeCards = new List<string>() { nameof(DoremySoporificNeedles), nameof(DoremySoporificShield) };

            con.UpgradedRelativeCards = new List<string>() { nameof(DoremySoporificNeedles), nameof(DoremySoporificShield), nameof(DoremySoporificCreativity) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremySimpleDreamsDef))]
    public sealed class DoremySimpleDreams : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var cards = EnumerateRelativeCards();

            if (cards.Count() != 0)
            {
                MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(cards)
                {
                    Source = this
                };
                yield return new InteractionAction(interaction, false);
                Card selectedCard = interaction.SelectedCard;
                yield return new AddCardsToHandAction(new Card[] { selectedCard });
            }
        }

    }
}
