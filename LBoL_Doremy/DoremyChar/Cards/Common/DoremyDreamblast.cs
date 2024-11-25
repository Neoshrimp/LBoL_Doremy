using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.Actions;

using LBoL_Doremy.DoremyChar.DreamManagers;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremyDreamblastDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2, };

            con.Damage = 8;
            con.UpgradedDamage = 10;

            con.Value1 = 8;
            con.UpgradedValue1 = 10;

            con.Value2 = 1;
            con.UpgradedValue2 = 2;


            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE), nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE), nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamblastDef))]
    public sealed class DoremyDreamblast: NaturalDreamLayerCard
    {

        protected override int AdditionalDamage => Value2 * DreamLevel;

        public NightmareInfo NM2Apply { get => Value1 + Value2 * DreamLevel; }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            yield return NightmareAction(selector.SelectedEnemy, NM2Apply);

        }
    }
}
