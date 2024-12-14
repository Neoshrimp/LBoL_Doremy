using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LBoL.Core.Units.UltimateSkill;
using System.Xml.Linq;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.DoremyChar.Actions;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{
    public sealed class DoremyAgedDreamDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.AllEnemies;
            con.Rarity = Rarity.Uncommon;

            con.Colors = new List<ManaColor>() { ManaColor.Black };
            con.Cost = new ManaGroup() { Black = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 1 };


            con.Value1 = 3;
            con.UpgradedValue1 = 5;

            con.RelativeEffects = new List<string>() { nameof(Weak), nameof(Vulnerable), nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(Weak), nameof(Vulnerable), nameof(DC_NightmareSE) };
            return con;
        }
    }
    [EntityLogic(typeof(DoremyAgedDreamDef))]
    public sealed class DoremyAgedDream : DCard
    {
        public int PercDesc => GameRun == null ? 50 : 50 + GameRun.EnemyVulnerableExtraPercentage;



        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var e in selector.GetEnemies(Battle))
            {
                if (Battle.BattleShouldEnd)
                    yield break;
                yield return DebuffAction<Weak>(e, duration: Value1, occupationTime: 0f);
                yield return DebuffAction<Vulnerable>(e, duration: Value1, occupationTime: 0f);
            }

            yield return BuffAction<DoremyAgedDreamSE>();

        }
    }



    public sealed class DoremyAgedDreamSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = true;
            con.HasCount = false;
            con.HasLevel = false;


            return con;
        }
    }



    [EntityLogic(typeof(DoremyAgedDreamSEDef))]
    public sealed class DoremyAgedDreamSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            HandleOwnerEvent(EventManager.NMEvents.nigtmareApplying, OnNMApplying, (GameEventPriority)8);
        }

        public int PercDesc => GameRun == null ? 50 : 50 + GameRun.EnemyVulnerableExtraPercentage;

        private void OnNMApplying(NightmareArgs args)
        {
            if (args.target is EnemyUnit enemy
                && !args.level.isSelfNightmare
                && enemy.TryGetStatusEffect<Vulnerable>(out var vuln)
                )
            {
                args.level *= (1f + vuln.Value/100f);
                args.AddModifier(this);
            }
        }
    }
}
