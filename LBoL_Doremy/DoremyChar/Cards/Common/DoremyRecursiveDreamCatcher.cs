using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyRecursiveDreamCatcherDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1 };

            con.Mana = new ManaGroup() { White = 1 };

            con.Block = 8;
            con.UpgradedBlock = 11;


            con.Keywords = Keyword.Exile | Keyword.Echo;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Echo;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyRecursiveDreamCatcherDef))]

    public sealed class DoremyRecursiveDreamCatcher : DCard
    {

        protected override void OnEnterBattle(BattleController battle)
        {
        }
    }
}
