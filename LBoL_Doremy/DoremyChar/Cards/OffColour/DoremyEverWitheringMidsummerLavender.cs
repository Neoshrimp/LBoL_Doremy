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
using LBoL.Core.Battle.BattleActions;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{

    public sealed class DoremyEverWitheringMidsummerLavenderDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;
            con.Rarity = Rarity.Rare;
            

            con.Colors = new List<ManaColor>() { ManaColor.Green };
            con.Cost = new ManaGroup() { Green = 1, Hybrid = 2, HybridColor = 0};


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DoremyEverWitheringMidsummerLavenderDef))]
    public sealed class DoremyEverWitheringMidsummerLavender : DCard
    {

        public BlockInfo NMBlockEst => new BlockInfo(ConsumeEstimate);

        public const float NMMult = 1.5f;

        public BlockInfo NMBarrierEst => GetNMBarrier(ConsumeEstimate);


        BlockInfo GetNMBarrier(int nmConsume)
        {
            return new BlockInfo((nmConsume * NMMult).RoundToInt(MidpointRounding.AwayFromZero));
        }

        int ConsumeEstimate
        {
            get
            {
                if (RealBattle == null)
                    return 0;

                return RealBattle.AllAliveUnits.Select(u =>
                {
                    if (u.TryGetStatusEffect<DC_NightmareSE>(out var nightmareSE))
                        return nightmareSE.Level;
                    return 0;

                }).Sum();
            }
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return PerformAction.Effect(Battle.Player, "MoonG");
            int consumedNM = 0;
            foreach (var u in Battle.AllAliveUnits)
                if (u.TryGetStatusEffect<DC_NightmareSE>(out var nightmareSE))
                {
                    consumedNM += nightmareSE.Level;
                    yield return new RemoveStatusEffectAction(nightmareSE);
                }


            //yield return DefenseAction(block: consumedNM, 0);
            //    Gain |Block| equal to the |Nightmare| consumed({NMBlockEst}).

            var shield = (consumedNM * NMMult).RoundToInt(MidpointRounding.AwayFromZero);

            if (!IsUpgraded)
            {
                var bInfo = GetNMBarrier(consumedNM);
                var args = new BlockShieldEventArgs()
                {
                    Source = Battle.Player,
                    Target = Battle.Player,
                    Block = 0,
                    Shield = bInfo.Block,
                    ActionSource = this,
                    Type = bInfo.Type,
                    HasBlock = false,
                    HasShield = true,
                    Cause = ActionCause.OnlyCalculate
                };
                Battle.Player.BlockShieldCasting.Execute(args);
                Battle.Player.BlockShieldGaining.Execute(args);

                var buffAction = (ApplyStatusEffectAction)BuffAction<DoremyEverWitheringMidsummerLavenderSE>(args.Shield.RoundToInt(MidpointRounding.AwayFromZero), duration: 1);
                yield return buffAction;
            }
            else
                yield return DefenseAction(0, shield);

        }
    }



    public sealed class DoremyEverWitheringMidsummerLavenderSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = false;
            con.HasLevel = true;
            con.LevelStackType = StackType.Keep;

            con.HasDuration = true;
            con.DurationStackType = StackType.Keep;
            con.DurationDecreaseTiming = DurationDecreaseTiming.TurnStart;


            return con;
        }
    }



    [EntityLogic(typeof(DoremyEverWitheringMidsummerLavenderSEDef))]
    public sealed class DoremyEverWitheringMidsummerLavenderSE : DStatusEffect
    {

        protected override void OnRemoving(Unit unit)
        {
            if (Battle.BattleShouldEnd)
                return;
            if (Battle._resolver._reactors == null)
            {
                Log.LogWarning($"{Name} tries to add block outside of reactor state.");
                return;
            }

            React(new CastBlockShieldAction(unit, block: 0, shield: Level, BlockShieldType.Direct));
        }

    }
}
