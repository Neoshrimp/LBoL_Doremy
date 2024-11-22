using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.StatusEffects.Reimu;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using LBoL.Core.Units;
using LBoL_Doremy.CreatedCardTracking;
using System.Linq;

namespace LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates
{
    public sealed class DoremyDreamyReimuDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.IsPooled = false;
            con.HideMesuem = true;

            con.Type = LBoL.Base.CardType.Friend;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Red };
            con.Cost = new ManaGroup() { Any = 2 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };

            con.Value1 = 1;
            con.Value2 = 9;

            con.Loyalty = 3;
            con.PassiveCost = 2;
            con.ActiveCost = -1;
            con.ActiveCost2 = -4;




            con.RelativeCards = new List<string>() { nameof(YinyangCard) };
            con.UpgradedRelativeCards = new List<string>() { nameof(YinyangCard) };

            con.RelativeEffects = new List<string>() { nameof(ReimuFreeAttackSe) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(ReimuFreeAttackSe) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamyReimuDef))]
    public sealed class DoremyDreamyReimu : DTeammate
    {
        protected override void OnEnterBattle(BattleController battle)
        {
            //ReactBattleEvent(battle.CardUsed, OnCardUsed);
            ReactOnCardsAddedEvents(battle, OnCardsAdded);
        }

        private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
        {
            if (!IsAbilityActive)
                yield break;
            foreach (var c in cards.Where(c => c != this))
            {
                if (Battle.BattleShouldEnd)
                    yield break;
                yield return BuffAction<TempFirepower>(1, occupationTime: 0.07f);
                yield return BuffAction<TempSpirit>(1, occupationTime: 0.07f);
            }
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (IsAbilityActive && args.Card != this && args.Card.WasGenerated())
            {
                if (Battle.BattleShouldEnd)
                    yield break;
                yield return BuffAction<TempFirepower>(1, occupationTime: 0.07f);
                yield return BuffAction<TempSpirit>(1, occupationTime: 0.07f);
            }

        }

        public override IEnumerable<BattleAction> OnTurnEndingInHand()
        {
            return this.GetPassiveActions();
        }

        public override IEnumerable<BattleAction> GetPassiveActions()
        {
            if (Battle.BattleShouldEnd)
                yield break;

            Loyalty += PassiveCost;
            NotifyActivating();
            var orb = Library.CreateCard<YinyangCard>();
            orb.GameRun = GameRun;
            //orb.IsEthereal = true;
            yield return new AddCardsToHandAction(orb);
            yield return BuffAction<ReimuFreeAttackSe>(1);
        }

        public int ToDraw => RealBattle == null ? 0 : RealBattle.TurnExileHistory.Count;


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            
            if (precondition == null || ((MiniSelectCardInteraction)precondition).SelectedCard.FriendToken == FriendToken.Active)
            {
                base.Loyalty += base.ActiveCost;
                yield return new DrawManyCardAction(ToDraw);
            }
            else
            {
                base.Loyalty += base.ActiveCost2;
                yield return BuffAction<DoremyDreamyReimuSE>(count: Value2);
            }
        }


    }

    public sealed class DoremyDreamyReimuSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;

            con.HasLevel = false;
            con.HasCount = true;
            con.CountStackType = StackType.Add;
            con.HasDuration = true;
            con.DurationStackType = StackType.Keep;
            con.DurationDecreaseTiming = DurationDecreaseTiming.Custom;

            return con;
        }
    }

    [EntityLogic(typeof(DoremyDreamyReimuSEDef))]
    public sealed class DoremyDreamyReimuSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            Duration = 1;
            ReactOwnerEvent(unit.BlockShieldGained, OnBlockShieldGained);
            ReactOwnerEvent(unit.TurnEnded, OnTurnEnded, (GameEventPriority)199);

        }

        private IEnumerable<BattleAction> OnTurnEnded(UnitEventArgs args)
        {
            if(--Duration <= 0)
                yield return new RemoveStatusEffectAction(this);
        }

        private IEnumerable<BattleAction> OnBlockShieldGained(BlockShieldEventArgs args)
        {
            foreach (var e in UnitSelector.AllEnemies.GetEnemies(Battle))
            {
                if (Battle.BattleShouldEnd)
                    yield break;

                yield return new DamageAction(Owner, e, DamageInfo.Reaction(Count));
            }
        }
    }
}
