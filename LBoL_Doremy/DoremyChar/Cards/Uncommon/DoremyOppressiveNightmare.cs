using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyOppressiveNightmareDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 1 };


            con.Value1 = 3;
            con.UpgradedValue1 = 5;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyOppressiveNightmareDef))]
    public sealed class DoremyOppressiveNightmare : DCard
    {

        public NightmareInfo NM2Apply => Value1;

        public static int buffPriority = 10;
        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(battle.CardUsed, OnSelfUsed, (GameEventPriority)buffPriority);
        }

        private IEnumerable<BattleAction> OnSelfUsed(CardUsingEventArgs args)
        {
            if(args.Card == this)
                yield return BuffAction<DoremyOppressiveNightmareSE>(Value1);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            return Enumerable.Empty<BattleAction>();
        }
    }

    public sealed class DoremyOppressiveNightmareSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;

            con.HasDuration = true;
            con.DurationStackType = StackType.Min;
            con.DurationDecreaseTiming = DurationDecreaseTiming.TurnEnd;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyOppressiveNightmareSEDef))]
    public sealed class DoremyOppressiveNightmareSE : DStatusEffect
    {



        protected override void OnAdded(Unit unit)
        {
            Duration = 1;

            ReactOwnerEvent(Battle.CardUsed, OnCardUsed, (GameEventPriority)(DoremyOppressiveNightmare.buffPriority - 1));
            ReactOwnerEvent(Battle.CardPlayed, OnCardUsed, (GameEventPriority)(DoremyOppressiveNightmare.buffPriority - 1));

            ReactOwnerEvent(unit.StatisticalTotalDamageDealt, OnKnife);
            ReactOwnerEvent(EventManager.DLEvents.appliedDL, OnDLApplied);
        }


        private IEnumerable<BattleAction> ApplyNightmare()
        {
            NotifyActivating();
            foreach (var e in UnitSelector.AllEnemies.GetEnemies(Battle))
                yield return NightmareAction(e, Level, 0f);
        }

        private IEnumerable<BattleAction> OnDLApplied(DreamLevelArgs arg)
        {
            return ApplyNightmare();
        }

        private IEnumerable<BattleAction> OnKnife(StatisticalDamageEventArgs args)
        {
            if(args.Cause == ActionCause.Card && args.ActionSource is Knife knife)
                return ApplyNightmare();

            return Enumerable.Empty<BattleAction>();
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs arg)
        {
           return ApplyNightmare();
        }
    }
}
