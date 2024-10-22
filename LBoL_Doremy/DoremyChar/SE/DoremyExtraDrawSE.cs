using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.RootTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Units;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;

namespace LBoL_Doremy.DoremyChar.SE
{
    public sealed class DoremyExtraDrawSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyExtraDrawSEDef))]
    public sealed class DoremyExtraDrawSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Battle.Player.TurnStarting, OnTurnStarting, (GameEventPriority)20);
        }

        private IEnumerable<BattleAction> OnTurnStarting(UnitEventArgs args)
        {
            NotifyActivating();
            yield return new DrawManyCardAction(Level);
            yield return new RemoveStatusEffectAction(this);
        }
    }
}
