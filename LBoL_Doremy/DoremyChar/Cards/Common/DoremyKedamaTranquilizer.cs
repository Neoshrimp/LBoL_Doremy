﻿using LBoL.Base;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.ConfigData;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoLEntitySideloader.Attributes;
using LBoL.Core.Battle.BattleActions;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyKedamaTranquilizerDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };


            con.Value1 = 16;
            con.UpgradedValue1 = 22;



            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }

    [EntityLogic(typeof(DoremyKedamaTranquilizerDef))]
    public sealed class DoremyKedamaTranquilizer : DCard
    {


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            // 2do add gun id
            yield return PerformAction.Gun(Battle.Player, selector.SelectedEnemy, "Simple1");
            yield return DebuffAction<DC_NightmareSE>(selector.SelectedEnemy, Value1);
        }
    }
}