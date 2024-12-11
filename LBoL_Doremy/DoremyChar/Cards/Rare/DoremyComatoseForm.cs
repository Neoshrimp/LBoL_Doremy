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

            con.Value1 = 1;

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
                yield return BuffAction<DoremyComatoseFormUpgradeSE>(Value1);


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
            EventManager.DoremyEvents.DLperLevelMult = DLMult;
            //ReactOnCardsAddedEvents(OnCardsAdded);
            // All created cards have |Dream Layer|.
            ReactOwnerEvent(Battle.CardUsed, OnCardUsed);
            ReactOwnerEvent(Battle.CardPlayed, OnCardUsed);


            SetSleepAnim(unit, true);
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (args.Card != this.SourceCard && args.Card.WasGenerated())
            {
                if (Battle.BattleShouldEnd)
                    yield break;

                for (int i = 0; i < Level; i++)
                {
                    var randomCard = Battle.HandZone.Where(c => DoremyComatoseForm.IsPositive(c)).SampleOrDefault(GameRun.BattleRng);
                    if (randomCard == null)
                        break;
                    yield return new ApplyDLAction(randomCard);
                }
            }
        }

/*        private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
        {
            foreach (var c in cards.Where(c => DoremyComatoseForm.IsPositive(c)))
                c.AddCustomKeyword(DoremyKw.NewDreamLayer);
            yield break;
        }*/


        private static void SetSleepAnim(Unit unit, bool enable)
        {
            if (unit.View is UnitView view && view._modelName == nameof(DoremyCavalier))
            {
                view.DoremySleeping = enable;

                if (enable)
                {
                    view._blinking = false;
                    foreach (AnimationState allState in view.AllStates)
                    {
                        var blinkTrack = allState.Tracks.FirstOrDefault(t => t.Animation.Name == "blink");
                        if (blinkTrack != null)
                            allState.SetEmptyAnimation(blinkTrack.TrackIndex, 0f);
                    }
                }
                else
                    view.Blink();
            }
        }

        protected override void OnRemoved(Unit unit)
        {
            EventManager.DoremyEvents.DLperLevelMult = DoremyEvents.defaultDLMult;
            SetSleepAnim(unit, false);
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
                for(int i = 0; i < Level; i++)
                    yield return new ApplyDLAction(c);
        }
    }

}
