using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Sources;

namespace LBoL_Doremy.DoremyChar.Cards.Token
{
    public sealed class DoremySoporificCreativityDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.IsPooled = false;
            con.HideMesuem = true;
            con.Rarity = Rarity.Common;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 0 };

            con.Value1 = 2;
            con.Value2 = 3;
            con.UpgradedValue2 = 8;

            con.Keywords = Keyword.Exile | Keyword.Retain;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Retain;


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremySoporificCreativityDef))]
    public sealed class DoremySoporificCreativity : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new DrawManyCardAction(Value1);

            foreach(var e in UnitSelector.AllEnemies.GetUnits(Battle))
                yield return DebuffAction<DC_NightmareSE>(e, Value2);
        }
    }
}
