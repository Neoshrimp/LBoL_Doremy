using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.BattleTracking;
using System.Linq;
using LBoL_Doremy.Utils;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{
    public sealed class DoremyDejaVuDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;
            con.Rarity = Rarity.Uncommon;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Green };
            con.Cost = new ManaGroup() { Hybrid = 1, HybridColor = 3, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 1 };


            con.Mana = new ManaGroup() { Hybrid = 1, HybridColor = 3};

            con.RelativeKeyword = Keyword.TempMorph;
            con.UpgradedRelativeKeyword = Keyword.TempMorph;


            con.RelativeEffects = new List<string>() { };
            con.UpgradedRelativeEffects = new List<string>() { };

            return con;
        }
    }

    [EntityLogic(typeof(DoremyDejaVuDef))]
    public sealed class DoremyDejaVu : DCard
    {

        public string FirstLegalCardName => FirstLegalCard?.ColorName() ?? StringDecorator.Decorate("|e:N/A|");

        Card FirstLegalCard => RealBattle == null ? null : BattleHistoryHandlers.GetInfo(RealBattle).lastCardUseTurnHist.FirstOrDefault(c => c.CanBeDuplicated);

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {

            var card = FirstLegalCard;
            if (card != null)
            {
                var copy = card.CloneBattleCard();
                copy.IsEthereal = true;
                copy.IsExile = true;
                copy.SetTurnCost(Mana);
                yield return new AddCardsToHandAction(copy);

            }
        }
    }

}
