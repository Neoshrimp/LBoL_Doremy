using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class NightmareAction : SimpleEventBattleAction<NightmareArgs>
    {

        public NightmareAction(Unit source, Unit target, NightmareInfo level, float occupationTime)
        {
            Args = new NightmareArgs(level.isSelfNightmare) {
                source = source, 
                target = target, 
                level = level,
                occupationTime = occupationTime
            };
        }


        public override void PreEventPhase()
        {
            Trigger(EventManager.NMEvents.nigtmareApplying);
        }


        public override void MainPhase()
        {
            if (Args.target?.IsInvalidTarget ?? true)
            {
                Args.ForceCancelBecause(LBoL.Core.CancelCause.InvalidTarget);
                return;
            }


            var applyNMAction = new ApplyStatusEffectAction<DC_NightmareSE>(Args.target, (int)Args.level, occupationTime: Args.occupationTime);
            if(Args.ActionSource is Card sourceCard)
                applyNMAction.Args.Effect.SourceCard = sourceCard;

            Args.appliedNightmare = (DC_NightmareSE)applyNMAction.Args.Effect;
            Args.appliedNightmare.NightmareSource = Args.source;

            React(applyNMAction, Args.ActionSource, Args.Cause);

            if (applyNMAction.IsCanceled)
                Args.ForceCancelBecause(applyNMAction.CancelCause);

            Args.CanCancel = false;


        }


        public override void PostEventPhase()
        {
            Trigger(EventManager.NMEvents.nigtmareApplied);
        }


    }
}
