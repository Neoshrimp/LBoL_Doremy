using LBoL.Base;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL_Doremy.RootTemplates;
using System.Collections.Generic;

namespace LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates
{
    public abstract class DTeammate : DCard
    {
        protected bool IsAbilityActive => this.Zone == CardZone.Hand && Summoned;


    }
}