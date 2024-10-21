using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Common
{
    public sealed class DoremySoporificSheathDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 0 };


            con.Value1 = 1;

            con.Keywords = Keyword.Forbidden;
            con.UpgradedKeywords = Keyword.Forbidden;


            con.RelativeCards = new List<string>() { nameof(DoremySoporificNeedles) };
            con.UpgradedRelativeCards = new List<string>() { $"{nameof(DoremySoporificNeedles)}+" };


            return con;
        }
    }


    [EntityLogic(typeof(DoremySoporificSheathDef))]
    public sealed class DoremySoporificSheath : DCard
    {


        public string NeedleName { get => "".Join(EnumerateRelativeCards().Select(c => c.Name + (c.IsUpgraded ? "+" : ""))); }

        public override IEnumerable<BattleAction> OnDraw()
        {
            return CreateNeedle();
        }

        public override IEnumerable<BattleAction> OnMove(CardZone srcZone, CardZone dstZone)
        {
            if (dstZone != CardZone.Hand)
            {
                return null;
            }
            return CreateNeedle();
        }


        protected override void OnEnterBattle(BattleController battle)
        {
            if (Zone == CardZone.Hand)
            {
                React(CreateNeedle());
            }
        }

        public override IEnumerable<BattleAction> OnExile(CardZone srcZone)
        {
            return CreateNeedle();
        }

        private IEnumerable<BattleAction> CreateNeedle()
        {
            var needle = Library.CreateCard<DoremySoporificNeedles>();
            needle.GameRun = GameRun;
            if (IsUpgraded)
                needle.Upgrade();


            yield return new AddCardsToHandAction(needle);
        }
    }
}
