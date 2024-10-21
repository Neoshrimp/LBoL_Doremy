using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyOutColdDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White } ;
            con.Cost = new ManaGroup() { White = 1, Any = 1 };

            con.Block = 20;
            con.UpgradedBlock = 25;

            con.Value1 = 5;
            con.UpgradedValue1 = 7;

            con.Value2 = 1;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyOutColdDef))]
    public sealed class DoremyOutCold : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            yield return DebuffAction<DC_NightmareSE>(UnitSelector.RandomEnemy.GetEnemy(Battle), Value1);

            yield return new LockRandomTurnManaAction(Value2);

        }
    }
}
