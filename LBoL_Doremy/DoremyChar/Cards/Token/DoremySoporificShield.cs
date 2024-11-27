using LBoL.Base;
using LBoL.ConfigData;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Token
{
    public sealed class DoremySoporificShieldDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();


            con.IsPooled = false;
            con.HideMesuem = true;
            con.Rarity = Rarity.Common;

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1 };



            con.Block = 15;
            con.UpgradedBlock = 0;

            con.Shield = 0;
            con.UpgradedShield = 20;

            con.Keywords = Keyword.Exile | Keyword.Retain;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Retain;


            return con;
        }
    }

    [EntityLogic(typeof(DoremySoporificShieldDef))]
    public sealed class DoremySoporificShield : DCard
    {
    }
}
