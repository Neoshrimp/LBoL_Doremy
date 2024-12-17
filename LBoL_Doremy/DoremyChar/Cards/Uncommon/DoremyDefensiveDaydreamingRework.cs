using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyDefensiveDaydreamingReworkDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2, Any = 1 };
            con.UpgradedCost = new ManaGroup() { White = 1, Any = 2 };


            con.Value1 = 4;
            con.UpgradedValue1 = 5;



            return con;
        }
    }


    [EntityLogic(typeof(DoremyDefensiveDaydreamingReworkDef))]
    public sealed class DoremyDefensiveDaydreamingRework : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyDefensiveDaydreamingReworkSE>(Value1);
        }
    }


    public sealed class DoremyDefensiveDaydreamingReworkSEDEf : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;
            return con;
        }
    }

    [EntityLogic(typeof(DoremyDefensiveDaydreamingReworkSEDEf))]
    public sealed class DoremyDefensiveDaydreamingReworkSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Battle.CardExiled, OnExiled);
            HandleOwnerEvent(Battle.CardUsing, OnCardUsing, GameEventPriority.Highest);
            HandleOwnerEvent(Battle.CardUsed, OnCardUsed, GameEventPriority.Lowest);

        }


        Card lastCardUsing = null;

        private void OnCardUsing(CardUsingEventArgs args)
        {
            lastCardUsing = args.Card;
        }
        private void OnCardUsed(CardUsingEventArgs args)
        {
            lastCardUsing = null;
        }

        private IEnumerable<BattleAction> OnExiled(CardEventArgs args)
        {
            var card = args.Card;
            if(card.WasGenerated() && lastCardUsing != card)
                yield return new CastBlockShieldAction(Owner, 0, shield: Level, BlockShieldType.Direct, cast: false);
        }

    }


}
