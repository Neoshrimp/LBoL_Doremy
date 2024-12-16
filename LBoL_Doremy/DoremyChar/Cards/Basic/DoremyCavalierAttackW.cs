using LBoL.Base;
using LBoL.ConfigData;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Basic
{
    public sealed class DoremyCavalierAttackWDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Common;
            con.IsPooled = false;

            con.GunName = "ShootW1";

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 1, White = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 2 };

            con.Damage = 10;
            con.UpgradedDamage = 14;



            con.Keywords = Keyword.Basic;
            con.UpgradedKeywords = Keyword.Basic;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyCavalierAttackWDef))]
    public sealed class DoremyCavalierAttackW : DCard
    {
    }
}
