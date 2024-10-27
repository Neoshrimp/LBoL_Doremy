using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Presentation.UI.Widgets;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.ExtraAssets;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LBoL_Doremy.DoremyChar.Visuals
{

    [HarmonyPatch(typeof(CardWidget), nameof(CardWidget.LazySetCard))]
    class CardWidget_LazySetCard_Patch
    {
        static void Postfix(CardWidget __instance)
        {
            var card = __instance.Card;
            if (card?.WasGenerated() ?? false)
            {
                var go = GameObject.Instantiate(__instance.baseLoyaltyObj, __instance.baseLoyaltyObj.transform.parent, worldPositionStays: true);

                go.name = "CreatedIcon";
                GameObject.Destroy(go.transform.Find("BaseLoyaltyText").gameObject);
                go.transform.localPosition += new Vector3(0, 285, 0);

                var img = go.GetComponent<Image>();
                img.sprite = AssetManager.DoremyAssets.CreatedIcon;

                go.SetActive(true);

            }
        }
    }


}
