using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.EntityLib.Cards.Enemy;
using LBoL.EntityLib.Cards.Misfortune;
using LBoL.EntityLib.Cards.Neutral.Black;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL.EntityLib.Cards.Tool;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.CustomHandlers;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Ults
{

    // 2do anti save scum
    public sealed class DoremyCavalierWUltDef : DUltimateSkillDef
    {
        public override UltimateSkillConfig MakeConfig()
        {
            return new UltimateSkillConfig(
                "",
                10,
                PowerCost: 110,
                PowerPerLevel: 110,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.OncePerTurn,
                //RepeatableType: UsRepeatableType.FreeToUse,
                Damage: 0,
                Value1: 5,
                Value2: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );
        }
    }

    [EntityLogic(typeof(DoremyCavalierWUltDef))]
    public sealed class DoremyCavalierWUlt : UltimateSkill
    {

        public DoremyCavalierWUlt()
        {
            TargetType = TargetType.Nobody;
        }


        public int MinColours = 1;
        public int MinOrigins => 1;
        public int MinTypes => 2;
        public int MaxTypePool => (int)CardType.Friend;


        public int RollAmount { get; set; }


        public int PoolSize { get; set; }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {

            RollAmount = Value1;
/*            var colorOptions = Enum.GetValues(typeof(ManaColor)).Cast<ManaColor>().Select((c, i) => {
                if (c == ManaColor.Any || c == ManaColor.Hybrid || c == ManaColor.Philosophy)
                    return null;
                var card = Library.CreateCard<DC_ManaOption>();
                card.ManaColor = c;
                return card;
            }).Where(c => c != null);

            var colorSelection = new SelectCardInteraction(2, colorOptions.Count(), colorOptions) { Source = this };
            yield return new InteractionActionPlus(colorSelection, false, new ViewSelectCardResolver(() => {
                var panel = UiManager.GetPanel<SelectCardPanel>();
                panel._selectCardWidgets.Do(w =>
                {
                    if (w.Card is DC_ManaOption mo)
                    {
                        w.CardWidget.cardImage.texture = ResourcesHelper.TryGetCardImage(DC_ManaOption.c2img[(int)mo.ManaColor]);
                    }
                });
            }).ExtendEnumerator);

            var colorFilter = colorSelection.SelectedCards.Cast<DC_ManaOption>().Select(c => c.ManaColor).ToHashSet();*/

            var originIds = Library.GetSelectablePlayers().Select(pu => pu.Id);
            originIds = new string[] { null }.Concat(originIds);
            var originOptions = originIds.Select(id => {
                var card = Library.CreateCard<DC_OriginOption>();
                card.OriginId = id;
                return card;
            });
            var originSelection = new SelectCardInteraction(MinOrigins, originOptions.Count(), originOptions) { Source = this };

            yield return new InteractionActionPlus(originSelection, false, new ViewSelectCardResolver(() => {
                var panel = UiManager.GetPanel<SelectCardPanel>();
                panel.titleTmp.text = LocalizeProperty("ChooseOrigin", true).RuntimeFormat(FormatWrapper);
                panel._selectCardWidgets.Do(w =>
                {
                    if (w.Card is DC_OriginOption oo)
                    {
                        if (oo.OriginId == null)
                            return;
                        var firstCC = CardConfig.AllConfig().FirstOrDefault(cc => cc.Owner == oo.OriginId 
                        && cc.IsPooled
                        && string.IsNullOrEmpty(cc.ImageId));

                        if (firstCC == null)
                            return;
                        var firstImg = ResourcesHelper.TryGetCardImage(firstCC.Id);
                        if(firstImg != null)
                            w.CardWidget.cardImage.texture = firstImg;
                    }
                });
            }).ExtendEnumerator);

            var originFilter = originSelection.SelectedCards.Cast<DC_OriginOption>().Select(c => c.OriginId).ToHashSet();

            RollAmount += Math.Min(15, Enumerable.Range(1, originSelection.SelectedCards.Count-1).Sum());

            var cTypes = Enumerable.Range(1, MaxTypePool).Select(i => (CardType)i).Select(t => {
                var card = Library.CreateCard<DC_TypeOption>();
                card.CType = t;
                return card;
            });

            var cTypeSelection = new SelectCardInteraction(MinTypes, cTypes.Count(), cTypes) { Source = this };

            yield return new InteractionActionPlus(cTypeSelection, false, new ViewSelectCardResolver(() => {
                var panel = UiManager.GetPanel<SelectCardPanel>();
                panel.titleTmp.text = LocalizeProperty("ChooseType", true).RuntimeFormat(FormatWrapper);
                panel._selectCardWidgets.Do(w =>
                {
                    if (w.Card is DC_TypeOption to)
                    {
                        if (to.CType == CardType.Friend)
                        {
                            w.CardWidget.SetProperties();
                            w.CardWidget._changed = false;
                            w.CardWidget.MarginAsFriend = false;
                            w.CardWidget.descriptionText.ForceMeshUpdate(false, false);
                            //w.CardWidget.descriptionText.alignment = TMPro.TextAlignmentOptions.Center;
                        }
                        var imgId = DC_TypeOption.Type2Img[(int)to.CType];
                        var img = ResourcesHelper.TryGetCardImage(imgId);
                        if (img != null)
                            w.CardWidget.cardImage.texture = img;

                        
                    }
                });
            }).ExtendEnumerator);

            var cTypeFilter = cTypeSelection.SelectedCards.Cast<DC_TypeOption>().Select(c => c.CType).ToHashSet();

            RollAmount += cTypeSelection.SelectedCards.Cast<DC_TypeOption>().Select(to => to.RollMod).Sum();

            // rolling
            var weightTable = new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.AllOnes, CardTypeWeightTable.AllOnes);
            //var wt = new CardWeightTable(new RarityWeightTable(common: 0f, uncommon: 1f, rare: 0.5f, mythic: 0f), OwnerWeightTable.AllOnes, CardTypeWeightTable.CanBeLoot);

            Predicate<CardConfig> filter = cc => true
            //&& !cc.Colors.Any(c => !colorFilter.Contains(c)) 
            && (originFilter.Contains(cc.Owner) || (cc.Type == CardType.Tool || cc.Type == CardType.Status || cc.Type == CardType.Misfortune))
            && cTypeFilter.Contains(cc.Type)
            ;

            //Log.LogDebug(string.Join("|", colorFilter.OrderBy(c => (int)c))});

            Log.LogDebug(string.Join("|", originFilter.OrderBy(c => c).Select(o => o == null ? "Neutral" : o)));
            Log.LogDebug(string.Join("|", cTypeFilter.OrderBy(c => c)));

            List<Card> finalCards = new List<Card>();
            var costDebug = Enumerable.Repeat(0, 6).ToArray();

            /*            for (int i = 0; i < 6; i++)
                        {
                            int amount = i;
                            Predicate<CardConfig> filterAndCost = cc => filter(cc) && cc.Cost.Amount == amount;

                            var debugPool = GameRun.CreateValidCardsPool(wt, null, false, false, true, filterAndCost);

                            costDebug[i] = debugPool.Count();

                            var card = Battle.RollCardsWithoutManaLimit(wt, 1, filterAndCost).FirstOrDefault();
                            if (card != null)
                                finalCards.Add(card);
                        }
                        var debugCostGroups = string.Join("|", Enumerable.Range(0, 6).Zip(costDebug, (i1, i2) => $"{i1}:{i2}"));
                        Log.LogDebug($"{debugCostGroups}, total: {costDebug.Sum()}");*/


            /*            var samplingPool = new UniqueRandomPool<Type>();
                        Library.EnumerateCardTypes().Where(t => filter(t.config) 
                        && t.config.Type != CardType.Unknown 
                        && !t.config.Keywords.HasFlag(Keyword.Gadgets))
                            .Do(t => samplingPool.Add(t.cardType, (int)t.config.Rarity>=3 ? 0.5f : 1f));*/

            
            PoolSize = GameRun.CreateValidCardsPool(weightTable, null, false, false, false, filter).Count();

            finalCards = GameRun.RollCardsWithoutManaLimit(GameRun.BattleCardRng, weightTable, RollAmount, false, false, filter).ToList();

            //finalCards = Battle.RollCardsWithoutManaLimit(weightTable, RollAmount, filter).ToList();
            /*finalCards = samplingPool.SampleMany(GameRun.BattleCardRng, RollAmount, false).Select(t => Library.CreateCard(t)).ToList();
            finalCards.Do(c => c.GameRun = GameRun);*/

            if (finalCards.Count < RollAmount)
            {
                yield return PerformAction.Chat(Owner, string.Format(LocalizeProperty("CancelChat"), finalCards.Count), 3f, talk: true);
            }
            Log.LogDebug($"total pool {PoolSize}");

            var cardSelection = new SelectCardInteraction(1, 1, finalCards) { CanCancel = false, Source = this };
            if (finalCards.Count > 0)
            { 
                yield return new InteractionActionPlus(cardSelection, false, new ViewSelectCardResolver(() => {
                    var panel = UiManager.GetPanel<SelectCardPanel>();
                    panel.titleTmp.text += LocalizeProperty("RollAmount", true).RuntimeFormat(FormatWrapper);
                }).ExtendEnumerator);
            }

            var cardChosen = cardSelection.SelectedCards?.FirstOrDefault();

            if (cardChosen != null)
            {
                yield return PerformAction.Spell(Owner, Id);
                cardChosen.SetBaseCost(new ManaGroup() { Any = cardChosen.BaseCost.Amount });
                yield return new AddCardsToHandAction(cardChosen);
                cardChosen = null;
            }

            PoolSize = 0;
            RollAmount = 0;
        }
    }

    public sealed class DC_ManaOptionDef : OptionCardDef
    {
        public override CardImages LoadCardImages()
        {
            var ci = new CardImages(Sources.imgsSource, (UnityEngine.Texture2D)ResourcesHelper.TryGetCardImage(nameof(CManaCard)));
            return ci;
        }

        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            con.Owner = "";
            return con;
        }
    }


    [EntityLogic(typeof(DC_ManaOptionDef))]
    public sealed class DC_ManaOption : Card
    {



        public static string[] c2img = new string[] { "", nameof(WManaCard), nameof(UManaCard), nameof(BManaCard), nameof(RManaCard), nameof(GManaCard), nameof(CManaCard), nameof(PManaCard), "" };



        public override void Initialize()
        {
            base.Initialize();
            Config = Config.Copy();
        }



        private ManaColor _manaColor;
        public ManaColor ManaColor
        {
            get => _manaColor; 
            set
            {
                _manaColor = value;

                if(_manaColor == ManaColor.Philosophy)
                    Config.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue, ManaColor.Black, ManaColor.Red, ManaColor.Green };
                else
                    Config.Colors = new List<ManaColor>() { value };
            }
        }

        public ManaGroup C2Mana
        {
            get
            {
                var mg = new ManaGroup();
                mg.SetValue(ManaColor, 1);
                return mg;
            }
        }
    }



    public sealed class DC_OriginOptionDef : OptionCardDef
    {
        public override CardImages LoadCardImages()
        {
            var ci = new CardImages(Sources.imgsSource, (UnityEngine.Texture2D)ResourcesHelper.TryGetCardImage(nameof(SummerFlower)));
            return ci;
        }

        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            con.Owner = "";
            return con;
        }
    }
    [EntityLogic(typeof(DC_OriginOptionDef))]
    public sealed class DC_OriginOption : Card
    {

        public override void Initialize()
        {
            base.Initialize();
            Config = Config.Copy();
        }

        public string OriginName => OriginId == null ? LocalizeProperty("Neutral") : UnitNameTable.GetName(OriginId, PlayerUnitConfig.FromId(OriginId).NarrativeColor).ToString(true);

        private string _originId = null;
        public string OriginId
        {
            get => _originId; 
            set
            {
                _originId = value;
                Config.Owner = value;
            }
        }
    }


    public sealed class DC_TypeOptionDef : OptionCardDef
    {
        public override CardImages LoadCardImages()
        {
            return null;
        }

        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            con.Owner = "";
            return con;
        }
    }
    [EntityLogic(typeof(DC_TypeOptionDef))]
    public sealed class DC_TypeOption : Card
    {

        public override void Initialize()
        {
            base.Initialize();
            Config = Config.Copy();
        }


        public static string[] Type2Img = new string[] { "", nameof(FinalSpark), nameof(NiuqiDefense), nameof(PerfectServant), nameof(BailianBlack),  nameof(MeilingFriend), nameof(ToolHeal), nameof(Nightmare), nameof(Drunk)};


        public static int[] Type2RollMod = new int[] { 0, 2, 1, 0, -1, -1, -3, 2, 2,};


        public string RollModDesc => GameEntityFormatWrapper.WrappedFormatNumber(0, RollMod, "");

        public int RollMod { get; set; }

        public string CTypeDesc
        {
            get
            {
                var rez = StringDecorator.Decorate($"|{$"CardType.{CType}".Localize(true)}|");
                return rez;
            }
        }

        CardType _cType;

        public CardType CType
        {
            get => _cType;
            set
            {
                Config.Type = value;
                RollMod = Type2RollMod[(int)value];
                _cType = value;
            }
        }
    }
}
