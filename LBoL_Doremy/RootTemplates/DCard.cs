using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.PlayerUnit;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{

    public abstract class DCardDef : CardTemplate
    {
        public override IdContainer GetId() => this.SelfId();

        public override CardImages LoadCardImages()
        {
            var ci = new CardImages(Sources.imgsSource);
            ci.AutoLoad(this, ".png");
            return ci;
        }

        public override LocalizationOption LoadLocalization()
        {
            return Loc.CardsBatchLoc.AddEntity(this);
        }

        public override CardConfig MakeConfig()
        {
            var con = PreConfig();

            return con;
        }

        public new CardConfig DefaultConfig() 
        {
            var con = new CardConfig(
                Index: 0,
                Id: "",
                Order: 10,
                AutoPerform: true,
                Perform: new string[0][],
                GunName: "",
                GunNameBurst: "",
                DebugLevel: 0,
                Revealable: false,

                IsPooled: true,
                FindInBattle: true,

                HideMesuem: false,
                IsUpgradable: true,

                Rarity: Rarity.Common,
                Type: CardType.Unknown,
                TargetType: TargetType.Self,
                Colors: new List<ManaColor>(),

                IsXCost: false,

                Cost: default(ManaGroup),

                UpgradedCost: null,
                MoneyCost: null,
                Damage: null,
                UpgradedDamage: null,
                Block: null,
                UpgradedBlock: null,
                Shield: null,
                UpgradedShield: null,
                Value1: null,
                UpgradedValue1: null,
                Value2: null,
                UpgradedValue2: null,
                Mana: null,
                UpgradedMana: null,
                Scry: null,
                UpgradedScry: null,
                ToolPlayableTimes: null,
                Loyalty: null,
                UpgradedLoyalty: null,
                PassiveCost: null,
                UpgradedPassiveCost: null,
                ActiveCost: null,
                UpgradedActiveCost: null,
                UltimateCost: null,
                UpgradedUltimateCost: null,
                Keywords: Keyword.None,
                UpgradedKeywords: Keyword.None,
                EmptyDescription: false,
                RelativeKeyword: Keyword.None,
                UpgradedRelativeKeyword: Keyword.None,
                RelativeEffects: new List<string>(),
                UpgradedRelativeEffects: new List<string>(),
                RelativeCards: new List<string>(),
                UpgradedRelativeCards: new List<string>(),

                Owner: nameof(DoremyCavalier),

                ImageId: "",
                UpgradeImageId: "",
                Unfinished: false,
                Illustrator: null,
                SubIllustrator: new List<string>());
            return con;

        }

        public abstract CardConfig PreConfig();

    }

    public abstract class DCard : Card
    {
    }
}
