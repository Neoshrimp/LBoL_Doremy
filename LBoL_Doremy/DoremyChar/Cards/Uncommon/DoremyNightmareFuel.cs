using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Enemy;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyNightmareFuelDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White, ManaColor.Blue };
            con.Cost = new ManaGroup() { White = 1, Blue = 1 };


            con.RelativeCards = new List<string>() { nameof(Nightmare), nameof(WManaCard), nameof(UManaCard) };
            con.UpgradedRelativeCards = new List<string>() { nameof(Nightmare), nameof(PManaCard) + '+', nameof(PManaCard) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyNightmareFuelDef))]
    public sealed class DoremyNightmareFuel : DCard
    {

        public Card Ritual1 => EnumerateRelativeCards().First(c => c.CardType != CardType.Status);
        public Card Ritual2 => EnumerateRelativeCards().Last();

        public string Ritual1Name => Ritual1.Name;
        public string Ritual2Name => Ritual2.Name;


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            var status = Library.CreateCard<Nightmare>();
            status.GameRun = GameRun;
            yield return new AddCardsToDrawZoneAction(new Card[] { status }, DrawZoneTarget.Random);

            yield return new AddCardsToHandAction(new Card[] { Ritual1, Ritual2 });

        }

    }
}
