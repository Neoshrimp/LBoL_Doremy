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
        public virtual bool DoOverrideFriendU => true;

        public override FriendCostInfo FriendU => DoOverrideFriendU ? new FriendCostInfo(base.UltimateCost, FriendCostType.Active): base.FriendU;

        protected bool IsAbilityActive => this.Zone == CardZone.Hand && Summoned;


    }
}