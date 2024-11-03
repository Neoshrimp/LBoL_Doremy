using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.StatusEffects.Reimu;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Rare.DreamTeamates
{

    public sealed class DoremyDreamTeamDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Rare;

            con.Type = CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2, Any = 3 };


            con.Value1 = 1;
            con.UpgradedValue1 = 3;


            con.RelativeCards = new List<string>() { nameof(DoremyDreamyReimu) };
            con.UpgradedRelativeCards = new List<string>() { nameof(DoremyDreamyReimu) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamTeamDef))]
    public sealed class DoremyDreamTeam : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var cards = EnumerateRelativeCards();
            var options = cards.SampleManyOrAll(Value1, GameRun.BattleCardRng);
            Card selectedCard = null;
            if (options.Length > 1)
            {
                var selection = new MiniSelectCardInteraction(options);
                yield return new InteractionAction(selection, false);
                selectedCard = selection.SelectedCard;
            }
            else
                selectedCard = options.FirstOrDefault();

            if (selectedCard != null) 
                yield return new AddCardsToHandAction(options);

        }
    }
}
