using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Tool;
using LBoL.Presentation.UI.Panels;
using LBoL_Doremy.CreatedCardTracking;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyPerfectPhantasmsDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 3 };





            return con;
        }
    }


    [EntityLogic(typeof(DoremyPerfectPhantasmsDef))]
    public sealed class DoremyPerfectPhantasms : DCard
    {
        public string UpgradeDesc => IsUpgraded ?  LocalizeProperty("UpgradeTxt", true, true) : "";

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyPerfectPhantasmsSE>();
            if (IsUpgraded)
            {
                var toUpgrade = Battle.EnumerateAllCards().Where(c => c.WasGenerated() && c.CanUpgradeAndPositive).Where(c => c.CanUpgradeAndPositive);
                if (toUpgrade.FirstOrDefault() != null)
                    yield return new UpgradeCardsAction(toUpgrade);

            }
        }


        public sealed class DoremyPerfectPhantasmsSEDef : DStatusEffectDef
        {
            public override StatusEffectConfig PreConfig()
            {
                var con = DefaultConfig();
                con.Type = LBoL.Base.StatusEffectType.Positive;
                con.HasLevel = false;

                return con;
            }
        }

        [EntityLogic(typeof(DoremyPerfectPhantasmsSEDef))]
        public sealed class DoremyPerfectPhantasmsSE : DStatusEffect
        {
            protected override void OnAdded(Unit unit)
            {
                ReactOnCardsAddedEvents(OnCardsAdded);
            }

            private IEnumerable<BattleAction> OnCardsAdded(Card[] cards, GameEventArgs args)
            {
                var toUpgrade = cards.Where(c => c.CanUpgradeAndPositive);
                if (toUpgrade.FirstOrDefault() != null)
                    yield return new UpgradeCardsAction(toUpgrade);
            }
        }
    }
}
