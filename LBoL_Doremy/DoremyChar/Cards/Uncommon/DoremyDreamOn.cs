using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL_Doremy.DoremyChar.BattleTracking;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyDreamOnDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.SingleEnemy;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 1, Any = 2 };

            con.Damage = 10;
            con.UpgradedDamage = 13;

            con.Value1 = 10;
            con.UpgradedValue1 = 13;

            con.Mana = new ManaGroup() { Philosophy = 3 };




            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamOnDef))]
    public sealed class DoremyDreamOn : DCard
    {
        public override bool Triggered
        {
            get
            {
                return Battle != null && BattleHistoryHandlers.GetCardCreationTurnHistory(Battle).addedToHand.NotEmpty();
            }
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            foreach (var a in base.Actions(selector, consumingMana, precondition))
                yield return a;

            yield return DebuffAction<DC_NightmareSE>(selector.SelectedEnemy, Value1);

            if (base.PlayInTriggered)
            {
                yield return new GainManaAction(base.Mana);
            }
        }
    }
}
