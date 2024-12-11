using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{

    public /*sealed*/ class DoremyFastAsleepDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };


            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };


            return con;
        }
    }


    //[EntityLogic(typeof(DoremyFastAsleepDef))]
    public /*sealed*/ class DoremyFastAsleep : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyFastAsleepSE>();
        }
    }

    public /*sealed*/ class DoremyFastAsleepSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;
            con.HasLevel = false;

            return con;
        }
    }

    //[EntityLogic(typeof(DoremyFastAsleepSEDef))]
    public /*sealed*/ class DoremyFastAsleepSE : DStatusEffect
    {
        // checked in DreamLayerCard
    }
}
