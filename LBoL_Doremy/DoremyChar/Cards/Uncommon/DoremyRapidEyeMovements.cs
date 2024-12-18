﻿using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyRapidEyeMovementsDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 2 };

            con.Block = 8;

            con.Value1 = 1;
            con.UpgradedValue1 = 2;

            con.Value2 = 3;

            con.RelativeCards = new List<string>() { nameof(DoremyConfoundingMovement), nameof(DoremyDefensiveMovement), nameof(DoremyEnergeticMovement) };
            con.UpgradedRelativeCards = new List<string>() { nameof(DoremyConfoundingMovement), nameof(DoremyDefensiveMovement), nameof(DoremyEnergeticMovement) };


            return con;
        }
    }



    [EntityLogic(typeof(DoremyRapidEyeMovementsDef))]
    public sealed class DoremyRapidEyeMovements : DCard
    {

        public override Interaction Precondition()
        {
            var selection = new SelectCardInteraction(1, Value1, EnumerateRelativeCards());
            if (IsUpgraded)
                selection.Description = Name + (IsUpgraded ? "+" : "") + LocalizeProperty("UpTo", true).RuntimeFormat(FormatWrapper);

            return selection;
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {

            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            if (precondition is SelectCardInteraction interaction && interaction.SelectedCards.Count > 0)
            {
                yield return new AddCardsToHandAction(interaction.SelectedCards);
            }



        }
    }
}
