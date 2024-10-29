using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Presentation.UI.Widgets;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Keywords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{
    public class DreamLayerCard : DCard
    {

        int _dreamLevel = 0;
        public int DreamLevel 
        {   get => _dreamLevel;
            internal set
            {
                _dreamLevel = value; _dreamLevel = Math.Max(0, _dreamLevel);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            this.AddCustomKeyword(DoremyKw.DreamLayer);
        }


        public virtual bool ShowDreamLevel { get => true; }

        public virtual void OnDLChanged(DreamLevelArgs args) {}

        protected override void OnEnterBattle(BattleController battle)
        {
            // 2DO sort reaction order by hand order
            ReactBattleEvent(GetBounceEvent(battle), OnPlayerTurnEnd, (GameEventPriority)bouncePriority);
        }

        public static int bouncePriority = 10;
        public static GameEvent<UnitEventArgs> GetBounceEvent(BattleController battle) => battle.Player.TurnEnding;

        
        private IEnumerable<BattleAction> OnPlayerTurnEnd(UnitEventArgs args)
        {
            if (Zone == LBoL.Core.Cards.CardZone.Hand)
            { 
                yield return new ApplyDLAction(this, isEndOfTurnBounce: true);
                yield return new MoveCardToDrawZoneAction(this, DrawZoneTarget.Random);
            }
        }


        [HarmonyPatch(typeof(Card), nameof(Card.CloneBattleCard))]
        class CloneBattleCard_Patch
        {
            static void Postfix(Card __instance, Card __result)
            {
                if(__instance is DreamLayerCard source && __result is DreamLayerCard copy)
                    copy.DreamLevel = source.DreamLevel;
            }
        }



        [HarmonyPatch]
        internal class BoostCardWidgetPatch
        {
            [HarmonyPatch(typeof(CardWidget), "SetProperties")]
            private static void Postfix(CardWidget __instance)
            {
                if (__instance._card is DreamLayerCard dreamLayerCard 
                    && dreamLayerCard.Config.Loyalty == null
                    && dreamLayerCard.ShowDreamLevel)
                {
                    __instance.baseLoyaltyObj.SetActive(true);
                    __instance.baseLoyalty.text = dreamLayerCard.DreamLevel.ToString();
                }
            }
        }


        static HashSet<string> allDreamLayerCards;
        public static HashSet<string> AllDreamLayerCards
        {
            get
            {
                if (allDreamLayerCards == null)
                {
                    allDreamLayerCards = new HashSet<string>();
                    var ass = typeof(DreamLayerCard).Assembly;
                    ass.ExportedTypes.Where(t => t.IsSubclassOf(typeof(DreamLayerCard))).Select(t => t.Name).Do(id => allDreamLayerCards.Add(id));
                }
                return allDreamLayerCards;
            }
        }
    }
}
