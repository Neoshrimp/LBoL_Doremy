using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            con.Value1 = 1;
            con.UpgradedValue1 = 2;


            con.RelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE) };

            return con;
        }
    }

    [EntityLogic(typeof(DoremyProcrastinateDef))]
    public sealed class DoremyProcrastinate : DCard
    {

        public override Interaction Precondition()
        {
            var hand = Battle.HandZone.Where(c => c != this);
            if (hand.FirstOrDefault() == null)
                return null;
            return new SelectCardInteraction(0, 2, hand) {
                Description = Name + (IsUpgraded ? "+" : "") + LocalizeProperty("UpTo", true).RuntimeFormat(FormatWrapper)
            };
        }


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            IEnumerable<Card> cards = new List<Card>();
            if (precondition is SelectCardInteraction interaction)
            {
                cards = interaction.SelectedCards;
                yield return new ExileManyCardAction(cards);
                if (cards.FirstOrDefault() != null)
                {
                    var buffAction = (ApplyStatusEffectAction)BuffAction<DoremyProcrastinateSE>();
                    yield return buffAction;
                    var status = buffAction.Args.Effect as DoremyProcrastinateSE;
                    if (status != null)
                        status.UpdateQueue(cards);
                }
            }

        }
    }



    public sealed class DoremyProcrastinateSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = false;
            con.HasCount = true;
            con.HasLevel = false;


            return con;
        }
    }



    [EntityLogic(typeof(DoremyProcrastinateSEDef))]
    public sealed class DoremyProcrastinateSE : DC_ExileQeueuSE
    {
        List<Card> _toBounceQueue = new List<Card>();
        public List<Card> ToBounceQueue { get => _toBounceQueue; set => _toBounceQueue = value; }


        public void UpdateQueue(IEnumerable<Card> cards2Add)
        {
            foreach (var c in cards2Add)
                ToBounceQueue.Add(c);
            UpdateCount(ToBounceQueue);
        }
        protected override string GetNoTargetCardInExile() => LocalizeProperty("NotInExile", true);

        public string QueuedCardsDesc => GetQueuedCardsDesc(ToBounceQueue);

        protected override IEnumerable<Card> UpdateQueueContainer(IEnumerable<Card> queue)
        {
            ToBounceQueue = queue.ToList();
            return ToBounceQueue;
        }

        protected override void OnAdded(Unit unit)
        {
            base.OnAdded(unit);
            if (unit is PlayerUnit pu)
            {
                ReactOwnerEvent(pu.TurnStarted, TurnStarted, (GameEventPriority)exileQueuePriority);
            }
        }



        private IEnumerable<BattleAction> TurnStarted(UnitEventArgs args)
        {
            return ProcessQueue(ToBounceQueue);
        }

        protected override IEnumerable<BattleAction> ProcessQueue(IList<Card> queue)
        {
            foreach (var a in base.ProcessQueue(queue))
                yield return a;

            if (ToBounceQueue.Count == 0)
                yield return new RemoveStatusEffectAction(this);
            else
                UpdateCount(queue);
        }

    }


}
