using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.Actions;

using LBoL_Doremy.DoremyChar.DreamManagers;
using LBoL_Doremy.DoremyChar.Keywords;
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

            con.Block = 4;

            con.Value1 = 1;
            con.Value2 = 2;


            //con.Keywords = Keyword.Forbidden;
            con.UpgradedKeywords = /*Keyword.Forbidden |*/ Keyword.Replenish;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamshieldDef))]
    public sealed class DoremyDreamshield : NaturalDreamLayerCard
    {
        //public override bool ShowDreamLevel => false;

        public NightmareInfo NM2Apply => Value2;

        public override void OnDLChanged(DreamLevelArgs args)
        {
            React(new CastBlockShieldAction(Battle.Player, Block));
            React(BuffAction<DoremyExtraDrawSE>(Value1));
            React(NightmareAction(Battle.Player, NM2Apply));
        }
    }
}
