using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.CustomKeywords;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyRunawayDreamDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Ability;
            con.TargetType = TargetType.Self;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 0 };



            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyRunawayDreamDef))]
    public sealed class DoremyRunawayDream : DCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<DoremyRunawayDreamSE>(1);
        }
    }

    public sealed class DoremyRunawayDreamSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Positive;


            return con;
        }
    }

    [EntityLogic(typeof(DoremyRunawayDreamSEDef))]
    public sealed class DoremyRunawayDreamSE : DStatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Battle.CardUsed, OnCardUsed);
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs arg)
        {
            var card = arg.Card;
            if (card.HasCustomKeyword(DoremyKw.dreamLayerId))
                for(int i = 0; i < Level; i++)
                    yield return new ApplyDLAction(card);
        }
    }
}
