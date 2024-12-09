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
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.DoremyChar.Cards.Uncommon;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Resource;
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

            con.Type = CardType.Ability;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { White = 2, Any = 3 };
            //con.UpgradedCost = new ManaGroup() { White = 1, Any = 4 };


            con.Value1 = 1;
            con.UpgradedValue1 = 2;

            var teamArray = new string[]
            {
                nameof(DoremyDreamyReimu),
                nameof(DoremyDreamyMarisa),
                nameof(DoremyDreamTeamNextPage),
                nameof(DoremyDreamySakuya),
                nameof(DoremyDreamyCirno),
                nameof(DoremyDreamyDoremy)
            };
            con.RelativeCards = teamArray.ToList();
            con.UpgradedRelativeCards = teamArray.ToList();

            return con;
        }
    }


    [EntityLogic(typeof(DoremyDreamTeamDef))]
    public sealed class DoremyDreamTeam : DCard
    {
        IEnumerable<Card> Teammates => EnumerateRelativeCards().Where(c => c.GetType() != typeof(DoremyDreamTeamNextPage));

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var cards = Teammates;
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
                yield return new AddCardsToHandAction(selectedCard);

        }
    }


    public sealed class DoremyDreamTeamNextPageDef : OptionCardDef
    {

        public override CardImages LoadCardImages()
        {
            var ci = new CardImages(Sources.imgsSource);
            ci.main = ResourceLoader.LoadTexture(nameof(DoremyDreamTeam) + ".png", Sources.imgsSource);
            return ci;
        }
        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();

            con.RelativeCards = new List<string>() { nameof(DoremyDreamyCirno), nameof(DoremyDreamyDoremy) };
            con.UpgradedRelativeCards = new List<string>() { nameof(DoremyDreamyCirno), nameof(DoremyDreamyDoremy) };

            return con;
        }
    }
    [EntityLogic(typeof(DoremyDreamTeamNextPageDef))]
    public sealed class DoremyDreamTeamNextPage : DCard
    {}
}
