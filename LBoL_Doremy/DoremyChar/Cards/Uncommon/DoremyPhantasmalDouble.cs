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

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 1 };
            


            con.Damage = 15;
            con.UpgradedDamage = 10;

            con.Value1 = 1;
            con.UpgradedValue1 = 2;

            con.UpgradedKeywords = Keyword.Accuracy;


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
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

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

        }
    }
}
