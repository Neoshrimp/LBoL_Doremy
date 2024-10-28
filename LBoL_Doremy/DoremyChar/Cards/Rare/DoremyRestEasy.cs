using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyRestEasyDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Rarity = Rarity.Rare;
            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 3 };
            con.UpgradedCost = new ManaGroup() { Blue = 1 };


            con.Value1 = 1;

            con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DLKwSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyRestEasyDef))]
    public sealed class DoremyRestEasy : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyRestEasySE>(Value1);
        }
    }

    public sealed class DoremyRestEasySEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyRestEasySEDef))]
    public sealed class DoremyRestEasySE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            HandleOwnerEvent(EventManager.DLEvents.appliedDL, OnAppliedDL);
        }

        private void OnAppliedDL(DreamLevelArgs args)
        {
            Card card = args.target;
            if (card.Cost.Amount > 0)
            {
                ManaColor[] array = card.Cost.EnumerateComponents().SampleManyOrAll(base.Level, base.GameRun.BattleRng);
                card.DecreaseBaseCost(ManaGroup.FromComponents(array));
            }
        }
    }
}
