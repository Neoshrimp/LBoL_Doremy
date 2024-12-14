using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LBoL_Doremy.DoremyChar.Keywords;
using System.Net.Http.Headers;
using LBoL.Base.Extensions;
using System.Collections;
using UnityEngine;
using System.Linq;
using LBoL.Core.Intentions;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{
    public sealed class DoremyNightmareFormDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Nobody;
            con.Rarity = Rarity.Rare;

            con.Colors = new List<ManaColor>() { ManaColor.Black };
            con.Cost = new ManaGroup() { Black = 1, Any = 1 };


            con.UpgradedKeywords = Keyword.Initial;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };
            return con;
        }
    }
    [EntityLogic(typeof(DoremyNightmareFormDef))]
    public sealed class DoremyNightmareForm : DCard
    {


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyNightmareFormSE>();
        }
    }



    public sealed class DoremyNightmareFormSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = true;
            con.HasCount = true;
            con.HasLevel = false;
            con.CountStackType = StackType.Keep;


            return con;
        }
    }



    [EntityLogic(typeof(DoremyNightmareFormSEDef))]
    public sealed class DoremyNightmareFormSE : DStatusEffect
    {
        public const float refreshSleepInterval = 0.33f;
        private const float nmMult = 2f;

        IEnumerator UpdateDmgEstimate()
        {
            while (true)
            {
                DmgEstimate();
                yield return new WaitForSecondsRealtime(refreshSleepInterval);
            }
        }

        private void DmgEstimate()
        {
            if (!Owner.IsInTurn)
                return;
            int totalDmg = 0;
            foreach (var e in Battle.AllAliveEnemies)
            {
                foreach (var i in e.Intentions.Where(i => !i.HiddenInBattle))
                {
                    if (i is AttackIntention ai)
                        totalDmg += ai.TotalDamage;
                    else if (i is ExplodeIntention ei)
                        totalDmg += ei.CalculateDamage(ei.Damage);
                    else if (i is KokoroDarkIntention kki)
                        totalDmg += kki.CalculateDamage(kki.Damage);
                    else if (i is SpellCardIntention si)
                        totalDmg += SpellCardDmgEstimate(si);
                }
            }

            Count = (Math.Max(totalDmg - (Owner.Block + Owner.Shield), 0) * nmMult).RoundToInt(MidpointRounding.AwayFromZero);
        }

        public int SpellCardDmgEstimate(SpellCardIntention si)
        {
            DamageInfo? damage = si.Damage;
            if (damage == null)
            {
                return 0;
            }
            DamageInfo valueOrDefault = damage.GetValueOrDefault();
            if (si.Times == null)
            {
                return si.CalculateDamage(valueOrDefault);
            }
            return si.CalculateDamage(valueOrDefault) * si.Times.Value;
        }

        protected override void OnAdded(Unit unit)
        {
            Count = 0;
            HandleOwnerEvent(unit.DamageTaking, OnDamageTaking, GameEventPriority.Lowest);
            // unneeded really
            //React(unit.BlockShieldGained)

            (unit.View as MonoBehaviour)?.StartCoroutine(UpdateDmgEstimate());
        }

        protected override void OnRemoved(Unit unit)
        {
            (unit.View as MonoBehaviour)?.StopCoroutine(UpdateDmgEstimate());
        }



        private void OnDamageTaking(DamageEventArgs args)
        {
            if (args.Cause == ActionCause.OnlyCalculate)
                return;


            if (Battle._resolver._reactors == null)
            {
                Log.LogError($"{this.Name} can't react outside of action resolving state. Aborting");
                return;
            }
            var nm = new NightmareInfo(args.DamageInfo.Damage * nmMult, true);
            React(new NightmareAction(args.Source, Owner, nm, 0f));

            args.DamageInfo = args.DamageInfo.ReduceActualDamageBy(args.DamageInfo.Damage.CeilingToInt());

            if (Owner.IsDead)
                args.CancelBy(this);
        }
    }
}
