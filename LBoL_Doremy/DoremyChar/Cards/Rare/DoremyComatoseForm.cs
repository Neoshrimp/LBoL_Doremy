using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Cards.Uncommon;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Cards;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL.Core.Units;
using LBoL.Core.Battle.BattleActions;
using LBoL.Presentation.Units;
using LBoL_Doremy.DoremyChar.DoremyPU;
using Spine;
using System.ComponentModel;
using System.Linq;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyComatoseFormDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { White = 2, Blue = 2, Hybrid = 1, HybridColor = 0 };

            con.Value1 = 1;

            con.Keywords = Keyword.Ethereal;

            con.RelativeKeyword = Keyword.CopyHint;
            con.UpgradedRelativeKeyword = Keyword.CopyHint;

            con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DLKwSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyComatoseFormDef))]
    public sealed class DoremyComatoseForm : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyComatoseFormSE>(Value1);
        }
    }


    public sealed class DoremyComatoseFormSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyComatoseFormSEDef))]
    public sealed class DoremyComatoseFormSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(EventManager.DLEvents.appliedDL, OnDLApplied);
            SetSleepAnim(unit, true);
        }

        private static void SetSleepAnim(Unit unit, bool enable)
        {
            if (unit.View is UnitView view && view._modelName == nameof(DoremyCavalier))
            {
                view.DoremySleeping = enable;

                if (enable)
                {
                    view._blinking = false;
                    foreach (AnimationState allState in view.AllStates)
                    {
                        var blinkTrack = allState.Tracks.FirstOrDefault(t => t.Animation.Name == "blink");
                        if (blinkTrack != null)
                            allState.SetEmptyAnimation(blinkTrack.TrackIndex, 0f);
                    }
                }
                else
                    view.Blink();
            }
        }

        protected override void OnRemoved(Unit unit)
        {
            SetSleepAnim(unit, false);
        }


        private IEnumerable<BattleAction> OnDLApplied(DreamLevelArgs args)
        {
            var target = args.target;
            if(!target.IsCopy)
            {
                NotifyActivating();

                var toAdd = new List<Card>();
                for (int i = 0; i < Level; i++)
                { 
                    var copy = args.target.CloneBattleCard();
                    if (!target.HasCustomKeyword(DoremyKw.DreamLayer))
                        target.IsCopy = true;
                        
                    if (args.isEndOfTurnBounce)
                        copy.IsTempRetain = true;
                    toAdd.Add(copy);
                }
                yield return new AddCardsToHandAction(toAdd);
            }
        }
    }
}
