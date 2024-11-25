using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Cards.Rare;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Units;
using LBoL.Core.Cards;
using System.Linq;
using LBoL.Base.Extensions;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL.Core.Battle.Interactions;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL_Doremy.DoremyChar.DreamManagers;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyUniversalDreamLayerDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { Hybrid = 2, HybridColor = 0 };


            con.Value1 = 1;
            con.UpgradedValue1 = 2;


            con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DLKwSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyUniversalDreamLayerDef))]
    public sealed class DoremyUniversalDreamLayer : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyUniversalDreamLayerSE>(Value1);
        }
    }



    public sealed class DoremyUniversalDreamLayerSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyUniversalDreamLayerSEDef))]
    public sealed class DoremyUniversalDreamLayerSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(DreamLayerHandlers.GetBounceEvent(Battle), OnTurnEnd, (GameEventPriority)(DoremyTranqBarrage.countResetPriorityOffset + DreamLayerHandlers.bouncePriority + 1));
        }

        private IEnumerable<BattleAction> OnTurnEnd(UnitEventArgs args)
        {
            if (Battle.BattleShouldEnd)
                yield break;

            var hand = Battle.HandZone.Where((Card card) => card.CanUpgradeAndPositive);

            SelectCardInteraction selection = null;
            if (hand.FirstOrDefault() != null)
                selection = new SelectCardInteraction(0, Level, hand) {
                    Source = this
                };

            if (selection != null)
            { 
                yield return new InteractionAction(selection, false);
                if (selection.SelectedCards != null && selection.SelectedCards.Count > 0)
                {
                    NotifyActivating();
                    var cards = selection.SelectedCards;
                    yield return new UpgradeCardsAction(cards);
                    foreach (var card in cards)
                    {
                        yield return new ApplyDLAction(card, isEndOfTurnBounce: true);
                        yield return new MoveCardToDrawZoneAction(card, DrawZoneTarget.Random);
                    }
                }
            }

        }

    }
}
