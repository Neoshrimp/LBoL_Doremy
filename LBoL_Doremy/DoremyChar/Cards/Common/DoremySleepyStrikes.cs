using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremySleepyStrikesDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;
            con.Rarity = Rarity.Common;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2 };

            con.Damage = 16;
            con.UpgradedDamage = 11;
            con.Value1 = 1;
            con.UpgradedValue1 = 2;
            con.Value2 = 3;

            return con;
        }
    }

    [EntityLogic(typeof(DoremySleepyStrikesDef))]
    public sealed class DoremySleepyStrikes : DCard
    {
        protected override void SetGuns()
        {
            CardGuns = new Guns(GunName, Value1);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach(var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            if (Battle.BattleShouldEnd)
                yield break;

            var cards = Battle.RollCards(new CardWeightTable(RarityWeightTable.OnlyCommon, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), Value2, cc => cc.Type == CardType.Attack && cc.Id != Id);

            if (cards.Length != 0)
            {
                MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(cards)
                {
                    Source = this
                };
                yield return new InteractionAction(interaction);
                Card selectedCard = interaction.SelectedCard;
                selectedCard.IsEthereal = true;
                selectedCard.IsExile = true;
                yield return new AddCardsToHandAction(new Card[] { selectedCard });
            }

        }

    }
}
