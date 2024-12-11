using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Presentation;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.DoremyPU;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.StaticResources;
using LBoL_Doremy.Utils;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            ci.AutoLoad(GetId(), ".png", "", subIds: PreConfig().SubIllustrator.ToList());
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
                ActiveCost2: null,
                UpgradedActiveCost2: null,
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

                ImageId: null,
                UpgradeImageId: null,
                Unfinished: false,
                Illustrator: Artists.DC,
                SubIllustrator: new List<string>());
            return con;

        }

        public abstract CardConfig PreConfig();

    }

    public abstract class DCard  : Card
    {
        public string DL { get => DColorUtils.DL; }

        public string LB { get => DColorUtils.LightBlue; }

        public string UIBlue { get => DColorUtils.UIBlue; }

        public string CC { get => DColorUtils.CC; }
        public string SS => " ";

        public string BrB => StringDecorator.Decorate($"|{(IsUpgraded ? LBoL.Core.Keywords.GetDisplayWord(Keyword.Shield).Name : LBoL.Core.Keywords.GetDisplayWord(Keyword.Block).Name)}|");

        public int BlockOrShield => IsUpgraded ? Shield.Shield : Block.Block;

        public string DoTimes { get => TimesVal > 1 ? LocalizeProperty("Times").RuntimeFormat(FormatWrapper) : ""; }
        public virtual int TimesVal => Value1;


        public override void Initialize()
        {
            base.Initialize();
            FormatWrapper = new DFormatterCard(this, (CardFormatWrapper)FormatWrapper);

        }

        // trick to display concrete values on right-click card zoom in widget while in Battle
        [MaybeNull] protected BattleController RealBattle => this.Battle ?? GameMaster.Instance?.CurrentGameRun?.Battle;


        protected void ReactOnCardsAddedEvents(BattleController battle, Func<Card[], GameEventArgs, IEnumerable<BattleAction>> reactor)
        {
            ReactOnCardsAddedEvents(battle, reactor, (GameEventPriority)Config.Order);
        }


        protected void ReactOnCardsAddedEvents(BattleController battle, Func<Card[], GameEventArgs, IEnumerable<BattleAction>> reactor, GameEventPriority priority)
        {
            EventSequencedReactor<CardsAddingToDrawZoneEventArgs> drawZoneReactor = args => reactor(args.Cards, args);

            EventSequencedReactor<CardsEventArgs> otherReactors = args => reactor(args.Cards, args);

            ReactBattleEvent(battle.CardsAddedToDiscard, otherReactors, priority);
            ReactBattleEvent(battle.CardsAddedToDrawZone, drawZoneReactor, priority);
            ReactBattleEvent(battle.CardsAddedToExile, otherReactors, priority);
            ReactBattleEvent(battle.CardsAddedToHand, otherReactors, priority);
        }

        public NightmareAction NightmareAction(Unit target, NightmareInfo level, float occupationTime = 0.15f) => new NightmareAction(Battle.Player, target, level, occupationTime);

        protected string GetPendingNMDmgDesc(Func<int, int> dmgFunc)
        {
            if (Battle == null)
                return "";

            var toWrap = "";
            if (this.PendingTarget == null)
                toWrap = "N/A";
            else if (PendingTarget.TryGetStatusEffect<DC_NightmareSE>(out var targetNM))
                toWrap = dmgFunc(targetNM.Level).ToString();
            else
                toWrap = "0";

            return StringDecorator.Decorate($"(|e:{toWrap}|) ");
        }


    }
}
