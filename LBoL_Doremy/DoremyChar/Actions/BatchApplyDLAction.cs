using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL_Doremy.Actions;

using System;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.GraphicsBuffer;

namespace LBoL_Doremy.DoremyChar.Actions
{
    // unused
   /* public class BatchApplyDLAction : EventBattleAction<DreamLevelArgs>
    {

        public readonly IReadOnlyCollection<Card> targets;

        int dreamLevel;
        bool isEndOfTurnBounce;

        public BatchApplyDLAction(IReadOnlyCollection<Card> targets, bool isEndOfTurnBounce, int dreamLevel = 1)
        {
            this.targets = targets;
            Args = new DreamLevelArgs(isEndOfTurnBounce)
            {
                dreamLevelDelta = dreamLevel
            };
            this.dreamLevel = dreamLevel;
            this.isEndOfTurnBounce = isEndOfTurnBounce;
        }


        public override BattleAction SetSource(GameEntity source)
        {
            Args.ActionSource = Args.target;
            return this;
        }


        public override IEnumerable<Phase> GetPhases()
        {
            var toNotify = new List<Card>();
            foreach (var t in targets)
            {
                Args.target = t;
                Args.dreamLevelDelta = dreamLevel;
                Args.isEndOfTurnBounce = isEndOfTurnBounce;

                yield return CreateEventPhase("PreAplying", Args, EventManager.DLEvents.applyingDL);
                yield return CreatePhase("Main", () => {
                    if (Args.target is NaturalDreamLayerCard dlc)
                    {
                        dlc.DreamLevel += Args.dreamLevelDelta;
                        Args.target.NotifyChanged();
                    }
                    Args.target.NotifyActivating();
                    //toNotify.Add(Args.target);
                    Args.CanCancel = false;
                });

                yield return CreateEventPhase("PostApplying", Args, EventManager.DLEvents.appliedDL);

                if (Args.isEndOfTurnBounce)
                    yield return CreatePhase("Bounce", () => {
                        React(new MoveCardToDrawZoneAction(Args.target, DrawZoneTarget.Random), Args.ActionSource, ActionCause.TurnEnd);
                });
            }
            

        }
    }*/
}
