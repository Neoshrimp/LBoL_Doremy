using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LBoL.Core.StatusEffects;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyPhantasmalDoubleDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.GunName = "Sweet01";

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2,};
            con.UpgradedCost = new ManaGroup() { White = 1, Any = 1 };




            con.Damage = 15;
            con.UpgradedDamage = 20;

            con.Value1 = 1;
            con.UpgradedValue1 = 1;

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Expel;


            con.RelativeKeyword = Keyword.CopyHint | Keyword.Ability;
            con.UpgradedRelativeKeyword = Keyword.CopyHint | Keyword.Ability;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyPhantasmalDoubleDef))]
    public sealed class DoremyPhantasmalDouble : DCard
    {
        protected override void SetGuns()
        {
            CardGuns = new Guns(GunName, Value1);
        }

        public string UpgradeDesc => IsUpgraded ? LocalizeProperty("UpgradeTxt", true, true).RuntimeFormat(FormatWrapper) : "";

        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent<DieEventArgs>(Battle.EnemyDied, Expel);
        }

        private IEnumerable<BattleAction> Expel(DieEventArgs args)
        {
            if (args.DieSource == this && !args.Unit.HasStatusEffect<Servant>())
            {
                var copy = this.CloneBattleCard();
                yield return new AddCardsToHandAction(copy);
            }
        }

        public override Interaction Precondition()
        {
            List<Card> list = base.Battle.HandZone.Where((Card hand) => hand != this && hand.CanBeDuplicated).ToList<Card>();
            if (list.Count <= 0)
            {
                return null;
            }
            return new SelectHandInteraction(1, 1, list);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            

            if (precondition != null)
            {
                Card origin = ((SelectHandInteraction)precondition).SelectedCards[0];

                Card card = origin.CloneBattleCard();
                yield return new AddCardsToHandAction(card);
                if (origin.CardType == CardType.Ability || origin.IsExile)
                {
                    origin.IsCopy = true;
                }
            }

            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;


        }


    }
}
