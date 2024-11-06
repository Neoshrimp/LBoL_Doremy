using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{

    public sealed class DoremyCreepingBulletDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2, Any = 3 };

            con.Damage = 20;
            con.UpgradedDamage = 25;

            con.Value1 = 20;
            con.UpgradedValue1 = 25;

            con.Mana = new ManaGroup() { Any = 1 };


            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyCreepingBulletDef))]
    public sealed class DoremyCreepingBullet : DreamLayerCard
    {

        public NightmareInfo NM2Apply => Value1;

        protected override ManaGroup AdditionalCost => Mana * -DreamLevel;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            yield return NightmareAction(selector.SelectedEnemy, NM2Apply);
        }
    }
}
