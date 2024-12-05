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
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };

            con.Value1 = 18;
            con.UpgradedValue1 = 23;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DoremyInfectiousNightmaresSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DoremyInfectiousNightmaresSE) };


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
            if (Battle.BattleShouldEnd)
                yield break;

            yield return NightmareAction(selector.SelectedEnemy, NM2Apply, 0f);
        }
    }

    public sealed class DoremyInfectiousNightmaresSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Negative;
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
            {
                var nextTarget = UnitSelector.RandomEnemy.GetEnemy(Battle);
                yield return DebuffAction<DoremyInfectiousNightmaresSE>(nextTarget);
                yield return NightmareAction(nextTarget, nightmare.Level);
            }
        }
    }
}
