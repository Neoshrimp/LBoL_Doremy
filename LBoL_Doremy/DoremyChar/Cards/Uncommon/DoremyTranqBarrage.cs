﻿using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.BattleTracking;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyTranqBarrageDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.RandomEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2, Any = 2 };


            con.Value1 = 10;
            con.UpgradedValue1 = 13;



            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_DLKwSE) };

            return con;
        }
    }



    [EntityLogic(typeof(DoremyTranqBarrageDef))]
    public sealed class DoremyTranqBarrage : DCard
    {

        public int ApplyTimes => (Battle == null ? 0 : BattleHistoryHandlers.DLHistory.applyTurnCount) + 1;

        public static int countResetPriorityOffset = -5;

        public NightmareInfo NM2Apply => Value1;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            for (int i = 0; i < ApplyTimes; i++)
            { 
                if (Battle.BattleShouldEnd)
                    yield break;
                var e = UnitSelector.RandomEnemy.GetEnemy(Battle);
                yield return PerformAction.Gun(Battle.Player, e, "颠茄B");
                yield return NightmareAction(e, NM2Apply, occupationTime: 0.05f);
            }
        }
    }
}
