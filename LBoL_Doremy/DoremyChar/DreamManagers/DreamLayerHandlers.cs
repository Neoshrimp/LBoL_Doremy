using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Presentation;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Cards.Rare;
using LBoLEntitySideloader.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LBoL_Doremy.DoremyChar.Keywords;
using HarmonyLib;
using LBoL.Presentation.UI.Widgets;
using LBoL.Core.Cards;
using LBoL_Doremy.ExtraAssets;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
using LBoL.Base.Extensions;
using System.Reflection.Emit;

namespace LBoL_Doremy.DoremyChar.DreamManagers
{
    internal static class DreamLayerHandlers
    {
        public static int bouncePriority = 10;

        public static GameEvent<UnitEventArgs> GetBounceEvent(BattleController battle) => battle.Player.TurnEnding;


        public static void RegisterHandlers()
        {
            CHandlerManager.RegisterBattleEventHandler(b => GetBounceEvent(b), OnPlayerTurnEnd, null, (GameEventPriority)bouncePriority);


            CHandlerManager.RegisterBattleEventHandler(b => b.Player.DamageDealing, OnDmgDealing, null, (GameEventPriority)20);
            CHandlerManager.RegisterBattleEventHandler(b => b.Player.BlockShieldCasting, OnSBGaining, null, (GameEventPriority)20);
            CHandlerManager.RegisterBattleEventHandler(b => EventManager.GetDoremyEvents(b).nightmareEvents.nigtmareApplying, OnNMApplying, null, (GameEventPriority)20);



        }


        static float BoostMultiplier(int dreamLevel) => 1 + (EventManager.DoremyEvents.DLperLevelMult * dreamLevel);

        private static void OnNMApplying(NightmareArgs args)
        {
            if (args.ActionSource is Card card && card.TryGetCustomKeyword(DoremyKw.dLId, out DLKeyword DL))
            {
                args.level *= BoostMultiplier(DL.DreamLevel);
                args.AddModifier(card);
            }
        }

        private static void OnSBGaining(BlockShieldEventArgs args)
        {
            
            if (args.Type == BlockShieldType.Normal &&
                args.ActionSource is Card card && card.TryGetCustomKeyword(DoremyKw.dLId, out DLKeyword DL))
            {
                args.Block = args.Block * BoostMultiplier(DL.DreamLevel);
                args.Shield = args.Shield * BoostMultiplier(DL.DreamLevel);

                args.AddModifier(card);
            }
        }

        private static void OnDmgDealing(DamageDealingEventArgs args)
        {
            if (args.DamageInfo.DamageType == LBoL.Base.DamageType.Attack &&
                args.ActionSource is Card card && card.TryGetCustomKeyword(DoremyKw.dLId, out DLKeyword DL))
            {
                var info = args.DamageInfo;
                info.Damage = args.DamageInfo.Amount * BoostMultiplier(DL.DreamLevel);
                args.DamageInfo = info;

                args.AddModifier(card);
            }
        }

        private static void OnPlayerTurnEnd(UnitEventArgs args)
        {
            var battle = GameMaster.Instance?.CurrentGameRun?.Battle;
            if (battle == null)
                return;


            foreach (var card in battle.HandZone.Where(c => c.HasCustomKeyword(DoremyKw.dreamLayerId)))
            {
                battle.React(new ApplyDLAction(card, isEndOfTurnBounce: true), card, ActionCause.Card);
                var zoneTarget = DrawZoneTarget.Random;
                if (battle.Player.HasStatusEffect<DoremyFastAsleepSE>())
                    zoneTarget = DrawZoneTarget.Top;
                battle.React(new MoveCardToDrawZoneAction(card, zoneTarget), card, ActionCause.Card);
            }

        }








        [HarmonyPatch]
        internal class BoostCardWidgetPatch
        {
            public const string DLGoName = "DLStackTracker";



            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(CardWidget), "SetProperties");
                yield return AccessTools.Method(typeof(CardWidget), nameof(CardWidget.LazySetCard));

            }


            private static void Postfix(CardWidget __instance)
            {
                var dlGo =__instance.baseLoyaltyObj.transform.parent.Find(DLGoName)?.gameObject;
                if (__instance._card.TryGetCustomKeyword(DoremyKw.dLId, out DLKeyword dl))
                {

                    if (dlGo == null)
                    {
                        dlGo = GameObject.Instantiate(__instance.baseLoyaltyObj, __instance.baseLoyaltyObj.transform.parent, worldPositionStays: true);
                        dlGo.name = DLGoName;
                        dlGo.transform.localPosition += new Vector3(0, 95, 0);

                        var img = dlGo.GetComponent<Image>();
                        img.sprite = AssetManager.DoremyAssets.dlTrackerIcon;
                    }

                    var tmpTxt = dlGo.transform.Find("BaseLoyaltyText").gameObject.GetComponent<TextMeshProUGUI>();
                    tmpTxt.text = dl.DreamLevel.ToString();

                    dlGo.SetActive(true);

                }
                else
                {
                    if (dlGo != null)
                        dlGo.SetActive(false);
                }
            }
        }
    }
}
