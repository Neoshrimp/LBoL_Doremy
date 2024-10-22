using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyProcrastinateDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;
            con.Rarity = Rarity.Common;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 2 };

            con.Block = 18;
            con.UpgradedBlock = 22;
            con.Value1 = 3;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyProcrastinateDef))]
    public sealed class DoremyProcrastinate : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            yield return BuffAction<DoremyProcrastinateSE>(level: Value1);
        }
    }



    public sealed class DoremyProcrastinateSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;
            con.CountStackType = StackType.Max;
            con.IsStackable = false;
            con.HasCount = false;
            con.HasLevel = true;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyProcrastinateSEDef))]
    public sealed class DoremyProcrastinateSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            if (unit is PlayerUnit pu)
            {
                ReactOwnerEvent(pu.TurnStarted, TurnStarted);
            }


        }

        private IEnumerable<BattleAction> TurnStarted(UnitEventArgs args)
        {
            if (Battle.BattleShouldEnd)
                yield break;

            NotifyActivating();
            var cards = Battle.RollCards(new CardWeightTable(RarityWeightTable.NoneRare, OwnerWeightTable.Valid, CardTypeWeightTable.CanBeLoot), Level, cc => cc.Type == CardType.Attack);

            if (cards.Length != 0)
            {
                MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(cards)
                {
                    Source = this
                };
                yield return new InteractionAction(interaction, false);
                Card selectedCard = interaction.SelectedCard;
                selectedCard.IsEthereal = true;
                yield return new AddCardsToHandAction(new Card[] { selectedCard });
            }
            yield return new RemoveStatusEffectAction(this);
        }
    }
}
