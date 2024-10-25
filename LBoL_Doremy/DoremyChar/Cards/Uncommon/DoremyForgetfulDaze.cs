using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyForgetfulDazeDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };

            con.Scry = 5;
            con.UpgradedScry = 7;


            con.Value1 = 3;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyForgetfulDazeDef))]
    public sealed class DoremyForgetfulDaze : DCard
    {
        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(battle.CardMoved, OnCardMoved);
        }

        private IEnumerable<BattleAction> OnCardMoved(CardMovingEventArgs args)
        {
            if (args.ActionSource != this)
                yield break;

            yield return new CastBlockShieldAction(Battle.Player, block: Value1, 0, BlockShieldType.Direct, cast: false);

        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new ScryAction(Scry);
        }
    }
}
