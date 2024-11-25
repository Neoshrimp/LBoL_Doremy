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
using System.Runtime.InteropServices;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL_Doremy.DoremyChar.SE;
using System.Linq;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.EntityLib.StatusEffects.Sakuya;
using LBoLEntitySideloader;
using UnityEngine;
using LBoL.Presentation;
using LBoL.EntityLib.StatusEffects.Enemy;
using HarmonyLib;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL.Core.Randoms;

namespace LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates
{
    public sealed class DoremyDreamyDoremyDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.IsPooled = false;
            con.HideMesuem = true;

            con.Type = LBoL.Base.CardType.Friend;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { Any = 2 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };

            con.Value1 = 13;
            con.Value2 = 9;

            con.Loyalty = 4;
            con.PassiveCost = 2;
            con.ActiveCost = -3;
            con.ActiveCost2 = -5;


            con.RelativeEffects = new List<string>() { nameof(DoremySleepShieldSE), nameof(DC_DreamLayerKeywordSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DoremySleepShieldSE), nameof(DC_DreamLayerKeywordSE) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamyDoremyDef))]
    public sealed class DoremyDreamyDoremy : DTeammate
    {

        public override IEnumerable<BattleAction> OnTurnEndingInHand()
        {
            return this.GetPassiveActions();
        }

        public override IEnumerable<BattleAction> GetPassiveActions()
        {
            if (!Summoned || Battle.BattleShouldEnd)
                yield break;

            Loyalty += PassiveCost;
            NotifyActivating();

            foreach (var c in Battle.HandZone.Where(c => c.HasCustomKeyword(DoremyKw.dreamLayerId)))
                 yield return new ApplyDLAction(c);
        }

        public override IEnumerable<BattleAction> SummonActions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach(var a in base.SummonActions(selector, consumingMana, precondition))
                yield return a;
            yield return BuffAction<DoremySleepShieldSE>(Value1);

        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            
            if (precondition == null || ((MiniSelectCardInteraction)precondition).SelectedCard.FriendToken == FriendToken.Active)
            {
                base.Loyalty += base.ActiveCost;
                yield return BuffAction<DoremySleepShieldSE>(Value2);

            }
            else
            {
                base.Loyalty += base.ActiveCost2;

                var pool = GameRun.CreateValidCardsPool(weightTable: new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.AllOnes, CardTypeWeightTable.OnlyFriend), manaLimit: null, colorLimit: false, applyFactors: false, battleRolling: true, filter: null);

                CardConfig.FromId(nameof(DoremyDreamTeam)).RelativeCards.Where(id => id != nameof(DoremyDreamTeamNextPage)).Select(id => TypeFactory<Card>.TryGetType(id)).Where(t => t != null)
                    .Do(t => pool.Add(t, RarityWeightTable.BattleCard.Rare));

                var cards = pool.SampleMany(GameRun.BattleCardRng, 2).Select(t => Library.CreateCard(t)).ToList();
                cards.Do(c => c.GameRun = GameRun);
                //var cards = Battle.RollCardsWithoutManaLimit(new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.AllOnes, CardTypeWeightTable.OnlyFriend), 2);
                cards.Do(c => c.Summon());

                yield return new AddCardsToHandAction(cards);
            }
        }

        public override IEnumerable<BattleAction> AfterFollowPlayAction() => AfterUse(base.AfterFollowPlayAction());
        public override IEnumerable<BattleAction> AfterUseAction() => AfterUse(base.AfterUseAction());

        public IEnumerable<BattleAction> AfterUse(IEnumerable<BattleAction> baseActions)
        {
            foreach (var a in baseActions)
            {
                if (a is MoveCardAction moveAction
                    && moveAction.Args.SourceZone == CardZone.PlayArea
                    && moveAction.Args.DestinationZone == CardZone.Discard)
                    yield return new MoveCardToDrawZoneAction(this, DrawZoneTarget.Random);
                else
                    yield return a;
            }
        }

    }



    public sealed class DoremySleepShieldSEDef : DStatusEffectDef
    {
        public override Sprite LoadSprite()
        {
            return ResourcesHelper.TryGetSprite<StatusEffect>(nameof(Sleep));
        }

        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;

            con.HasLevel = true;
            con.CountStackType = StackType.Add;


            return con;
        }
    }

    [EntityLogic(typeof(DoremySleepShieldSEDef))]
    public sealed class DoremySleepShieldSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(unit.DamageReceived, OnDmgReceived);
            ReactOwnerEvent(unit.TurnEnded, OnTurnEnded, (GameEventPriority)199);

        }

        private IEnumerable<BattleAction> OnDmgReceived(DamageEventArgs args)
        {
            if (args.DamageInfo.Damage > 0f)
            {
                yield return new RemoveStatusEffectAction(this);
            }
        }

        private IEnumerable<BattleAction> OnTurnEnded(UnitEventArgs args)
        {
            if (Owner.Shield < Level)
            {
                NotifyActivating();
                yield return new CastBlockShieldAction(Owner, Owner, new ShieldInfo(Level - Owner.Shield, BlockShieldType.Direct), false);
            }
        }


    }

}
