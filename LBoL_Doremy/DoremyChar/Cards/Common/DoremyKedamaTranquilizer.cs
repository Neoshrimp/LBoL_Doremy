using LBoL.Base;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.ConfigData;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoLEntitySideloader.Attributes;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.StaticResources;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyKedamaTranquilizerDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 2 };



            con.Value1 = 16;
            con.UpgradedValue1 = 22;



            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };

            con.SubIllustrator = new string[] { Artists.kimmchu };


            return con;
        }
    }

    [EntityLogic(typeof(DoremyKedamaTranquilizerDef))]
    public sealed class DoremyKedamaTranquilizer : DCard
    {

        public NightmareInfo NM2Apply => Value1;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            // 2do add gun id


            yield return PerformAction.Gun(Battle.Player, selector.SelectedEnemy, "MaoyuU");
            yield return NightmareAction(selector.SelectedEnemy, NM2Apply);
        }
    }
}
