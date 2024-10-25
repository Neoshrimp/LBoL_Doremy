using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.Cards.Character.Alice;
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
    public sealed class DoremyMelatoninMagicDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2 };


            con.Value1 = 3;
            con.UpgradedValue1 = 4;

            con.Mana = new ManaGroup() { Colorless = 1 };


            con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DLKwSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyMelatoninMagicDef))]
    public sealed class DoremyMelatoninMagic : DCard
    {
        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(battle.CardDrawn, OnCardsDrawn);
        }

        private IEnumerable<BattleAction> OnCardsDrawn(CardEventArgs args)
        {
            if(args.ActionSource != this)
                yield break;
            yield return new ApplyDLAction(args.Card);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new GainManaAction(Mana);
            yield return new DrawManyCardAction(Value1);
        }
    }
}
