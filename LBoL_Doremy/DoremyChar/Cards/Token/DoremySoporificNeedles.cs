using Cysharp.Threading.Tasks;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Token
{

    public sealed class DoremySoporificNeedlesDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.IsPooled = false;
            con.HideMesuem = true;
            con.Rarity = Rarity.Common;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1 };


            con.Damage = 5;
            con.UpgradedDamage = 8;

            con.Keywords = Keyword.Exile | Keyword.Retain;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Retain;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremySoporificNeedlesDef))]

    public sealed class DoremySoporificNeedles : DCard
    {
        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent(battle.Player.DamageDealt, OnPlayerDmgDealt, (GameEventPriority)99);
        }

        private IEnumerable<BattleAction> OnPlayerDmgDealt(DamageEventArgs args)
        {
            if(args.ActionSource != this)
                yield break;

            var dmgLevel = (int)args.DamageInfo.Amount;

            if(dmgLevel > 0)
                yield return DebuffAction<DC_NightmareSE>(args.Target, dmgLevel);
        }
    }
}
