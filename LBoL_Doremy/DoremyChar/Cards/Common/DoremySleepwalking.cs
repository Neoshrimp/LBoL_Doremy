using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremySleepwalkingDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Common;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 1 };


            con.Value1 = 2;
            con.Mana = ManaGroup.Empty;

            con.Keywords = Keyword.Exile;

            con.RelativeKeyword = Keyword.TempMorph;
            con.UpgradedRelativeKeyword = Keyword.TempMorph;

            con.RelativeEffects = new List<string>() { nameof(Weak) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(Weak) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremySleepwalkingDef))]
    public sealed class DoremySleepwalking : DCard
    {


        public override Interaction Precondition()
        {
            var hand = Battle.HandZone.Where(c => c != this && c.WasGenerated());
            if(hand.FirstOrDefault() == null)
                return null;
            return new SelectHandInteraction(1, 1, hand);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return DebuffAction<Weak>(selector.SelectedEnemy, duration: Value1);

            if (precondition is SelectHandInteraction handInteraction)
            {
                var card = handInteraction.SelectedCards.FirstOrDefault();
                if (card != null) 
                {
                    card.SetTurnCost(Mana);
                }
            }
        }
    }
}
