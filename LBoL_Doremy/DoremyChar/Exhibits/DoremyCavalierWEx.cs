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

namespace LBoL_Doremy.DoremyChar.Exhibits
{

    public sealed class DoremyCavalierWExDef : DExhibitDef
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
                RelativeEffects: new List<string>(),
                RelativeCards: new List<string>());
        }
    }

    [EntityLogic(typeof(DoremyCavalierWExDef))]
    public sealed class DoremyCavalierWEx : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            Active = true;

            ReactBattleEvent(Battle.CardsAddedToDiscard, OnOtherAdded);
            ReactBattleEvent(Battle.CardsAddedToDrawZone, OnDrawZoneAdded);
            ReactBattleEvent(Battle.CardsAddedToExile, OnOtherAdded);
            ReactBattleEvent(Battle.CardsAddedToHand, OnOtherAdded);
        }

        protected override void OnLeaveBattle()
        {
            Active = true;
        }

        private IEnumerable<BattleAction> DoDiscount(IEnumerable<Card> cards, GameEventArgs args)
        {
            if (!Active || cards.FirstOrDefault() == null)
                yield break;

            if (args.ActionSource.TrickleDownActionSource() is Card)
            {
                NotifyActivating();
                Active = false;
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
