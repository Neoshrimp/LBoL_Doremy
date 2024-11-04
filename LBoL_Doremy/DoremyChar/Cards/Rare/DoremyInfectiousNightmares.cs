using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyInfectiousNightmaresDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 2 };

            con.Value1 = 15;
            con.UpgradedValue1 = 25;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyInfectiousNightmaresDef))]
    public sealed class DoremyInfectiousNightmares : DCard
    {
        public NightmareInfo NM2Apply => Value1;


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return DebuffAction<DoremyInfectiousNightmaresSE>(selector.SelectedEnemy);
            yield return DebuffAction<DC_NightmareSE>(selector.SelectedEnemy, NM2Apply);
        }
    }

    public sealed class DoremyInfectiousNightmaresSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Special;
            con.HasLevel = false;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyInfectiousNightmaresSEDef))]
    public sealed class DoremyInfectiousNightmaresSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(unit.Died, OnDied);
        }

        private IEnumerable<BattleAction> OnDied(DieEventArgs args)
        {
            if (Battle.BattleShouldEnd)
                yield break;

            if (args.Unit.TryGetStatusEffect<DC_NightmareSE>(out var nightmare))
                yield return DebuffAction<DC_NightmareSE>(UnitSelector.RandomEnemy.GetEnemy(Battle), nightmare.Level);
        }
    }
}
