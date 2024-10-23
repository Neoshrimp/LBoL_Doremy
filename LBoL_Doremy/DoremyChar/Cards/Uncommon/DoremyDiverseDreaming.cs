using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyDiverseDreamingDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2, Any = 1 };


            con.Value1 = 1;

            con.RelativeCards = new List<string>() { nameof(WManaCard) };
            con.UpgradedRelativeCards = new List<string>() { nameof(PManaCard)};


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDiverseDreamingDef))]
    public sealed class DoremyDiverseDreaming : DCard
    {
        public string Ritual => EnumerateRelativeCards().First().Name;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var attack = Battle.RollCard(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), cc => cc.Type == CardType.Attack);

            var defense = Battle.RollCard(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), cc => cc.Type == CardType.Defense);

            var skill = Battle.RollCard(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), cc => cc.Type == CardType.Skill);

            var cards = new Card[] { attack, defense, skill };
            cards.Do(c => { c.IsExile = true; c.IsEthereal = true; });

            yield return new AddCardsToHandAction(cards);

            yield return new AddCardsToHandAction(EnumerateRelativeCards());


        }

    }
}
