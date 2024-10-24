using HarmonyLib;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using LBoLEntitySideloader.CustomHandlers;
using LBoL_Doremy.DoremyChar.Actions;
using System.ComponentModel.Design;
using LBoL.Core.StatusEffects;
using LBoL.Core.Battle.BattleActions;

namespace LBoL_Doremy.DoremyChar.CreatedCardTracking
{
    public static class BattleHandlers
    {

        static ConditionalWeakTable<BattleController, BattleCreationInfo> cwt_cardCreationTurnHistory = new ConditionalWeakTable<BattleController, BattleCreationInfo>();

        static BattleCreationInfo GetInfo(BattleController battle)
        {
            cwt_cardCreationTurnHistory.TryGetValue(battle, out var info);
            return info;
        }

        public static CreatedCount CreatedCount 
        {
            get
            {
                var battle = EventManager.Battle;
                if(battle == null)
                    return new CreatedCount();
                var rez = GetInfo(battle)?.createdCount;
                if (rez == null)
                    return new CreatedCount();
                return rez;
            }
        }

        public static CardCreationTurnHistory GetCardCreationTurnHistory(BattleController battle)
        {
            cwt_cardCreationTurnHistory.TryGetValue(battle, out var info);
            return info.cardCreationTurnHistory;
        }

        public static CardCreationTurnHistory CardCreationTurnHistory { get => GetCardCreationTurnHistory(EventManager.Battle); }

        public static void RegisterAll()
        {
            RegisterTurnHistHandlers();
            RegisterCommonHandlers(TrackCount);
        }

        private static void TrackCount(Card[] cards, GameEventArgs args)
        {
            var createdCount = CreatedCount;
            int increment = cards.Length;

            // card ownership (original source) could be tracked
            if (args.ActionSource is Card)
                createdCount.byPlayer += increment;
            else if (args.ActionSource is EnemyUnit)
                createdCount.byEnemies += increment;
            else if (args.ActionSource is Exhibit)
                createdCount.byPlayer += increment;
            else if (args.ActionSource is StatusEffect se)
                if(se.Owner is PlayerUnit)
                    createdCount.byPlayer += increment;
                else if(se.Owner is PlayerUnit)
                    createdCount.byEnemies += increment;
                else
                    createdCount.byOther += increment;
            else if (args.ActionSource is UltimateSkill)
                createdCount.byPlayer += increment;
            else if (args.ActionSource is PlayerUnit)
                createdCount.byPlayer += increment;
            else
                createdCount.byOther += increment;

        }

        static void RegisterCommonHandlers(Action<Card[], GameEventArgs> handler, GameEventPriority priority = GameEventPriority.ConfigDefault)
        {
            GameEventHandler<CardsAddingToDrawZoneEventArgs> drawZoneReactor = args => handler(args.Cards, args);
            GameEventHandler<CardsEventArgs> otherReactors = args => handler(args.Cards, args);

            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToExile, otherReactors, null, priority);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToDrawZone, drawZoneReactor, null, priority);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToHand, otherReactors, null, priority);
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToDiscard, otherReactors, null, priority);
        }



        static void RegisterTurnHistHandlers()
        {
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToExile,
                args => CardCreationTurnHistory.addedToExile.AddRange(args.Cards));
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToDrawZone,
                args => CardCreationTurnHistory.addedToDrawZone.AddRange(args.Cards));
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToHand,
                args => CardCreationTurnHistory.addedToHand.AddRange(args.Cards));
            CHandlerManager.RegisterBattleEventHandler(bt => bt.CardsAddedToDiscard,
                args => CardCreationTurnHistory.addedToDiscard.AddRange(args.Cards));
        }




        [HarmonyPatch(typeof(StartPlayerTurnAction), nameof(StartPlayerTurnAction.GetPhases))]
        class StartPlayerTurnAction_Patch
        {
            static void Prefix()
            {
                var hist = CardCreationTurnHistory;
                hist.Clear();
            }
        }



        [HarmonyPatch(typeof(BattleController), nameof(BattleController.EndPlayerTurn))]
        class BattleController_EndPlayerTurn_Patch
        {
            static void Postfix(BattleController __instance)
            {
                var hist = GetCardCreationTurnHistory(__instance);
                hist.Clear();
            }
        }





        [HarmonyPatch(typeof(BattleController), MethodType.Constructor, new Type[] { typeof(GameRunController), typeof(EnemyGroup), typeof(IEnumerable<Card>) })]
        class BattleController_Patch
        {
            static void Prefix(BattleController __instance)
            {
                cwt_cardCreationTurnHistory.Add(__instance, new BattleCreationInfo());
            }
        }
    }

    public class BattleCreationInfo
    {
        public CardCreationTurnHistory cardCreationTurnHistory = new CardCreationTurnHistory();
        public CreatedCount createdCount = new CreatedCount();
    }

    public class CreatedCount
    {
        public int byPlayer = 0;
        public int byEnemies = 0;
        public int byOther = 0;

        public override string ToString()
        {
            return $"{byPlayer};{byEnemies};{byOther}";
        }


        public int Total => byPlayer + byEnemies + byOther;
    }

    public class CardCreationTurnHistory
    {
        public List<Card> addedToExile = new List<Card>();
        public List<Card> addedToDrawZone = new List<Card>();
        public List<Card> addedToHand = new List<Card>();
        public List<Card> addedToDiscard = new List<Card>();

        public int Total => addedToExile.Count + addedToDrawZone.Count + addedToHand.Count + addedToDiscard.Count;

        public void Clear()
        {
            this.addedToExile.Clear();
            this.addedToDrawZone.Clear();
            this.addedToHand.Clear();
            this.addedToDiscard.Clear();
        }

    }
}
