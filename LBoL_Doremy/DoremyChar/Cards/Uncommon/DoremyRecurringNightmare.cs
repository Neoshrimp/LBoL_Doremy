﻿using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.DoremyChar.BattleTracking;
using LBoL_Doremy.CreatedCardTracking;
using System.Threading.Tasks.Sources;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public /*sealed*/ class DoremyRecurringNightmareDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 0 };

            con.Value1 = 6;
            con.UpgradedValue1 = 9;

            con.Value2 = 1;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    //[EntityLogic(typeof(DoremyRecurringNightmareDef))]
    public /*sealed*/ class DoremyRecurringNightmare : DCard
    {
        public NightmareInfo NM2Apply => Value1;

        public NightmareInfo NM2ApplySelf => Value2;


        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(Battle.CardUsed, OnCardUsed);
            ReactBattleEvent(Battle.CardPlayed, OnCardUsed);

        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            var card = args.Card;
            if (this.Zone == LBoL.Core.Cards.CardZone.Discard 
                && card.WasGenerated() 
                && card.GetType() != typeof(DoremyRecurringNightmare))
                yield return new MoveCardAction(this, LBoL.Core.Cards.CardZone.Hand);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return NightmareAction(selector.SelectedEnemy, NM2Apply);
            yield return NightmareAction(Battle.Player, NM2ApplySelf);
        }
    }
}
