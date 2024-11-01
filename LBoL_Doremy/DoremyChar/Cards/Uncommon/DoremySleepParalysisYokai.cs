using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremySleepParalysisYokaiDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.AllEnemies;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2, Any = 2 };
            con.UpgradedCost = new ManaGroup() { Blue = 1, Any = 1 };



            con.Value1 = 3;
            //con.UpgradedValue1 = 5;


            con.RelativeEffects = new List<string>() { nameof(TempFirepowerNegative), nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(TempFirepowerNegative), nameof(DC_NightmareSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremySleepParalysisYokaiDef))]
    public sealed class DoremySleepParalysisYokai : DreamLayerCard
    {

        public int DreamLevelCapped => Math.Min(Value1, DreamLevel);

        public string DreamLevelCappedDesc => Battle == null ? "" : $" ({LightBlue}{DreamLevelCapped}{CC})";
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var e in selector.GetEnemies(Battle))
            {
                if (Battle.BattleShouldEnd)
                    yield break;
                if (e.TryGetStatusEffect<DC_NightmareSE>(out var nightmare))
                { 
                    yield return new DamageAction(Battle.Player, e, DamageInfo.HpLose(nightmare.Level / 2f));
                }
            }
        }

        public override void OnDLChanged(DreamLevelArgs args)
        {
            foreach (var e in UnitSelector.AllEnemies.GetEnemies(Battle))
                React(DebuffAction<TempFirepowerNegative>(e, DreamLevelCapped));
        }
    }
}
