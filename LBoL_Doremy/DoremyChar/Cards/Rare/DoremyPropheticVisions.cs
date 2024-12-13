using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare
{
    public sealed class DoremyPropheticVisionsDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.AllEnemies;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { Any = 0 };

            con.Value1 = 1;
            con.UpgradedValue1 = 2;

            con.Value2 = 8;

            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile;

            con.RelativeKeyword = Keyword.CopyHint;
            con.UpgradedRelativeKeyword = Keyword.CopyHint;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE), nameof(DC_SelfNightmareTooltipSE) };

            return con;
        }
    }



    [EntityLogic(typeof(DoremyPropheticVisionsDef))]
    public sealed class DoremyPropheticVisions : DCard
    {

        public NightmareInfo SelfNM2Apply => new NightmareInfo(Value2, true);

        public override Interaction Precondition()
        {
            var drawZone = Battle.DrawZoneToShow.Where(c => c.CanBeDuplicated);
            var selection = new SelectCardInteraction(0, Value1, drawZone) {
                Description = Name + (IsUpgraded ? "+" : "")

            };

            if (drawZone.FirstOrDefault() == null)
                selection.Description += LocalizeProperty("NoCopyFound", true).RuntimeFormat(FormatWrapper);
            else
                selection.Description += LocalizeProperty("UpTo", true).RuntimeFormat(FormatWrapper);

            return selection;
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            if (precondition is SelectCardInteraction interaction) 
            {
                yield return new AddCardsToHandAction(interaction.SelectedCards.Select(c => c.CloneBattleCard()));
                yield return NightmareAction(Battle.Player, SelfNM2Apply);
            }
        }
    }
}
