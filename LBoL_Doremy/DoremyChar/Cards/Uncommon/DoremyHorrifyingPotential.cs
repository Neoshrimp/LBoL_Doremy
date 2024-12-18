using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyHorrifyingPotentialDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { Hybrid = 2, HybridColor = 0 };


            con.Value1 = 2;
            con.UpgradedValue1 = 3;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };

            con.Illustrator = Artists.kimmchu;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyHorrifyingPotentialDef))]
    public sealed class DoremyHorrifyingPotential : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyHorrifyingPotentialSE>(Value1);
        }
    }

    public sealed class DoremyHorrifyingPotentialSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyHorrifyingPotentialSEDef))]
    public sealed class DoremyHorrifyingPotentialSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(EventManager.DLEvents.appliedDL, OnDLApplied);
            ReactOnCardsAddedEvents(OnCardsAdded);
        }

        private IEnumerable<BattleAction> ApplyNightmare()
        {
            if (Battle.BattleShouldEnd)
                yield break;
            NotifyActivating();
            var e = UnitSelector.RandomEnemy.GetEnemy(Battle);
            //foreach (var e in UnitSelector.AllEnemies.GetEnemies(Battle))
            yield return NightmareAction(e, Level, 0f);
        }

        private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
        {
            foreach (var c in cards)
            { 
                if (Battle.BattleShouldEnd)
                    yield break;
                foreach (var a in ApplyNightmare())
                    yield return a;
            }
        }


        private IEnumerable<BattleAction> OnDLApplied(DreamLevelArgs arg)
        {
            if (Battle.BattleShouldEnd)
                return Enumerable.Empty<BattleAction>();
            return ApplyNightmare();
        }
    }
}
