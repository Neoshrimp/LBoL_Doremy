using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyRecursiveDreamCatcherDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { Hybrid = 1, HybridColor = 0 };
            con.UpgradedCost = new ManaGroup() { Hybrid = 1, HybridColor = 0 };



            con.Block = 6;
            con.UpgradedBlock = 8;

            con.Value1 = 2;
            con.UpgradedValue1 = 1;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyRecursiveDreamCatcherDef))]
    [HarmonyDebug]
    public sealed class DoremyRecursiveDreamCatcher : DCard
    {

        public string Plus => IsUpgraded ? "+" : "";

        public NightmareInfo NM2Apply => new NightmareInfo(Value1, true);

        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(battle.CardUsed, OnSelfUsed);
        }

        private IEnumerable<BattleAction> OnSelfUsed(CardUsingEventArgs args)
        {
            if(args.Card != this)
                yield break;
            yield return new RemoveCardAction(this);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;


            yield return NightmareAction(Battle.Player, NM2Apply, 0f);
            

            if (consumingMana.Total > ManaGroup.Empty.Total)
            {
                var card = Library.CreateCard<DoremyRecursiveDreamCatcher>();
                card.GameRun = GameRun;
                if (IsUpgraded)
                    card.Upgrade();
                yield return new AddCardsToHandAction(card);
                //card.SetBaseCost(Config.Cost);
            }
        }






    }
}
