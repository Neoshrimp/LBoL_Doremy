using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyDreamshieldDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = ManaGroup.Empty;

            con.Block = 6;

            con.Value1 = 1;

            con.Keywords = Keyword.Forbidden;
            con.UpgradedKeywords = Keyword.Forbidden | Keyword.Replenish;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamshieldDef))]
    public sealed class DoremyDreamshield : DreamLayerCard
    {
        //public override bool ShowDreamLevel => false;

        public override void OnDLChanged(DreamLevelArgs args)
        {
            React(new CastBlockShieldAction(Battle.Player, Block));
            React(BuffAction<DoremyExtraDrawSE>(Value1));
        }
    }
}
