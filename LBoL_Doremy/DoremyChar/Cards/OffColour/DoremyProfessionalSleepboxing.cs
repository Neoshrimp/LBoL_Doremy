using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL_Doremy.DoremyChar.DreamManagers;
using LBoL_Doremy.Actions;

namespace LBoL_Doremy.DoremyChar.Cards.OffColour
{
    public sealed class DoremyProfessionalSleepboxingDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.RandomEnemy;
            con.Rarity = Rarity.Rare;

            con.Colors = new List<ManaColor>() { ManaColor.Red };
            con.Cost = new ManaGroup() { Red = 1, Any = 2 };


            con.Damage = 3;
            con.UpgradedDamage = 5;


            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DoremyProfessionalSleepboxingDef))]
    public sealed class DoremyProfessionalSleepboxing : NaturalDreamLayerCard
    {

        public NightmareInfo NM2Apply => Value1;

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            for (int i = 0; i < DreamLevel; i++)
                yield return AttackAction(UnitSelector.RandomEnemy);
        }

        public override void OnDLChanged(DreamLevelArgs args)
        {
            React(AttackAction(UnitSelector.RandomEnemy));
            DreamLayerHandlers.CorruptedDLPenalty(this);
        }
    }
}
