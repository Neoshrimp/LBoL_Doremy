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
using LBoL.EntityLib.StatusEffects.ExtraTurn;

namespace LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates
{
    public sealed class DoremyDreamySakuyaDef : DCardDef
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


            con.Loyalty = 5;
            con.PassiveCost = 2;
            con.ActiveCost = -3;
            con.ActiveCost2 = -6;

            con.Mana = new ManaGroup() { Any = 1 };


            con.RelativeKeyword = Keyword.AutoExile;
            con.UpgradedRelativeKeyword = Keyword.AutoExile;

            con.RelativeCards = new List<string>() { nameof(Knife) + "+" };
            con.UpgradedRelativeCards = new List<string>() { nameof(Knife) + "+" };

            con.RelativeEffects = new List<string>() { nameof(TimeAuraSe) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(TimeAuraSe) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamySakuyaDef))]
    public sealed class DoremyDreamySakuya : DTeammate
    {
        protected override void OnEnterBattle(BattleController battle)
        {
            ReactOnCardsAddedEvents(battle, OnCardsAdded);
        }

        private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
        {
            if(!IsAbilityActive)
                yield break;

            foreach(var c in cards)
                yield return BuffAction<TimeAuraSe>(2, occupationTime: 0.07f);
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

            var knife1 = Library.CreateCard<Knife>(true);
            knife1.IsAutoExile = true;
            yield return new AddCardsToHandAction(knife1);

            var knife2 = Library.CreateCard<Knife>(true);
            knife2.IsAutoExile = true;
            yield return new AddCardsToDrawZoneAction(new Card[] { knife2 }, DrawZoneTarget.Random);

            var knife3 = Library.CreateCard<Knife>(true);
            knife3.IsAutoExile = true;
            yield return new AddCardsToDiscardAction(knife3);

        }


        public override bool DiscardCard => true;


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            
            if (precondition == null || ((MiniSelectCardInteraction)precondition).SelectedCard.FriendToken == FriendToken.Active)
            {
                base.Loyalty += base.ActiveCost;
                var selection = new SelectCardInteraction(0, Battle.HandZone.Count, Battle.HandZone) { Source = this, CanCancel = false };

                yield return new InteractionAction(selection, false);

                if (selection.SelectedCards != null && selection.SelectedCards.Count > 0)
                {
                    yield return new DiscardManyAction(selection.SelectedCards);
                    if (Battle.BattleShouldEnd)
                        yield break;
                    yield return new DrawManyCardAction(selection.SelectedCards.Count);
                }

            }
            else
            {
                base.Loyalty += base.ActiveCost2;
                //UltimateUsed = true;
                yield return BuffAction<ExtraTurn>(1);
                yield return DebuffAction<TimeIsLimited>(Battle.Player,1);

            }
        }


    }


}
