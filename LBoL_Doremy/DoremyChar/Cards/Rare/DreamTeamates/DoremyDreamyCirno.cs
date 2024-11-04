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
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.Cards.Character.Cirno.Friend;
using HarmonyLib;
using LBoL.Presentation.UI.Panels;

namespace LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates
{
    public sealed class DoremyDreamyCirnoDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.IsPooled = false;
            con.HideMesuem = true;

            con.Type = LBoL.Base.CardType.Friend;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.Blue, ManaColor.Green };
            con.Cost = new ManaGroup() { Any = 2 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };


            con.Loyalty = 3;
            con.PassiveCost = 1;
            con.ActiveCost = -3;
            con.UltimateCost = -7;



            con.RelativeCards = new List<string>() { nameof(SunnyFriend), nameof(LunaFriend), nameof(StarFriend)};
            con.UpgradedRelativeCards = new List<string>() { nameof(SunnyFriend), nameof(LunaFriend), nameof(StarFriend) };

            con.RelativeEffects = new List<string>() { nameof(FrostArmor) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(FrostArmor) };



            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamyCirnoDef))]
    public sealed class DoremyDreamyCirno : DTeammate
    {
        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(battle.CardUsed, OnCardUsed);
        }



        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (IsAbilityActive && args.Card != this && args.Card.WasGenerated())
            {
                if (Battle.BattleShouldEnd)
                    yield break;
                yield return BuffAction<FrostArmor>(2, occupationTime: 0.07f);
            }
        }

        public override IEnumerable<BattleAction> OnTurnEndingInHand()
        {
            return this.GetPassiveActions();
        }

        public int ExtraPassive
        {
            get
            {
                if (RealBattle?.Player.TryGetStatusEffect<FrostArmor>(out var fa) ?? false)
                    return fa.Level / 3;
                return 0;
            }
        }

        public override IEnumerable<BattleAction> GetPassiveActions()
        {
            Loyalty += PassiveCost + ExtraPassive;
            NotifyActivating();
            yield break;
        }


        public int TimesCold => RealBattle?.HandZone.Where(c => c != RealCard && c.WasGenerated()).Count() ?? 0;

        private Card _realCard = null;
        public Card RealCard { get => _realCard == null ? this : _realCard; set => _realCard = value; }

        public override Interaction Precondition()
        {
            if (this.CardType == CardType.Friend && this.Config.ActiveCost != null && this.Config.UltimateCost != null && this.Summoned && this.Loyalty >= -this.UltimateCost)
            {
                var options = this.Clone(2).Cast<DoremyDreamyCirno>().ToList();
                options[0].FriendToken = FriendToken.Active;
                options[0].RealCard = this;
                options[1].FriendToken = FriendToken.Ultimate;
                options[1].RealCard = this;
                return new MiniSelectCardInteraction(options, false, false, false);
            }
            return null;
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            
            if (precondition == null || ((MiniSelectCardInteraction)precondition).SelectedCard.FriendToken == FriendToken.Active)
            {
                base.Loyalty += base.ActiveCost;
                for (int i = 0; i < TimesCold; i++)
                {
                    if (Battle.BattleShouldEnd)
                        yield break;
                    foreach (var a in DebuffAction<Cold>(Battle.AllAliveEnemies))
                        yield return a;
                }

            }
            else
            {
                base.Loyalty += base.UltimateCost;
                var fairies = EnumerateRelativeCards().ToList();
                fairies.Do(f => f.Summon());
                yield return new AddCardsToHandAction(fairies);
            }
        }


        [HarmonyPatch(typeof(Card), nameof(Card.GetDetailInfoCard))]
        class SetRealCardForUI_Patch
        {
            static void Postfix(Card __instance, ref ValueTuple<Card, Card> __result)
            {
                if (__instance is DoremyDreamyCirno dreamyCirno)
                {
                    var card1 = (__result.Item1 as DoremyDreamyCirno);
                    if (card1 != null)
                        card1.RealCard = dreamyCirno;
                    var card2 = (__result.Item2 as DoremyDreamyCirno);
                    if (card2 != null)
                        card2.RealCard = dreamyCirno;
                }
            }
        }



    }


}
