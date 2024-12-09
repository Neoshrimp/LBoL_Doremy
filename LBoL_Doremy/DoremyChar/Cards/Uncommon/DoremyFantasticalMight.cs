using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{

    public sealed class DoremyFantasticalMightDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 2 };


            con.Value1 = 3;
            con.UpgradedValue1 = 4;


            con.Value2 = 2;
            con.UpgradedValue2 = 3;




            return con;
        }
    }


    [EntityLogic(typeof(DoremyFantasticalMightDef))]
    public sealed class DoremyFantasticalMight : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyFantasticalMightSE>(Value1, count: Value2);
        }
    }

    public sealed class DoremyFantasticalMightSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;

            con.HasCount = true;
            con.CountStackType = StackType.Add;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyFantasticalMightSEDef))]
    public sealed class DoremyFantasticalMightSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            HandleOwnerEvent(unit.DamageDealing, OnDmgDealing, (GameEventPriority)4);
            HandleOwnerEvent(unit.BlockShieldGaining, OnBlockShieldGaining, (GameEventPriority)6);

        }

        private void OnDmgDealing(DamageDealingEventArgs args)
        {
            if (args.ActionSource is Card card && card.WasGenerated() && args.DamageInfo.DamageType == DamageType.Attack)
            {
                args.DamageInfo = args.DamageInfo.IncreaseBy(Level);
                args.AddModifier(this);
            }
        }

        private void OnBlockShieldGaining(BlockShieldEventArgs args)
        {
            if (args.Type == BlockShieldType.Direct)
                return;
            ActionCause cause = args.Cause;
            if (args.ActionSource is Card card && card.WasGenerated())
            {
                if (args.HasBlock)
                {
                    args.Block += (float)Count;
                }
                if (args.HasShield)
                {
                    args.Shield += (float)Count;
                }
                args.AddModifier(this);
            }
        }


    }
}
