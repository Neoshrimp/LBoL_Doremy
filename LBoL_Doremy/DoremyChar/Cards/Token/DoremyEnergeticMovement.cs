using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Token
{

    public sealed class DoremyEnergeticMovementDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.IsPooled = false;
            con.HideMesuem = true;
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 0 };

            con.Value1 = 2;
            con.UpgradedValue1 = 3;

            con.Mana = new ManaGroup() { White = 1 };
            con.UpgradedMana = new ManaGroup() { Philosophy = 1 };

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;



            return con;
        }
    }


    [EntityLogic(typeof(DoremyEnergeticMovementDef))]
    public sealed class DoremyEnergeticMovement : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new DrawManyCardAction(Value1);
            yield return new GainManaAction(Mana);

        }
    }
}
