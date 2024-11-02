using HarmonyLib;
using LBoL.Base;
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
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
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
    public sealed class DoremyCavalierWUltDef : DUltimateSkillDef
    {
        public override UltimateSkillConfig MakeConfig()
        {
            return new UltimateSkillConfig(
                "",
                10,
                PowerCost: 120,
                PowerPerLevel: 120,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.OncePerTurn,
                //RepeatableType: UsRepeatableType.FreeToUse,
                Damage: 0,
                Value1: 3,
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
        class GenFilters
        {
            HashSet<ManaColor> manaColors = new HashSet<ManaColor>();
            HashSet<string> origins = new HashSet<string>();
        }

        public DoremyCavalierWUlt()
        {
            TargetType = TargetType.Nobody;
        }




        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {

            var colorOptions = Enum.GetValues(typeof(ManaColor)).Cast<ManaColor>().Select((c, i) => {
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

            var colorFilter = colorSelection.SelectedCards.Cast<DC_ManaOption>().Select(c => c.ManaColor).ToHashSet();

            var originIds = Library.GetSelectablePlayers().Select(pu => pu.Id);
            originIds = new string[] { null }.Concat(originIds);
            var originOptions = originIds.Select(id => {
                var card = Library.CreateCard<DC_OriginOption>();
                card.OriginId = id;
                return card;
            });
            var originSelection = new SelectCardInteraction(Value1, originOptions.Count(), originOptions) { Source = this };

            yield return new InteractionActionPlus(originSelection, false, new ViewSelectCardResolver(() => {
                var panel = UiManager.GetPanel<SelectCardPanel>();
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

            // rolling
            var wt = new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.AllOnes, CardTypeWeightTable.CanBeLoot);

            Predicate<CardConfig> filter = cc => !cc.Colors.Any(c => !colorFilter.Contains(c)) && originFilter.Contains(cc.Owner);

            var pool = GameRun.CreateValidCardsPool(wt, null, false, false, true, filter);
            Log.LogDebug($"{string.Join("|", colorFilter.OrderBy(c => (int)c))}, {string.Join("|", originFilter.OrderBy(c => c).Select(o => o == null ? "Neutral" : o))}, pool size: {pool.Count()}");

            var cards = Battle.RollCardsWithoutManaLimit(wt, 5, filter);
            if (cards.Length < 5)
            {
                yield return PerformAction.Chat(Owner, string.Format(LocalizeProperty("CancelChat"), cards.Length), 3f, talk: true);
            }
            // debug
            var cardSelection = new SelectCardInteraction(1, 1, cards) { CanCancel = false, Source = this };
            if (cards.Length > 0)
            { 
                yield return new InteractionAction(cardSelection);
            }

            var cardChosen = cardSelection.SelectedCards?.FirstOrDefault();

            if (cardChosen != null)
            {
                yield return PerformAction.Spell(Owner, Id);
                cardChosen.SetBaseCost(new ManaGroup() { Any = cardChosen.BaseCost.Amount });
                yield return new AddCardsToHandAction(cardChosen);
                cardChosen = null;
            }
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
}
