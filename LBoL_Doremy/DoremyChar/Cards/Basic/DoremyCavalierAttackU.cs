using LBoL.Base;
using LBoL.ConfigData;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Basic
{


    public sealed class DoremyCavalierAttackUDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Common;
            con.IsPooled = false;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() {  ManaColor.Blue };
            con.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 2 };

            con.Damage = 10;
            con.UpgradedDamage = 14;



            con.Keywords = Keyword.Basic;
            con.UpgradedKeywords = Keyword.Basic;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyCavalierAttackUDef))]
    public sealed class DoremyCavalierAttackU : DCard
    {
    }
}
