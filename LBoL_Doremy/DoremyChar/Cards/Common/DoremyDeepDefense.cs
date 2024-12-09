using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using System.Linq;
using LBoL_Doremy.DoremyChar.Actions;
using LBoLEntitySideloader.CustomKeywords;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{

    public sealed class DoremyDeepDefenseDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 2 };

            con.Block = 12;
            //con.UpgradedBlock = 17;

            con.Keywords = Keyword.Debut;
            con.UpgradedKeywords = Keyword.Debut;


            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE), nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE), nameof(DC_DLKwSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDeepDefenseDef))]
    public sealed class DoremyDeepDefense : DCard
    {


        public string DebutDesc
        {
            get
            {
                var dkey = "Debut";
                if (IsUpgraded)
                    dkey = "UpgradedDebut";

                var desc = LocalizeProperty(dkey, true).RuntimeFormat(FormatWrapper);
                if (!DebutActive)
                    desc = StringDecorator.Decorate(string.Concat("|d:", desc, "|"));

                return desc;
            }
        }

        public override Interaction Precondition()
        {
            var hand = base.Battle.HandZone.Where(c => c != this);
            if (hand.FirstOrDefault() == null)
                return null;
            return new SelectHandInteraction(1, 1, hand);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            if (precondition is SelectHandInteraction interaction)
            {
                var target = interaction.SelectedCards.FirstOrDefault();
                if (target != null)
                {
                    target.AddCustomKeyword(DoremyKw.NewDreamLayer);
                    if (DebutActive)
                    { 
                        yield return new ApplyDLAction(target);
                        if(IsUpgraded)
                            yield return new ApplyDLAction(target);
                    }
                }
            }

        }
    }
}
