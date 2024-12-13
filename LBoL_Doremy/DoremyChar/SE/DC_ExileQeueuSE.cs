using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LBoL_Doremy.DoremyChar.SE
{
    public abstract class DC_ExileQeueuSE : DStatusEffect
    {

        public const int exileQueuePriority = 12;

        public int maxQDisplay = 13;

        protected override void OnAdded(Unit unit)
        {
            Count = 0;
        }

        protected string GetQueuedCardsDesc(IEnumerable<Card> queue)
        {
            if (queue.FirstOrDefault() == null)
                return LocalizeProperty("Nothing");
            var rez = string.Join(", ", queue.Take(maxQDisplay).Select(c => c.ColorName()));

            var count = queue.Count();
            if (count > maxQDisplay)
            {
                rez += string.Format(LocalizeProperty("OverQ", true), count - maxQDisplay);
            }

            return rez;
        }

        protected abstract IEnumerable<Card> UpdateQueueContainer(IEnumerable<Card> queue);

        protected abstract string GetNoTargetCardInExile();

        protected virtual void PostProcessCopy(Card copy) { return; }

        protected virtual IEnumerable<BattleAction> ProcessQueue(IList<Card> queue)
        {
            if (Battle.BattleShouldEnd)
                yield break;

            List<Card> bulkCopies = new List<Card>();

            Dictionary<Card, int> potentialRemove = new Dictionary<Card, int>();
            var actuallyToRemove = new HashSet<int>();

            bool moveBroken = false;
            bool copyBroken = false;

            IEnumerable<BattleAction> ProcessBulkCopies()
            {
                if (bulkCopies.Count > 0 && !copyBroken)
                {
                    var addAction = new AddCardsToHandAction(bulkCopies);
                    NotifyActivating();
                    yield return addAction;
                    foreach (var c in addAction.Args.Cards)
                    {
                        PostProcessCopy(c);
                        actuallyToRemove.Add(potentialRemove[c]);
                    }

                    UpdateCount(queue, actuallyToRemove.Count);

                    if (!addAction.Args.Cards.SequenceEqual(bulkCopies))
                        copyBroken = true;
                    bulkCopies.Clear();
                }

            }

            for (int i = 0; i < queue.Count; i++)
            {

                var card = queue[i];

                if (!card.Config.FindInBattle 
                    || card.Config.ToolPlayableTimes != null 
                    || (card.CardType == LBoL.Base.CardType.Friend && card.Summoned))
                {
                    foreach (var a in ProcessBulkCopies())
                        yield return a;

                    if (!moveBroken)
                    {
                        if (card.Zone == CardZone.Exile)
                        {
                            var moveAction = new MoveCardAction(card, CardZone.Hand);
                            yield return moveAction;

                            if (moveAction.Args.IsCanceled)
                                moveBroken = true;
                            else
                            {
                                PostProcessCopy(moveAction.Args.Card);
                                actuallyToRemove.Add(i);
                            }

                        }
                        else
                        {
                            yield return PerformAction.Chat(Owner, string.Format(GetNoTargetCardInExile(), card.ColorName()), 3f, talk: false);
                            actuallyToRemove.Add(i);
                        }
                        UpdateCount(queue, actuallyToRemove.Count);
                    }
                }
                else
                {
                    if (!copyBroken)
                    {
                        var copy = card.CloneBattleCard();
                        bulkCopies.Add(copy);
                        potentialRemove.Add(copy, i);
                    }

                }

                if (moveBroken && copyBroken)
                    break;
            }


            foreach (var a in ProcessBulkCopies())
                yield return a;

            UpdateQueueContainer(queue.Where((_, i) => !actuallyToRemove.Contains(i)));


        }

        protected void UpdateCount(ICollection<Card> queue, int toBeRemovedCount = 0)
        {
            Count = queue.Count - toBeRemovedCount;
            NotifyChanged();
        }

    }


}
