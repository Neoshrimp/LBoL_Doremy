using LBoL.Base;
using LBoL.ConfigData;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Basic
{

    public sealed class DoremyCavalierDefenseWDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Common;
            con.IsPooled = false;

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 1, White = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 2 };

            con.Block = 10;
            con.UpgradedBlock = 13;
            

            con.Keywords = Keyword.Basic;
            con.UpgradedKeywords = Keyword.Basic;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyCavalierDefenseWDef))]
    public sealed class DoremyCavalierDefenseW : DCard
    {
    }
}
