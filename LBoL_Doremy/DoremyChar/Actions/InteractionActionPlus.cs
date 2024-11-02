using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI;
using HarmonyLib;
using LBoL_Doremy.DoremyChar.Ults;
using LBoL.Presentation;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public sealed class InteractionActionPlus : BattleAction
    {
        public readonly Interaction _interaction;
        private readonly Func<IEnumerator, IEnumerator> resolver;

        public override bool IsModified => false;

        public override string[] Modifiers => Array.Empty<string>();

        public override bool IsCanceled => false;

        public override CancelCause CancelCause => CancelCause.None;

        public override string Name => base.Name + " (" + _interaction.GetType().Name + ")";

        public InteractionActionPlus(Interaction interaction, bool canCancel, Func<IEnumerator, IEnumerator> resolver)
        {
            _interaction = interaction;
            this.resolver = resolver;
            _interaction.CanCancel = canCancel;
        }

        public override IEnumerable<Phase> GetPhases()
        {
            yield return CreatePhase("Resolve", resolver(Battle.GameRun.InteractionViewer.View(_interaction)));
        }



        public override void ClearModifiers()
        {
        }

        public override string ExportDebugDetails()
        {
            return string.Empty;
        }
    }
}
