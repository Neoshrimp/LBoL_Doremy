using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL_Doremy.Actions;
using LBoL_Doremy.RootTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class ApplyDLAction : SimpleEventBattleAction<DreamLevelArgs>
    {
        public ApplyDLAction(Card target, int dreamLevel = 1)
        {
            Args = new DreamLevelArgs();
            Args.target = target;
            Args.dreamLevelDelta = dreamLevel;
        }

        public override void PreEventPhase()
        {
            Trigger(EventManager.DLEvents.applyingDL);
        }

        public override void MainPhase()
        {
            if (Args.target is DreamLayerCard dlc)
            {
                dlc.DreamLevel += Args.dreamLevelDelta;
                dlc.NotifyChanged();
            }
            Args.target.NotifyActivating();
        }

        public override void PostEventPhase()
        {
            if (Args.target is DreamLayerCard dlc)
                dlc.OnDLChanged(Args);
            Trigger(EventManager.DLEvents.appliedDL);
        }

    }
}
