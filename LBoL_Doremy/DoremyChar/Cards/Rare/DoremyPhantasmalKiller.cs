using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyPhantasmalKillerDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.GunName = "Sweet01";

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2, Any = 2 };

            con.Damage = 10;
            con.UpgradedDamage = 6;

            con.Value1 = 1;
            con.UpgradedValue1 = 2;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }



    [EntityLogic(typeof(DoremyPhantasmalKillerDef))]
    public sealed class DoremyPhantasmalKiller : DCard
    {
        protected override void SetGuns()
        {
            CardGuns = new Guns(GunName, Value1);
        }

        EnemyUnit targetUnit = null;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;
            targetUnit = selector.SelectedEnemy;
            tryApplyNightmare = true;
        }

        public override IEnumerable<BattleAction> AfterFollowPlayAction() => AfterUse(base.AfterFollowPlayAction());
        public override IEnumerable<BattleAction> AfterUseAction() => AfterUse(base.AfterUseAction());

        public IEnumerable<BattleAction> AfterUse(IEnumerable<BattleAction> baseActions)
        {
            tryApplyNightmare = false;
            var e = targetUnit;
            if (e != null && e.IsAlive)
//            foreach (var e in UnitSelector.AllEnemies.GetEnemies(Battle))
                if (e.TryGetStatusEffect<DC_NightmareSE>(out var nightmare))
                    yield return new DamageAction(Battle.Player, e, DamageInfo.HpLose(nightmare.Level));

            targetUnit = null;
            foreach (var a in baseActions)
                yield return a; 
        }

        bool tryApplyNightmare = false;

        protected override void OnEnterBattle(BattleController battle)
        {
            tryApplyNightmare = false;
            targetUnit = null;
            ReactBattleEvent(battle.Player.StatisticalTotalDamageDealt, OnStatisticalTotalDamageDealt);
        }

        private IEnumerable<BattleAction> OnStatisticalTotalDamageDealt(StatisticalDamageEventArgs mainArgs)
        {

            if (Battle.BattleShouldEnd)
                yield break;
            if (mainArgs.ActionSource != this)
                yield break;
            if (!tryApplyNightmare)
                yield break;

            tryApplyNightmare = false;

            var NMperUnit = new Dictionary<Unit, int>();

            foreach ((var u, var dmgArgsList) in mainArgs.ArgsTable)
            {
                if (u == Battle.Player)
                    continue;

                foreach (var args in dmgArgsList)
                {
                    if (args.ActionSource == this && args.DamageInfo.DamageType == DamageType.Attack)
                    { 
                        NMperUnit.TryAdd(args.Target, 0);
                        NMperUnit[args.Target] += args.DamageInfo.Damage.ToInt();
                    }
                }
            }

            foreach (var kv in NMperUnit)
                if (kv.Value > 0)
                    yield return NightmareAction(kv.Key, kv.Value);

        }

    }
}
