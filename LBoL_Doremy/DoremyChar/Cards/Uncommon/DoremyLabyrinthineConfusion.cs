using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{

    public sealed class DoremyLabyrinthineConfusionDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2 };


            con.Block = 0;

            con.Shield = 0;

            con.Value1 = 7;
            con.UpgradedValue1 = 10;

            con.Value2 = 2;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyLabyrinthineConfusionDef))]
    public sealed class DoremyLabyrinthineConfusion : DCard
    {
        protected override int AdditionalBlock => IsUpgraded ? 0 : Value1 * GenCount;
        protected override int AdditionalShield => IsUpgraded ? Value1 * GenCount : 0;

        int GenCount => Battle == null ? 0 : Battle.HandZone.Where(c => c.WasGenerated() && c != this).Count();

        public int NM2Apply => Value2 * GenCount;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            foreach (var e in UnitSelector.AllEnemies.GetUnits(Battle))
                yield return DebuffAction<DC_NightmareSE>(e, NM2Apply);
        }
    }

}
