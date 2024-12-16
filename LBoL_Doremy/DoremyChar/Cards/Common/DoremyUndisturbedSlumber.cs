﻿using BepInEx.Logging;
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
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyUndisturbedSlumberDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2, Any = 1 };

            con.Shield = 15;
            con.UpgradedShield = 20;

            con.Value1 = 5;
            con.UpgradedValue1 = 8;


            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyUndisturbedSlumberDef))]
    public sealed class DoremyUndisturbedSlumber : DCard
    {

        public NightmareInfo NM2Apply => Value1;
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {


            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            yield return PerformAction.Gun(Battle.Player, Battle.AllAliveEnemies.FirstOrDefault(), "飞光虫");

            foreach (var e in UnitSelector.AllEnemies.GetUnits(Battle))
                yield return NightmareAction(e, NM2Apply, 0f);

        }
    }
}
