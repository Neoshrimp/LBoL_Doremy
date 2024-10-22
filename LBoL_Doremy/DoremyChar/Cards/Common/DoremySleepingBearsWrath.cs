using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{

    public sealed class DoremySleepingBearsWrathDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.AllEnemies;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 2 };

            con.Damage = 12;

            con.Value1 = 8;
            con.UpgradedValue1 = 11;

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;



            return con;
        }
    }


    [EntityLogic(typeof(DoremySleepingBearsWrathDef))]
    public sealed class DoremySleepingBearsWrath : DreamLayerCard
    {
        protected override int AdditionalDamage => Value1 * DreamLevel;

    }
}
