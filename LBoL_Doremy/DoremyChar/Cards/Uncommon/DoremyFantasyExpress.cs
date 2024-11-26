using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL_Doremy.DoremyChar.BattleTracking;
using LBoL_Doremy.DoremyChar.Actions;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyFantasyExpressDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 3 };
            //con.UpgradedCost = new ManaGroup() { White = 1, Any = 2 };


            con.Damage = 12;
            con.Value1 = 12;
            con.Value2 = 1;

            con.Keywords = Keyword.Accuracy | Keyword.Exile;
            con.UpgradedKeywords = Keyword.Accuracy;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyFantasyExpressDef))]
    public sealed class DoremyFantasyExpress : DCard
    {
        protected override int AdditionalDamage => Value2 * CreatedCount;

        public string CreatedCountString => RealBattle == null ? "N/A" : CreatedCount.ToString();

        int CreatedCount { get => IsUpgraded ? BattleHistoryHandlers.GetCreatedCount(RealBattle).Total : BattleHistoryHandlers.GetCreatedCount(RealBattle).byPlayer; }

        public NightmareInfo NM2Apply { get => Value1 + Value2 * CreatedCount; }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            yield return NightmareAction(selector.SelectedEnemy, NM2Apply);
        }

    }
}
