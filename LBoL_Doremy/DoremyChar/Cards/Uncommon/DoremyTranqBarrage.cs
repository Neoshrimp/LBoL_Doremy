using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyTranqBarrageDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.RandomEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2, Any = 2 };


            con.Value1 = 10;
            con.UpgradedValue1 = 13;



            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_DLKwSE) };

            return con;
        }
    }



    [EntityLogic(typeof(DoremyTranqBarrageDef))]
    public sealed class DoremyTranqBarrage : DCard
    {
        int DLCount { get; set; }

        public int ApplyTimes => DLCount + 1;

        protected override void OnEnterBattle(BattleController battle)
        {
            //HandleBattleEvent(battle.Player.TurnStarting, args => DLCount = 0);
            HandleBattleEvent(battle.Player.TurnEnding, args => DLCount = 0, (GameEventPriority)(DreamLayerCard.dreamLayerPriority-1));

            HandleBattleEvent(EventManager.DLEvents.appliedDL, args => DLCount++);

        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            for (int i = 0; i < ApplyTimes; i++)
                yield return DebuffAction<DC_NightmareSE>(selector.GetEnemy(Battle), Value1);
        }
    }
}
