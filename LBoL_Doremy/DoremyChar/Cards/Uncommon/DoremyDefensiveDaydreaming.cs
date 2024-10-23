using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyDefensiveDaydreamingDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2, Any = 1 };

            con.Value1 = 3;
            con.UpgradedValue1 = 4;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDefensiveDaydreamingDef))]
    public sealed class DoremyDefensiveDaydreaming : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyDefensiveDaydreamingSE>(Value1);
        }
    }


    public sealed class DoremyDefensiveDaydreamingSEDEf : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;
            return con;
        }
    }

    [EntityLogic(typeof(DoremyDefensiveDaydreamingSEDEf))]
    public sealed class DoremyDefensiveDaydreamingSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOnCardsAddedEvents(unit, OnCardsAdded);
        }

        private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
        {
            foreach(var c in cards)
            {
                NotifyActivating();
                yield return new CastBlockShieldAction(Owner, 0, shield: Level, BlockShieldType.Direct, cast: false);
            }
        }
    }


}
