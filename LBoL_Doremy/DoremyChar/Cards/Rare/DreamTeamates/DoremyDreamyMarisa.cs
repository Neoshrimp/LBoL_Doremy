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
using LBoL_Doremy.DoremyChar.Actions;

namespace LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates
{
    public sealed class DoremyDreamyMarisaDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.IsPooled = false;
            con.HideMesuem = true;

            con.Type = LBoL.Base.CardType.Friend;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.Black, ManaColor.Red };
            con.Cost = new ManaGroup() { Any = 2 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };


            con.Loyalty = 6;
            con.PassiveCost = 2;
            con.ActiveCost = -5;
            con.ActiveCost2 = -6;




            con.RelativeCards = new List<string>() { nameof(Potion), nameof(Astrology) };
            con.UpgradedRelativeCards = new List<string>() { nameof(Potion), nameof(Astrology) };

            con.RelativeEffects = new List<string>() { nameof(Charging), nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(Charging), nameof(DC_NightmareSE) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamyMarisaDef))]
    public sealed class DoremyDreamyMarisa : DTeammate
    {
        public const int nightmareMultPriority = 20;
        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(battle.CardUsed, OnCardUsed);
            ReactBattleEvent(battle.CardPlayed, OnCardUsed);


            HandleBattleEvent(EventManager.NMEvents.nigtmareApplying, OnNMApplying);

        }

        private void OnNMApplying(NightmareArgs args)
        {
            if (IsAbilityActive
                && args.source is PlayerUnit
                && args.source.HasStatusEffect<Burst>()
                && !args._modifiers.Any(wr => {
                    if (wr.TryGetTarget(out var modifier))
                        return modifier is DoremyDreamyMarisa;
                    return false;
                }))
            {
                args.level *= 2f;
                args.AddModifier(this);
            }
        }


        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (IsAbilityActive && args.Card != this && args.Card.WasGenerated())
            {
                if (Battle.BattleShouldEnd)
                    yield break;
                yield return BuffAction<Charging>(1, occupationTime: 0.07f);
            }
        }

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
            var pot = Library.CreateCard<Potion>();
            pot.GameRun = GameRun;

            var ast = Library.CreateCard<Astrology>();
            ast.GameRun = GameRun;

            yield return new AddCardsToDrawZoneAction(new Card[] { pot }, DrawZoneTarget.Random);
            yield return new AddCardsToHandAction(ast);
        }

        public override IEnumerable<BattleAction> SummonActions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.SummonActions(selector, consumingMana, precondition))
                yield return a;

            foreach (var c in Battle.EnumerateAllCards())
                c.NotifyChanged();
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            
            if (precondition == null || ((MiniSelectCardInteraction)precondition).SelectedCard.FriendToken == FriendToken.Active)
            {
                base.Loyalty += base.ActiveCost;
                yield return new AddCardsToDrawZoneAction(Library.CreateCards<Potion>(2), DrawZoneTarget.Random);
                yield return new DrawManyCardAction(3);
            }
            else
            {
                base.Loyalty += base.ActiveCost2;
                yield return BuffAction<Burst>(1);
                yield return new DrawManyCardAction(3);
            }
        }


    }


}
