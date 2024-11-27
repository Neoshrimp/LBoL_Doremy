using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{

    public sealed class DoremyLabyrinthineConfusionDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Defense;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2 };


            con.Block = 7;
            con.UpgradedBlock = 10;


            //con.Value1 = 7;
            //con.UpgradedValue1 = 10;

            con.Value2 = 2;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyLabyrinthineConfusionDef))]
    public sealed class DoremyLabyrinthineConfusion : DCard
    {

        int GenCount
        {
            get
            {
                if (RealBattle == null)
                    return 0;

                var count = RealBattle.HandZone.Where(c => c.WasGenerated() && c != RealCard).Count();
                /*if (IsUpgraded)
                    count += RealBattle.DrawZone.Where(c => c.WasGenerated() && c != RealCard).Count();*/

                /*  UpgradedDescription: |-
                    |For each created card in {PlayerName}'s hand and draw pile|:
                    Gain {Block} |Block| and apply {NM2Apply} |Nightmare| to each enemy.{TimesHint}*/
                return count;
            }
        }

        private Card _realCard = null;
        public Card RealCard { get => _realCard == null ? this : _realCard; set => _realCard = value; }

        public string TimesHint => RealBattle == null ? "" : "\n"+LocalizeProperty("Times").RuntimeFormat(FormatWrapper);

        public NightmareInfo NM2Apply => Value2;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            for (int i = 0; i < GenCount; i++)
            {
                yield return DefenseAction(cast: false);
                foreach (var e in UnitSelector.AllEnemies.GetUnits(Battle))
                    yield return NightmareAction(e, NM2Apply, 0f);
            }

        }

        [HarmonyPatch(typeof(Card), nameof(Card.GetDetailInfoCard))]
        class SetRealCardForUI_Patch
        {
            static void Postfix(Card __instance, ref ValueTuple<Card, Card> __result)
            {
                if (__instance is DoremyLabyrinthineConfusion confusion)
                {
                    var card1 = (__result.Item1 as DoremyLabyrinthineConfusion);
                    if (card1 != null)
                        card1.RealCard = confusion;
                    var card2 = (__result.Item2 as DoremyLabyrinthineConfusion);
                    if (card2 != null)
                        card2.RealCard = confusion;
                }
            }
        }
    }

}
