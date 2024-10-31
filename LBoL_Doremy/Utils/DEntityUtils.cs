using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoL_Doremy.Utils
{
    public static class DEntityUtils
    {

        [return: MaybeNull]
        public static GameEntity TrickleDownActionSource(this GameEntity actionSource) 
        {
            var rez = actionSource;
            if (actionSource is StatusEffect se && se.SourceCard != null)
                rez = se.SourceCard;
            return rez;

        }
    }
}
