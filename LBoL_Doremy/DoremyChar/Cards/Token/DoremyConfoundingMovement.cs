using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Token
{
    public sealed class DoremyConfoundingMovementDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.IsPooled = false;
            con.HideMesuem = true;
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.AllEnemies;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 0 };

            con.Value1 = 2;
            con.UpgradedValue1 = 4;

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;


            con.RelativeEffects = new List<string>() { nameof(TempFirepowerNegative) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(TempFirepowerNegative) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyConfoundingMovementDef))]
    public sealed class DoremyConfoundingMovement : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var e in UnitSelector.AllEnemies.GetUnits(Battle))
                yield return DebuffAction<TempFirepowerNegative>(e, Value1);
        }
    }
}
