using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.ExtraTurn;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyCatnapDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2 };


            con.Keywords = Keyword.Exile;

            con.RelativeEffects = new List<string>() { nameof(TurnStartDontLoseBlock) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(TurnStartDontLoseBlock) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyCatnapDef))]
    public sealed class DoremyCatnap : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<ExtraTurn>(1);
            yield return BuffAction<TurnStartDontLoseBlock>(1);
            yield return BuffAction<DoremyCatnapSE>();

            yield return new RequestEndPlayerTurnAction();
        }
    }


    public sealed class DoremyCatnapSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Special;

            con.HasLevel = false;
            con.IsStackable = false;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyCatnapSEDef))]
    public sealed class DoremyCatnapSE : ExtraTurnPartner
    {


        protected override void OnAdded(Unit unit)
        {
            ThisTurnActivating = false;
            HandleOwnerEvent(Battle.Player.TurnStarting, delegate (UnitEventArgs _)
            {
                if (Battle.Player.IsExtraTurn && !Battle.Player.IsSuperExtraTurn && Battle.Player.GetStatusEffectExtend<ExtraTurnPartner>() == this)
                {
                    ThisTurnActivating = true;
                    ShowCount = true;
                }
            });
            ReactOwnerEvent(Battle.Player.TurnEnded, OnPlayerTurnEnded);
        }

        public override bool ShouldPreventCardUsage(Card card)
        {
            return true;
        }

        public override string PreventCardUsageMessage => LocalizeProperty("NapMsg");

        private IEnumerable<BattleAction> OnPlayerTurnEnded(UnitEventArgs args)
        {
            if (ThisTurnActivating)
            {
                yield return new RemoveStatusEffectAction(this, true);
            }
        }
    }
}
