using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core;
using LBoL.EntityLib.Exhibits;
using LBoL_Doremy.DoremyChar.DoremyPU;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL_Doremy.Utils;
using System.Linq;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoLEntitySideloader.Resource;
using LBoL_Doremy.StaticResources;

namespace LBoL_Doremy.DoremyChar.Exhibits
{

    public sealed class DoremyCavalierWExDef : DExhibitDef
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
                Value1: null,
                Value2: null,
                Value3: null,
                Mana: ManaGroup.Empty,
                BaseManaRequirement: null,
                BaseManaColor: ManaColor.White,
                BaseManaAmount: 1,
                HasCounter: false,
                InitialCounter: null,
                Keywords: Keyword.TempMorph,
                RelativeEffects: new List<string>() { nameof(DC_CreatedTooltipSE) },
                RelativeCards: new List<string>());
        }
    }

    [EntityLogic(typeof(DoremyCavalierWExDef))]
    public sealed class DoremyCavalierWEx : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            IsActive = true;

            ReactBattleEvent(Battle.CardsAddedToDiscard, OnOtherAdded);
            ReactBattleEvent(Battle.CardsAddedToDrawZone, OnDrawZoneAdded);
            ReactBattleEvent(Battle.CardsAddedToExile, OnOtherAdded);
            ReactBattleEvent(Battle.CardsAddedToHand, OnOtherAdded);
        }

        protected override void OnLeaveBattle()
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

        private bool _isActive = true;

        public bool IsActive { get => _isActive; private set { _isActive = value; NotifyChanged(); } }

        private IEnumerable<BattleAction> DoDiscount(IEnumerable<Card> cards, GameEventArgs args)
        {
            if (!IsActive || cards.FirstOrDefault() == null)
                yield break;

            if (args.ActionSource.TrickleDownActionSource() is Card)
            {
                NotifyActivating();
                IsActive = false;
                cards.First().SetTurnCost(Mana);
            }
        }

        private IEnumerable<BattleAction> OnDrawZoneAdded(CardsAddingToDrawZoneEventArgs args)
        {
            return DoDiscount(args.Cards, args);
        }

        private IEnumerable<BattleAction> OnOtherAdded(CardsEventArgs args)
        {
            return DoDiscount(args.Cards, args);
        }
    }
}
