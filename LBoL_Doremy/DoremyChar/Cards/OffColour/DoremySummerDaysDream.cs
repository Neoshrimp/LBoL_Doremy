using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.BattleTracking;
using System.Linq;
using LBoL_Doremy.Utils;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Units;
using LBoL.Presentation.UI.Panels;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{
    public sealed class DoremySummerDaysDreamDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;
            con.Rarity = Rarity.Uncommon;

            con.Colors = new List<ManaColor>() { ManaColor.Green };
            con.Cost = new ManaGroup() { Green = 1, Any = 2 };


            con.Value1 = 1;


            con.Value2 = 3;

            con.RelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };

            return con;
        }
    }

    [EntityLogic(typeof(DoremySummerDaysDreamDef))]
    public sealed class DoremySummerDaysDream : DCard
    {

      
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
            if (IsUpgraded)
            {
/*                var buffAction = (ApplyStatusEffectAction)BuffAction<DoremySummerDaysDreamQueueSE>();
                yield return buffAction;
                var status = buffAction.Args.Effect as DoremySummerDaysDreamQueueSE;
                if (status != null)
                    status.UpdateQueue();*/
            }

        }
    }



    public sealed class DoremySummerDaysDreamQueueSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.IsStackable = false;
            con.HasCount = true;
            con.HasLevel = false;


            return con;
        }
    }



    [EntityLogic(typeof(DoremySummerDaysDreamQueueSEDef))]
    public sealed class DoremySummerDaysDreamQueueSE : DC_ExileQeueuSE
    {
        List<Card> _toBounceQueue = new List<Card>();
        public List<Card> ToBounceQueue { get => _toBounceQueue; set => _toBounceQueue = value; }


        public void UpdateQueue(IEnumerable<Card> cards2Add)
        {
            foreach (var c in cards2Add)
                ToBounceQueue.Add(c);
            UpdateCount(ToBounceQueue);
        }
        protected override string GetNoTargetCardInExile() => LocalizeProperty("NotInExile", true);

        public string QueuedCardsDesc => GetQueuedCardsDesc(ToBounceQueue);

        protected override IEnumerable<Card> UpdateQueueContainer(IEnumerable<Card> queue)
        {
            ToBounceQueue = queue.ToList();
            return ToBounceQueue;
        }

        protected override void OnAdded(Unit unit)
        {
            base.OnAdded(unit);
            if (unit is PlayerUnit pu)
            {
                ReactOwnerEvent(pu.TurnStarted, TurnStarted, (GameEventPriority)exileQueuePriority);
            }
        }



        private IEnumerable<BattleAction> TurnStarted(UnitEventArgs args)
        {
            return ProcessQueue(ToBounceQueue);
        }

        protected override IEnumerable<BattleAction> ProcessQueue(IList<Card> queue)
        {
            foreach (var a in base.ProcessQueue(queue))
                yield return a;

            if (ToBounceQueue.Count == 0)
                yield return new RemoveStatusEffectAction(this);
            else
                UpdateCount(queue);
        }

    }

}
