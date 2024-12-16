using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Ults
{


    public sealed class DoremyCavalierUUltDef : DUltimateSkillDef
    {
        public override UltimateSkillConfig MakeConfig()
        {
            return new UltimateSkillConfig(
                "",
                10,
                PowerCost: 100,
                PowerPerLevel: 100,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.OncePerTurn,
                Damage: 0,
                Value1: 15,
                Value2: 0,
                Keywords: Keyword.Accuracy,
                RelativeEffects: new List<string>() { nameof(DC_NightmareSE) } ,
                RelativeCards: new List<string>() { }
                );
        }
    }

    [EntityLogic(typeof(DoremyCavalierUUltDef))]
    public sealed class DoremyCavalierUUlt : UltimateSkill
    {
        public DoremyCavalierUUlt()
        {
            TargetType = TargetType.AllEnemies;
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {


            yield return PerformAction.Spell(Owner, Id);
            yield return PerformAction.Gun(Owner, Battle.AllAliveEnemies.FirstOrDefault(), "Sweet02");

            foreach (var e in selector.GetEnemies(Battle))
                yield return new ApplyStatusEffectAction<DC_NightmareSE>(e, Value1) { Source = this };

            if (Battle.BattleShouldEnd)
                yield break;


            foreach (var e in UnitSelector.AllEnemies.GetEnemies(Battle))
                if (e.TryGetStatusEffect<DC_NightmareSE>(out var nightmare))
                    yield return new DamageAction(Battle.Player, e, DamageInfo.Attack(nightmare.Level / 2f, true));

        }
    }
}
