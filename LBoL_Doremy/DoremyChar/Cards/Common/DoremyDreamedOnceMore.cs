using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Battle.Interactions;
using System.Linq;
using LBoL.Core.Battle.BattleActions;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyDreamedOnceMoreDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue, ManaColor.White };
            con.Cost = new ManaGroup() { Blue = 1, White = 1 };

            con.Block = 5;
            con.UpgradedBlock = 8;

            con.Keywords = Keyword.Exile;

            con.RelativeKeyword = Keyword.CopyHint;
            con.UpgradedRelativeKeyword = Keyword.CopyHint;

            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamedOnceMoreDef))]
    public sealed class DoremyDreamedOnceMore : DCard
    {

        public override Interaction Precondition()
        {
            var discard = Battle.DiscardZone.Where(c => c != this);
            if (discard.FirstOrDefault() == null)
                return null;
            return new SelectCardInteraction(1, 1, discard);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            if (precondition is SelectCardInteraction interaction)
            {
                var target = interaction.SelectedCards.FirstOrDefault();
                if (target != null)
                {
                    yield return new MoveCardAction(target, LBoL.Core.Cards.CardZone.Hand);

                    if (target is DreamLayerCard dlc)
                    {
                        var copy = dlc.CloneBattleCard() as DreamLayerCard;
                        copy.IsExile = true;
                        yield return new AddCardsToHandAction(copy);
                    }
                }
            }

        }
    }
}
