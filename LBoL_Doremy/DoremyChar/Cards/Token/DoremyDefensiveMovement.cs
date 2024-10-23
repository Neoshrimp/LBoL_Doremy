using LBoL.Base;
using LBoL.ConfigData;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Token
{
    public sealed class DoremyDefensiveMovementDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.IsPooled = false;
            con.HideMesuem = true;
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 0 };

            con.Block = 0;

            con.Shield = 8;
            con.UpgradedShield = 13;


            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDefensiveMovementDef))]
    public sealed class DoremyDefensiveMovement : DCard
    {
    }
}
