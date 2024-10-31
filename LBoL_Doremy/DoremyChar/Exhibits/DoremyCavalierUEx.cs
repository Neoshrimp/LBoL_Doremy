using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.Exhibits;
using LBoL_Doremy.DoremyChar.DoremyPU;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Exhibits
{

    public sealed class DoremyCavalierUExDef : DExhibitDef
    {
        public override ExhibitConfig MakeConfig()
        {
            return new ExhibitConfig(
                Index: 0,
                Id: "",
                Order: 10,
                IsDebug: false,
                IsPooled: false,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.Anywhere,
                Owner: DoremyCavalierDef.Name,
                LosableType: ExhibitLosableType.DebutLosable,
                Rarity: Rarity.Shining,
                Value1: 3,
                Value2: null,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: ManaColor.Blue,
                BaseManaAmount: 1,
                HasCounter: false,
                InitialCounter: null,
                Keywords: Keyword.Shield,
                RelativeEffects: new List<string>(),
                RelativeCards: new List<string>());
        }
    }

    [EntityLogic(typeof(DoremyCavalierUExDef))]
    public sealed class DoremyCavalierUEx : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            ReactBattleEvent(Owner.TurnEnding, OnTurnEnding);

            HandleBattleEvent(Owner.TurnStarted, OnTurnStarted, (GameEventPriority)(-199));
            HandleBattleEvent(Battle.CardUsed, OnCardUsed);
        }

        private void OnTurnStarted(UnitEventArgs args)
        {
            Active = true;
        }

        private void OnCardUsed(CardUsingEventArgs args)
        {
            if (args.Card?.CardType == CardType.Attack)
            { 
                Active = false;
                NotifyChanged();
            }
        }

        protected override void OnLeaveBattle()
        {
            Active = false;
        }

        private IEnumerable<BattleAction> OnTurnEnding(UnitEventArgs args)
        {
            if (Active)
            {
                NotifyActivating();
                yield return new CastBlockShieldAction(Owner, 0, shield: Value1, BlockShieldType.Direct, cast: true);
            }
        }
    }
}
