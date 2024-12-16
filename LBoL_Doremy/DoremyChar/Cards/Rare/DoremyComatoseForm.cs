using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL.Core.Units;
using LBoL.Presentation.Units;
using LBoL_Doremy.DoremyChar.DoremyPU;
using Spine;
using System.Linq;
using UnityEngine;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Resource;
using LBoL.Base.Extensions;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.Utils;
using LBoL.Core.StatusEffects;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyComatoseFormDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { White = 2, Blue = 2, Hybrid = 1, HybridColor = 0 };

            con.Value1 = 3;


            //con.Keywords = Keyword.Ethereal;

            con.RelativeKeyword = Keyword.CopyHint;
            con.UpgradedRelativeKeyword = Keyword.CopyHint;

            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE), nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE), nameof(DC_DLKwSE) };

            con.Illustrator = Artists.kimmchu;

            return con;
        }
    }


    [EntityLogic(typeof(DoremyComatoseFormDef))]
    public sealed class DoremyComatoseForm : DCard
    {

        public string DLMultDesc => (DoremyComatoseFormSE.DLMult * 100).ToString();

        public string UpgradeDesc => IsUpgraded ? LocalizeProperty("UpgradeTxt", true, true).RuntimeFormat(FormatWrapper) : "";

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyComatoseFormSE>(Value1);
            if(IsUpgraded)
                yield return BuffAction<DoremyComatoseFormUpgradeSE>(1);


        }

        public static bool IsPositive(Card card) => card.CardType != CardType.Misfortune && card.CardType != CardType.Status;
    }


    public sealed class DoremyComatoseFormSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;
            con.HasLevel = true;

            con.HasCount = true;
            con.CountStackType = StackType.Keep;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyComatoseFormSEDef))]
    public sealed class DoremyComatoseFormSE : DStatusEffect
    {

        public string DLMultDesc => (DoremyComatoseFormSE.DLMult * 100).ToString();


        public const float DLMult = 0.15f;
        protected override void OnAdded(Unit unit)
        {
            Count += Level;

            //EventManager.DoremyEvents.DLperLevelMult = DLMult;
            //Card output boost is increased to | e:{ DLMultDesc}|% per { DL}stack.

            //ReactOnCardsAddedEvents(OnCardsAdded);
            // All created cards have |Dream Layer|.
            ReactOwnerEvent(Battle.CardUsed, OnCardUsed);
            ReactOwnerEvent(Battle.CardPlayed, OnCardUsed);

            // priority not super important
            HandleOwnerEvent(unit.TurnStarting, args => Count = Level, (GameEventPriority)(-30));


            if(unit is DoremyCavalier doremy)
                doremy.SetSleepAnim(true);
        }

        public override bool Stack(StatusEffect other)
        {
            var rez = base.Stack(other);
            if (rez)
                Count += other.Level;

            return rez;
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (args.Card != this.SourceCard && args.Card.WasGenerated() && Count > 0)
            {
                if (Battle.BattleShouldEnd)
                    yield break;


                var randomCard = Battle.HandZone.Where(c => DoremyComatoseForm.IsPositive(c)).SampleOrDefault(GameRun.BattleRng);
                if (randomCard != null)
                {
                    yield return new ApplyDLAction(randomCard);
                    Count--;
                }


                /*for (int i = 0; i < Level; i++)
                {

                }*/
            }
        }

/*        private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
        {
            foreach (var c in cards.Where(c => DoremyComatoseForm.IsPositive(c)))
                c.AddCustomKeyword(DoremyKw.NewDreamLayer);
            yield break;
        }*/


     

        protected override void OnRemoved(Unit unit)
        {
            EventManager.DoremyEvents.DLperLevelMult = DoremyEvents.defaultDLMult;
            if (unit is DoremyCavalier doremy)
                doremy.SetSleepAnim(false);
        }
     
    }


    public sealed class DoremyComatoseFormUpgradeSEDef : DStatusEffectDef
    {
        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite(nameof(DoremyComatoseFormSE) + ".png", Sources.imgsSource);
        }

        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyComatoseFormUpgradeSEDef))]
    public sealed class DoremyComatoseFormUpgradeSE : DStatusEffect
    {

        protected override void OnAdded(Unit unit)
        {
            ReactOnCardsAddedEvents(OnCardsAdded);
        }

        private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
        {
            foreach (var c in cards.Where(c => DoremyComatoseForm.IsPositive(c)))
                //for(int i = 0; i < Level; i++)
                    yield return new ApplyDLAction(c, Level);
        }
    }

}
