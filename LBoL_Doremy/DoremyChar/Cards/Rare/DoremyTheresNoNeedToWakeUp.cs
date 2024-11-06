using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyTheresNoNeedToWakeUpDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.AllEnemies;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { Hybrid = 2, HybridColor = 0 };

            con.Value1 = 1;
            con.UpgradedValue1 = 2;


            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;



            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyTheresNoNeedToWakeUpDef))]
    public sealed class DoremyTheresNoNeedToWakeUp : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var e in UnitSelector.AllEnemies.GetEnemies(Battle))
            {
                if (e.TryGetStatusEffect<DC_NightmareSE>(out var nightmare))
                {
                    yield return NightmareAction(e, nightmare.Level * Value2, 0.05f);
                }
            }
        }
    }
}
