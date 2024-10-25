using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyDreamBalloonFlightDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 2 };
            con.UpgradedCost = new ManaGroup() { Blue = 1, Any = 1 };


            con.Value1 = 1;


            con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DLKwSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamBalloonFlightDef))]
    public sealed class DoremyDreamBalloonFlight : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyDreamBalloonFlightSE>(Value1);
        }
    }


    public sealed class DoremyDreamBalloonFlightSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyDreamBalloonFlightSEDef))]
    public sealed class DoremyDreamBalloonFlightSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            unit.ReactBattleEvent(EventManager.DLEvents.appliedDL, OnDLGained);
        }

        private IEnumerable<BattleAction> OnDLGained(DreamLevelArgs arg)
        {
            yield return BuffAction<DoremyExtraDrawSE>(Level);
        }
    }
}
