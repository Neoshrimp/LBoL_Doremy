using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.DreamManagers;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoLEntitySideloader.CustomKeywords;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class ApplyDLAction : SimpleEventBattleAction<DreamLevelArgs>
    {
        public ApplyDLAction(Card target, int dreamLevel = 1)
        {
            Args = new DreamLevelArgs
            {
                target = target,
                dreamLevelDelta = dreamLevel
            };
        }

        public ApplyDLAction(Card target, bool isEndOfTurnBounce, int dreamLevel = 1)
        {
            Args = new DreamLevelArgs(isEndOfTurnBounce)
            {
                target = target,
                dreamLevelDelta = dreamLevel
            };
        }

        public override void PreEventPhase()
        {
            Trigger(EventManager.DLEvents.applyingDL);
        }

        public override void MainPhase()
        {
            var target = Args.target;

            if (!target.HasCustomKeyword(DoremyKw.dLId))
                target.AddCustomKeyword(DoremyKw.NewDLKeyword);

            var dl = target.GetCustomKeyword<DLKeyword>(DoremyKw.dLId);

            dl.DreamLevel += Args.dreamLevelDelta;
            
            target.NotifyChanged();
            target.NotifyActivating();
        }

        public override void PostEventPhase()
        {
            if (Args.target is NaturalDreamLayerCard dlc)
                dlc.OnDLChanged(Args);
            Trigger(EventManager.DLEvents.appliedDL);
        }

    }
}
