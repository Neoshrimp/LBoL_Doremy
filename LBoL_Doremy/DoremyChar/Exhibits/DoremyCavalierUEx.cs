using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.Exhibits;
using LBoL_Doremy.DoremyChar.DoremyPU;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Exhibits
{

    public sealed class DoremyCavalierUExDef : DExhibitDef
    {
        public override ExhibitSprites LoadSprite()
        {
            var exs = base.LoadSprite();
            exs.customSprites.Add(inactive, ResourceLoader.LoadSprite(inactive + GetId() + ".png", Sources.exAndBomb));
            return exs;
        }

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
                Value1: 4,
                Value2: null,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: ManaColor.Blue,
                BaseManaAmount: 1,
                HasCounter: false,
                InitialCounter: null,
                Keywords: Keyword.Shield,
                RelativeEffects: new List<string>() { nameof(DC_CreatedTooltipSE) },
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
            HandleBattleEvent(Battle.CardPlayed, OnCardUsed);

        }

        private void OnTurnStarted(UnitEventArgs args)
        {
            IsActive = true;
        }

        public override string OverrideIconName
        {
            get
            {
                if (IsActive)
                    return Id;
                return Id + DExhibitDef.inactive;

            }
        }

        private void OnCardUsed(CardUsingEventArgs args)
        {
            if (args.Card?.CardType == CardType.Attack)
            { 
                IsActive = false;
                NotifyChanged();
            }
        }

        private bool _isActive = true;

        public bool IsActive { get => _isActive; private set { _isActive = value; NotifyChanged(); } }

        protected override void OnLeaveBattle()
        {
            IsActive = false;
        }

        private IEnumerable<BattleAction> OnTurnEnding(UnitEventArgs args)
        {
            if (IsActive)
            {
                NotifyActivating();
                yield return new CastBlockShieldAction(Owner, 0, shield: Value1, BlockShieldType.Direct, cast: true);
            }
        }
    }
}
