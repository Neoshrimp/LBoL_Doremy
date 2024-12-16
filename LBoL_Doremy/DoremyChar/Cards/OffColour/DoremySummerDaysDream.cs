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
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL_Doremy.DoremyChar.DreamManagers;
using UnityEngine;
using LBoLEntitySideloader.Resource;
using LBoL_Doremy.StaticResources;
using System.IO;
using System.Collections;

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



            con.RelativeCards = new string[] { nameof(SummerFlower) };
            con.UpgradedRelativeCards = new string[] { nameof(SummerFlower) };


            con.RelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_ExileQueueTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };

            return con;
        }
    }

    [EntityLogic(typeof(DoremySummerDaysDreamDef))]
    public sealed class DoremySummerDaysDream : DCard
    {
        public string UpgradeDesc => IsUpgraded ? LocalizeProperty("UpgradeTxt", true).RuntimeFormat(FormatWrapper) : "";


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremySummerDaysDreamMainSE>(Value1, count: Value2);

            if (IsUpgraded)
            {
                var buffAction = (ApplyStatusEffectAction)BuffAction<DoremySummerDaysDreamQueueSE>();
                yield return buffAction;
                var status = buffAction.Args.Effect as DoremySummerDaysDreamQueueSE;
                if (status != null)
                    status.UpdateQueue(Library.CreateCards<SummerFlower>(2));
            }

        }
    }



    public sealed class DoremySummerDaysDreamQueueSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite(nameof(DoremySummerDaysDreamMainSE) + ".png", Sources.imgsSource);

        }
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
    public sealed class DoremySummerDaysDreamQueueSE : DC_ExileQueueSE
    {
        List<Card> _toBounceQueue = new List<Card>();
        public List<Card> ToBounceQueue { get => _toBounceQueue; set => _toBounceQueue = value; }


        public void UpdateQueue(IEnumerable<Card> cards2Add)
        {
            foreach (var c in cards2Add)
                ToBounceQueue.Add(c);
            UpdateCount(ToBounceQueue);
        }

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
            foreach (var a in base.ProcessQueue(ToBounceQueue))
                yield return a;

            if (ToBounceQueue.Count == 0)
                yield return new RemoveStatusEffectAction(this);
            else
                UpdateCount(ToBounceQueue);
        }


    }

    public sealed class DoremySummerDaysDreamMainSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = StatusEffectType.Positive;

            con.HasCount = true;
            con.CountStackType = StackType.Add;

            return con;
        }
    }



    [EntityLogic(typeof(DoremySummerDaysDreamMainSEDef))]
    public sealed class DoremySummerDaysDreamMainSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(unit.TurnEnding, ConsumeNM, (GameEventPriority)(DreamLayerHandlers.bouncePriority - 2));
            ReactOwnerEvent(unit.TurnEnding, QFlowers, (GameEventPriority)(DreamLayerHandlers.bouncePriority + 2));

        }

        private IEnumerable<BattleAction> QFlowers(UnitEventArgs args)
        {
            var buffAction = (ApplyStatusEffectAction)BuffAction<DoremySummerDaysDreamQueueSE>();
            yield return buffAction;
            var status = buffAction.Args.Effect as DoremySummerDaysDreamQueueSE;
            if (status != null)
                status.UpdateQueue(Library.CreateCards<SummerFlower>(Level));
        }
        

        public NightmareInfo NM2ConsumeDesc => new NightmareInfo(Count, true);
        public NightmareInfo NM2Consume => new NightmareInfo(-Count, true);

        private IEnumerable<BattleAction> ConsumeNM(UnitEventArgs args)
        {
            if (Owner.HasStatusEffect<DC_NightmareSE>())
                yield return NightmareAction(Owner, NM2Consume);
        }
    }

}
