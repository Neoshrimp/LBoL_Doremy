﻿using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Presentation;
using LBoL_Doremy.DoremyChar.DoremyPU;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;

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
            con.Index = CardIndexGenerator.GetUniqueIndex(con);

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

    public abstract class DCard  : Card
    {
        public string DL { get => DColorUtils.DL; }

        public string LightBlue { get => DColorUtils.LightBlue; }

        public string UIBlue { get => DColorUtils.UIBlue; }

        public string CC { get => DColorUtils.CC; }
        public string SS => " ";

        public string BrB => StringDecorator.Decorate($"|{(IsUpgraded ? LBoL.Core.Keywords.GetDisplayWord(Keyword.Shield).Name : LBoL.Core.Keywords.GetDisplayWord(Keyword.Block).Name)}|");

        public int BlockOrShield => IsUpgraded ? Shield.Shield : Block.Block;
        public string DmgTimes { get => Value1 > 1 ? LocalizeProperty("Times").RuntimeFormat(FormatWrapper) : ""; }

        [MaybeNull]
        protected BattleController RealBattle => this.Battle ?? GameMaster.Instance?.CurrentGameRun?.Battle;



    }
}
