﻿using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.EntityLib.Cards.Enemy;
using LBoL.Presentation;
using LBoL_Doremy.DoremyChar.BattleTracking;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyDreamEaterDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.GunName = "Sweet01";

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 3 };
            con.UpgradedCost = new ManaGroup() { Any = 4 };


            con.Damage = 6;
            //con.UpgradedDamage = 11;

            con.Mana = new ManaGroup() { Any = 1 };


            con.UpgradedKeywords = Keyword.Accuracy;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamEaterDef))]
    public sealed class DoremyDreamEater : DCard
    {

        protected override ManaGroup AdditionalCost => Battle == null ? ManaGroup.Empty : Mana * -BattleHistoryHandlers.CardCreationTurnHistory.Total;

        public string NMDmgDesc
        {
            get
            {
                if(Battle == null)
                    return "";

                var toWrap = "";
                if (this.PendingTarget == null)
                    toWrap = "N/A";
                else if (PendingTarget.TryGetStatusEffect<DC_NightmareSE>(out var targetNM))
                        toWrap = NMDmg(targetNM.Level).ToString();
                     else
                        toWrap = "0";

                return StringDecorator.Decorate($"(|e:{toWrap}|) ");
            }
        }

        public int NMDmg(int nightmareLevel) => (nightmareLevel / 2f).RoundToInt(MidpointRounding.AwayFromZero);

        protected override void OnEnterBattle(BattleController battle)
        {
            HandleBattleEvent(battle.Player.DamageDealing, OnSelfDealingDmg, (GameEventPriority)4);//fp priority
        }

        private void OnSelfDealingDmg(DamageDealingEventArgs args)
        {
            if (args.ActionSource != this)
                return;
            

            if (args.Targets?.FirstOrDefault()?.TryGetStatusEffect<DC_NightmareSE>(out var nightmare) ?? false)
            {
                args.DamageInfo = args.DamageInfo.IncreaseBy(NMDmg(nightmare.Level));
                args.AddModifier(this);
            }
        }
    }
}
