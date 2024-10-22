using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Presentation.UI.Widgets;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Keywords;
using System;
using System.Collections.Generic;
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
            ReactBattleEvent(battle.Player.TurnEnding, OnPlayerTurnEnding);
        }

        private IEnumerable<BattleAction> OnPlayerTurnEnding(UnitEventArgs args)
        {
            if (Zone == LBoL.Core.Cards.CardZone.Hand)
            { 
                yield return new ApplyDLAction(this);
                yield return new MoveCardToDrawZoneAction(this, DrawZoneTarget.Random);
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

    }
}
