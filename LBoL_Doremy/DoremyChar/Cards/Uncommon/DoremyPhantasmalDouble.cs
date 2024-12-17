using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LBoL.Core.StatusEffects;
using HarmonyLib;
using LBoL_Doremy.DoremyChar.Cards.Rare;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyPhantasmalDoubleDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.GunName = "Sweet01";

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2,};
            con.UpgradedCost = new ManaGroup() { White = 1, Any = 1 };




            con.Damage = 15;
            con.UpgradedDamage = 10;

            con.Value1 = 1;
            con.UpgradedValue1 = 2;

            //con.Keywords = Keyword.Exile;
            //con.UpgradedKeywords = Keyword.Exile;


            con.RelativeKeyword = Keyword.CopyHint | Keyword.Ability;
            con.UpgradedRelativeKeyword = Keyword.CopyHint | Keyword.Ability;


            return con;
        }
    }


    [EntityLogic(typeof(DoremyPhantasmalDoubleDef))]
    public sealed class DoremyPhantasmalDouble : DCard
    {
        protected override void SetGuns()
        {
            CardGuns = new Guns(GunName, Value1);
        }

        //public string UpgradeDesc => IsUpgraded ? LocalizeProperty("UpgradeTxt", true, true).RuntimeFormat(FormatWrapper) : "";


        public string JustName => Name;

        public string CopyDesc
        {
            get
            {
                var rez = LocalizeProperty("CopyTxt", true, true).RuntimeFormat(FormatWrapper);
                if(!CanCopy)
                    rez = "|d:" + rez + "|";
                rez = StringDecorator.Decorate(rez);
                return rez;
            }
        }

        bool _canCopy = true;
        public bool CanCopy
        {
            get => _canCopy;
            set => _canCopy = value;
        }

        public int MaxUseCount => (GameRun?.BaseDeck.Count(c => c.Id == this.Id) ?? 0) + 1;

        public int RemainingUseCount { get => !CanCopy ? 0 : Math.Max(0, MaxUseCount - useCount); }

        int useCount = 0;
        
        private bool CheckCanCopy()
        {
            if (RealBattle == null)
                return true;
            useCount = RealBattle.BattleCardUsageHistory.Count(c => c.Id == this.Id) + RealBattle.BattleCardPlayHistory.Count(c => c.Id == this.Id);

            return MaxUseCount > useCount;
        }


        void UpdateCanCopy()
        {
            if (CanCopy)
            {
                var actuallyCanCopy = CheckCanCopy();

                if (CanCopy != actuallyCanCopy)
                {
                    CanCopy = actuallyCanCopy;
                    if (!CanCopy)
                        Battle.EnumerateAllCards().Where(c => c.Id == Id)
                            .Cast<DoremyPhantasmalDouble>()
                            .Do(c => { c.CanCopy = false; c.NotifyChanged(); });
                }
            }
        }

        protected override void OnEnterBattle(BattleController battle)
        {
            CanCopy = CheckCanCopy();
            HandleBattleEvent(Battle.CardUsed, OnCardUsed);
            HandleBattleEvent(Battle.CardPlayed, OnCardUsed);

        }

        private void OnCardUsed(CardUsingEventArgs args)
        {
            if (args.Card == this)
                UpdateCanCopy();
        }

        protected override void OnLeaveBattle()
        {
            useCount = 0;
        }



        public override Interaction Precondition()
        {
            UpdateCanCopy();
            if(!CanCopy)
                return null;

            List<Card> list = base.Battle.HandZone.Where((Card hand) => hand != this && hand.CanBeDuplicated).ToList<Card>();
            if (list.Count <= 0)
            {
                return null;
            }
            return new SelectHandInteraction(1, 1, list);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {

            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            if (precondition != null)
            {
                Card origin = ((SelectHandInteraction)precondition).SelectedCards[0];

                Card card = origin.CloneBattleCard();
                yield return new AddCardsToHandAction(card);
                if (origin.CardType == CardType.Ability || origin.IsExile)
                {
                    origin.IsCopy = true;
                }
            }
        }



    }
}
