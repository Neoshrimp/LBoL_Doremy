using HarmonyLib;
using LBoL.Presentation.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{
    public class DreamLayerCard : DCard
    {
        public int DreamLevel { get; set; }

        [HarmonyPatch]
        internal class BoostCardWidgetPatch
        {
            [HarmonyPatch(typeof(CardWidget), "SetProperties")]
            private static void Postfix(CardWidget __instance)
            {
                if (__instance._card is DreamLayerCard dreamLayerCard && dreamLayerCard.Config.Loyalty == null)
                {
                    __instance.baseLoyaltyObj.SetActive(true);
                    __instance.baseLoyalty.text = dreamLayerCard.DreamLevel.ToString();
                }
            }
        }

    }
}
