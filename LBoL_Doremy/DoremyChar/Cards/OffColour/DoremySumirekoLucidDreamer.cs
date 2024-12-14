using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.Core.Battle.Interactions;
using System.Linq;
using LBoL.EntityLib.StatusEffects.ExtraTurn.Partners;
using LBoL.EntityLib.StatusEffects.ExtraTurn;
using HarmonyLib;
using LBoL.Core.Units;
using UnityEngine;
using LBoL.Presentation;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoL_Doremy.DoremyChar.Actions;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{
    public sealed class DoremySumirekoLucidDreamerDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Friend;
            con.TargetType = TargetType.Nobody;
            con.Rarity = Rarity.Uncommon;

            con.Colors = new List<ManaColor>() { ManaColor.Black };
            con.Cost = new ManaGroup() { Black = 1, Blue = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 2, Hybrid = 1, HybridColor = 4};

            con.Loyalty = 3;
            con.PassiveCost = 1;
            con.UpgradedPassiveCost = 2;
            con.ActiveCost = -4;
            con.UltimateCost = -7;


            con.Value1 = 2;
            con.UpgradedValue1 = 3;

            con.Value2 = 1;

            con.RelativeKeyword = Keyword.Scry;
            con.UpgradedRelativeKeyword = Keyword.Scry;


            con.RelativeCards = new string[] { nameof(PManaCard) + '+' };
            con.UpgradedRelativeCards = new string[] { nameof(PManaCard) + '+' };


            return con;
        }
    }
    [EntityLogic(typeof(DoremySumirekoLucidDreamerDef))]
    public sealed class DoremySumirekoLucidDreamer : DTeammate
    {

        public ScryInfo ScryInfo
        {
            get
            {
                return new ScryInfo(Value1);
            }
        }


        public Card Ritual => EnumerateRelativeCards().First();

        public string UpgradeDesc => IsUpgraded ? "+" : "";

        public string RitualName => Ritual.Name + (Ritual.IsUpgraded ? "+" : "");


        public override IEnumerable<BattleAction> OnTurnEndingInHand()
        {
            return GetPassiveActions();
        }


        public override IEnumerable<BattleAction> GetPassiveActions()
        {
            if (!Summoned || Battle.BattleShouldEnd)
                yield break;

            Loyalty += PassiveCost;
            NotifyActivating();
            for (int i = 0; i < base.Battle.FriendPassiveTimes; i++)
            {
                yield return BuffAction<DoremySumirekoScrySE>(Value1);
            }
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {

            if (precondition == null || ((MiniSelectCardInteraction)precondition).SelectedCard.FriendToken == FriendToken.Active)
            {
                base.Loyalty += base.ActiveCost;
                var cards = new Card[] { Library.CreateCard<DoremySumirekoLucidDreamer>(IsUpgraded), Ritual };
                yield return new AddCardsToHandAction(cards);
            }
            else
            {
                base.Loyalty += base.UltimateCost;
                UltimateUsed = true;
                yield return BuffAction<ExtraTurn>(1);
                yield return BuffAction<HuiyeTimeSe>(limit: Value2);
                yield return BuffAction<TurnStartDontLoseBlock>(1);
                yield return BuffAction<DoremySumirekoDontLoseGrazeNextTurnSE>(1);


                yield return new RequestEndPlayerTurnAction();
            }
        }
    }

    public sealed class DoremySumirekoScrySEDef : DStatusEffectDef
    {
        public override Sprite LoadSprite()
        {
            return ResourcesHelper.TryGetSprite<StatusEffect>(nameof(WindGirl));
        }

        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = true;
            con.HasLevel = true;


            return con;
        }
    }


    [EntityLogic(typeof(DoremySumirekoScrySEDef))]
    public sealed class DoremySumirekoScrySE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(unit.TurnStarting, OnTurnStarting);
        }

        public ScryInfo ScryInfo => new ScryInfo(Level);

        private IEnumerable<BattleAction> OnTurnStarting(UnitEventArgs args)
        {
            yield return new ScryAction(ScryInfo);
            yield return new RemoveStatusEffectAction(this);
        }
    }

    public sealed class DoremySumirekoDontLoseGrazeNextTurnSEDEf : DStatusEffectDef
    {
        public override Sprite LoadSprite()
        {
            return ResourcesHelper.TryGetSprite<StatusEffect>(nameof(WindGirl));
        }

        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = true;
            con.HasCount = false;
            con.HasLevel = false;

            con.HasDuration = true;
            con.DurationStackType = StackType.Keep;
            con.DurationDecreaseTiming = DurationDecreaseTiming.Custom;

            return con;
        }
    }

    [EntityLogic(typeof(DoremySumirekoDontLoseGrazeNextTurnSEDEf))]
    public sealed class DoremySumirekoDontLoseGrazeNextTurnSE : DStatusEffect
    {

        protected override void OnAdded(Unit unit)
        {
            Duration = 1;
            ReactOwnerEvent(unit.TurnStarted, OnTurnStarted, GameEventPriority.Lowest);
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            Duration--;
            NotifyChanged();
            if (Duration <= 0)
                yield return new RemoveStatusEffectAction(this, occupationTime: 0f);
        }

        [HarmonyPatch(typeof(Graze), nameof(Graze.OnOwnerTurnStarted))]
        class Graze_Patch
        {
            static bool Prefix(Graze __instance)
            {
                if (__instance.Owner.TryGetStatusEffect<DoremySumirekoDontLoseGrazeNextTurnSE>(out var se))
                {
                    se.NotifyActivating();
                    return false;
                }
                return true;
            }
        }


    }
}
