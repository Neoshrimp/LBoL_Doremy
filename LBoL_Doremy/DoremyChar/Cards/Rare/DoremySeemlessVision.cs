using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremySeemlessVisionDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.AllEnemies;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 2 };


            con.Value1 = 20;
            con.UpgradedValue1 = 25;

            con.Value2 = 2;

            con.Mana = new ManaGroup() { Any = 2 };

            con.RelativeKeyword = Keyword.TempMorph;
            con.UpgradedRelativeKeyword = Keyword.TempMorph;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremySeemlessVisionDef))]
    public sealed class DoremySeemlessVision : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach(var e in UnitSelector.AllEnemies.GetEnemies(Battle))
                yield return DebuffAction<DC_NightmareSE>(e, Value1);

            foreach (var c in Battle.HandZone.Where(c => c.WasGenerated() && c.Cost.Amount > 0))
            {
                var discountMana = Mana;
                if (IsUpgraded)
                {
                    ManaColor[] array = c.Cost.EnumerateComponents().SampleManyOrAll(Value2, GameRun.BattleRng);
                    discountMana = ManaGroup.FromComponents(array);
                }
                c.DecreaseTurnCost(discountMana);
            }
        }
    }
}
