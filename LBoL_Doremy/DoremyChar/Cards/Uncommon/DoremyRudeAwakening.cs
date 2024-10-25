using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{

    public sealed class DoremyRudeAwakeningDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };


            con.Value1 = 2;
            con.UpgradedValue1 = 3;

            con.Value2 = 1;

            con.RelativeEffects = new List<string>() { nameof(Firepower) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(Firepower) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyRudeAwakeningDef))]
    public sealed class DoremyRudeAwakening : DreamLayerCard
    {
        public int Fp2Apply => Value1 + DreamLevel;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<Firepower>(Fp2Apply);
        }
    }
}
