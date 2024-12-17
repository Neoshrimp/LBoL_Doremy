using LBoL.Base;
using LBoL.ConfigData;
using LBoL_Doremy.DoremyChar.DoremyPU;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader;
using System;
using System.Collections.Generic;
using System.Text;
using LBoLEntitySideloader.Attributes;
using LBoL.EntityLib.Cards.Enemy;
using LBoL.EntityLib.Cards.Neutral.Blue;
using HarmonyLib;
using LBoL.Core.Battle;
using LBoLEntitySideloader.Utils;
using System.Linq;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL.Presentation;

namespace LBoL_Doremy.DoremyChar.VanillaTweaks
{
/*    [OverwriteVanilla]
    public sealed class DCardDef : CardTemplate
    {
        public override IdContainer GetId() => nameof(HatateDiscard);

        [DontOverwrite]
        public override CardImages LoadCardImages()
        {
            return null;
        }

        [DontOverwrite]
        public override LocalizationOption LoadLocalization()
        {
            return null;
        }

        [DontOverwrite]
        public override CardConfig MakeConfig()
        {
            var con = CardConfig.FromId(nameof(HatateDiscard)).Copy();
            con.RelativeEffects = con.RelativeEffects.Concat(new string[] { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) }).ToList();
            con.UpgradedRelativeEffects = con.RelativeEffects.Concat(new string[] { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) }).ToList();

            return con;
        }
    }*/




    //[HarmonyPatch(typeof(Card), nameof(Card.Initialize))]
    class HatateInit_Patch
    {
        static void Postfix(Card __instance)
        {
            var card = __instance;

            if (card.Id == nameof(HatateDiscard) 
                && GameMaster.Instance.CurrentGameRun?.Player.Id == nameof(DoremyCavalier))
            {
                card.Config = card.Config.Copy();
                var con = card.Config;

                con.RelativeEffects = con.RelativeEffects.Concat(new string[] { nameof(DC_VanillaExTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) }).ToList();
                con.UpgradedRelativeEffects = con.RelativeEffects.Concat(new string[] { nameof(DC_VanillaExTooltipSE), nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) }).ToList();

            }

        }
    }

    //[HarmonyPatch(typeof(Card), "GetBaseDescription")]
    class HatateGetBaseDescription_Patch
    {
        static void Postfix(Card __instance, ref string __result)
        {
            var card = __instance;
            if (card.Id == nameof(HatateDiscard)
                && card.GameRun?.Player.Id == nameof(DoremyCavalier))
            {
                __result += DC_VanillaLocHelper.GetLoc("NewsFab");
            }

        }
    }


    //[HarmonyPatch(typeof(HatateDiscard), "Actions")]
    class NewsFab_Patch
    {
        static IEnumerable<BattleAction> Postfix(IEnumerable<BattleAction> actions, HatateDiscard __instance)
        {
            var card = __instance;
            foreach (var a in actions)
                yield return a;

            if(card.GameRun?.Player.Id == nameof(DoremyCavalier))
                yield return new NightmareAction(card.Battle.Player, card.Battle.Player, new NightmareInfo(5, true), 0.15f);
        }
    }


}
