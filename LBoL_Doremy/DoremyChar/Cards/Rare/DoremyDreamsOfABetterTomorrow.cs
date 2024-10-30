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
using LBoL.EntityLib.Cards;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL.EntityLib.EnemyUnits.Normal.Yinyangyus;
using LBoL_Doremy.DoremyChar.Cards.Common;
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.Utils;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyDreamsOfABetterTomorrowDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.FindInBattle = false;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 4 };

            con.Value1 = 1;
            con.UpgradedValue1 = 2;

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;



            con.RelativeCards = new List<string>() { nameof(DOBToptionMaxHp), nameof(DOBToptionGold), nameof(DOBToptionP), nameof(DOBToptionTool) };
            con.UpgradedRelativeCards = new List<string>() { nameof(DOBToptionMaxHp), nameof(DOBToptionGold), nameof(DOBToptionP), nameof(DOBToptionTool) };



            return con;
        }
       
    }
    

    [EntityLogic(typeof(DoremyDreamsOfABetterTomorrowDef))]
    public sealed class DoremyDreamsOfABetterTomorrow : DCard
    {

        public override string Description =>  CanActivate ? base.Description : StringDecorator.Decorate("|d:" + base.Description + "|") ;


        bool _canActivate = true;
        public bool CanActivate
        {
            get
            {
                if (Battle == null)
                    return true;
                if(_canActivate)
                    _canActivate = !Battle.BattleCardUsageHistory.Any(c => c.Id == Id);

                return _canActivate;
            }
            internal set => _canActivate = value;
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {


            if (!CanActivate)
                yield break;

            var options = EnumerateRelativeCards().ToList();
            var i = GameRun.BattleCardRng.NextInt(0, options.Count - 1);

            options.RemoveAt(i);

            var selection = new SelectCardInteraction(1, Value1, options) { CanCancel = false };
            yield return new InteractionAction(selection);

            if (selection.SelectedCards != null)
            {
                var tool = selection.SelectedCards.FirstOrDefault(c => c is DOBToptionTool) as DOBToptionTool;
                if(tool != null)
                    foreach (var a in tool.EffectActions())
                        yield return a;

                var cards = selection.SelectedCards.Where(c => !(c is DOBToptionTool));
                if (cards.FirstOrDefault() != null)
                {
                    yield return BuffAction<DoremyDreamsOfABetterTomorrowSE>();
                    var status = Battle.Player.StatusEffects.FirstOrDefault(se => se.SourceCard == this) as DoremyDreamsOfABetterTomorrowSE;
                    if (status != null)
                        status.UpdateCards(cards);
                }
                
                // update desc
                Battle.EnumerateAllCards().Where(c => c.Id == Id)
                    .Cast<DoremyDreamsOfABetterTomorrow>()
                    .Do(c => { c.CanActivate = false; c.NotifyChanged(); });
            }
            
        }

        public override IEnumerable<BattleAction> AfterUseAction()
        {
            return base.AfterUseAction();
        }
    }


    public sealed class DoremyDreamsOfABetterTomorrowSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Special;

            con.IsStackable = false;
            con.HasCount = true;
            con.HasLevel = false;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamsOfABetterTomorrowSEDef))]
    public sealed class DoremyDreamsOfABetterTomorrowSE : DStatusEffect
    {


        List<Card> _endOfCombatCards = new List<Card>();
        public IReadOnlyList<Card> EndOfBattleCards { get => _endOfCombatCards.AsReadOnly(); }

        public void UpdateCards(IEnumerable<Card> cards)
        {
            _endOfCombatCards.AddRange(cards.Where(c => !c.IsCopy));
            Count = EndOfBattleCards.Count;
            NotifyChanged();
        }


        public string QueuedEffects
        {
            get
            {
                return string.Join(", ", EndOfBattleCards.Cast<IOptionCard>().Select(op => LightBlue + op.BriefDesc + CC));
            }
        }

        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Battle.BattleEnding, OnBattleEnding);
        }

        private IEnumerable<BattleAction> OnBattleEnding(GameEventArgs args)
        {
            foreach (var op in EndOfBattleCards.Cast<IOptionCard>())
                foreach (var a in op.EffectActions())
                    yield return a;
        }
    }



    // -----------------OPTIONS---------------------------

    public sealed class DOBToptionMaxHpDef : OptionCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            con.Value1 = 3;
            return con;
        }
    }
    [EntityLogic(typeof(DOBToptionMaxHpDef))]
    public sealed class DOBToptionMaxHp : DCard, IOptionCard
    {
        public string BriefDesc => LocalizeProperty("Brief", true, true).RuntimeFormat(FormatWrapper).FirstToLower();

        public IEnumerable<BattleAction> EffectActions()
        {
            GameRun.GainMaxHp(Value1, true, true);
            yield break;
        }
    }


    public sealed class DOBToptionGoldDef : OptionCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            con.Value1 = 25;
            return con;
        }
    }
    [EntityLogic(typeof(DOBToptionGoldDef))]
    public sealed class DOBToptionGold : DCard, IOptionCard
    {
        public string BriefDesc => LocalizeProperty("Brief", true, true).RuntimeFormat(FormatWrapper).FirstToLower();

        public IEnumerable<BattleAction> EffectActions()
        {
            yield return new GainMoneyAction(Value1, SpecialSourceType.None);
        }
    }


    public sealed class DOBToptionPDef : OptionCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            con.Value1 = 25;
            return con;
        }
    }
    [EntityLogic(typeof(DOBToptionPDef))]
    public sealed class DOBToptionP : DCard, IOptionCard
    {

        public string BriefDesc => LocalizeProperty("Brief", true, true).RuntimeFormat(FormatWrapper).FirstToLower();

        public IEnumerable<BattleAction> EffectActions()
        {
            yield return new GainPowerAction(Value1);
        }
    }


    public sealed class DOBToptionToolDef : OptionCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            return con;
        }
    }
    [EntityLogic(typeof(DOBToptionToolDef))]
    public sealed class DOBToptionTool : DCard, IOptionCard
    {
        public string BriefDesc => "";

        public IEnumerable<BattleAction> EffectActions()
        {
            var card = RealBattle.RollCard(new CardWeightTable(RarityWeightTable.OnlyCommon, OwnerWeightTable.Valid, CardTypeWeightTable.OnlyTool));

            if (card != null)
            { 
                card.DeckCounter = 1;
                yield return new AddCardsToDeckAction(card);
            }
        }
    }


}
