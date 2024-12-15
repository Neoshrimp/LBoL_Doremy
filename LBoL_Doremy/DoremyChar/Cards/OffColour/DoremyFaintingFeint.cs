using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Intentions;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LBoL.Core.Units;
using LBoL.Base.Extensions;
using LBoL_Doremy.DoremyChar.Actions;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{

    public sealed class DoremyFaintingFeintDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Nobody;
            con.Rarity = Rarity.Rare;

            con.Colors = new List<ManaColor>() { ManaColor.Red };
            con.Cost = new ManaGroup() { Red = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };



            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DoremyFaintingFeintDef))]
    public sealed class DoremyFaintingFeint : DCard
    {


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyFaintingFeintSE>();
        }
    }



    public sealed class DoremyFaintingFeintSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = true;
            con.HasLevel = false;


            return con;
        }
    }



    [EntityLogic(typeof(DoremyFaintingFeintSEDef))]
    public sealed class DoremyFaintingFeintSE : DStatusEffect
    {
       

        protected override void OnAdded(Unit unit)
        {
            HandleOwnerEvent(unit.DamageGiving, OnDamageTaking, GameEventPriority.Lowest);
        }



        private void OnDamageTaking(DamageEventArgs args)
        {
            if (args.Cause == ActionCause.OnlyCalculate)
                return;

            
            if (args.DamageInfo.DamageType == DamageType.Attack)
            {
                if (Battle._resolver._reactors == null)
                {
                    Log.LogError($"{this.Name} can't react outside of action resolving state. Aborting");
                    return;
                }
                React(Convert2NMSequence(args));

                var nm = new NightmareInfo(args.DamageInfo.Damage, false);
                React(new NightmareAction(Owner, args.Target, nm, 0f));

                args.DamageInfo = args.DamageInfo.ReduceActualDamageBy(args.DamageInfo.Damage.CeilingToInt());

                // doesnt do anything
                /*if (args.Target.IsDead || args.Target.IsDying)
                    args.CancelBy(this);*/
            }

        }

        private IEnumerable<BattleAction> Convert2NMSequence(DamageEventArgs args)
        {
            var nm = new NightmareInfo(args.DamageInfo.Damage, false);
            yield return new NightmareAction(Owner, args.Target, nm, 0f);

            args.DamageInfo = args.DamageInfo.ReduceActualDamageBy(args.DamageInfo.Damage.CeilingToInt());

            if (args.Target.IsDead || args.Target.IsDying)
            {
                args.CancelBy(this);
            }
        }
    }
}
