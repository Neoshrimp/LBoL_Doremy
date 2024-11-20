using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMetaScaling.Debug
{
    public /*sealed*/ class DebugCardDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(DebugCard);
        }

        public override CardImages LoadCardImages()
        {
            return null;
        }

        public override LocalizationOption LoadLocalization()
        {
            return null;
        }

        public override CardConfig MakeConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = LBoL.Base.TargetType.Nobody;
            con.Cost = new LBoL.Base.ManaGroup() { Any = 2 };
            return con;
        }
    }

    //[EntityLogic(typeof(DebugCardDef))]
    public /*sealed*/ class DebugCard : Card
    {
        protected override void OnEnterBattle(BattleController battle)
        {
            //HandleBattleEvent(battle.CardUsed, OnCardUsed);
            ReactBattleEvent(battle.CardUsed, OnCardUsedReactor);
        }

        private IEnumerable<BattleAction> OnCardUsedReactor(CardUsingEventArgs args)
        {
            yield return new GainMoneyAction(10);
            GameRun.GainMaxHpOnly(3);
            yield break;
        }

        private void OnCardUsed(CardUsingEventArgs args)
        {
            GameRun.GainMaxHpOnly(3);
        }

    }

}
