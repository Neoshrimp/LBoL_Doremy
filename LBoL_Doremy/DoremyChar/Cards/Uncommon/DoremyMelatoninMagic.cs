using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.Cards.Character.Alice;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Cards.Common;
using LBoL_Doremy.DoremyChar.Cards.Rare;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.CustomKeywords;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyMelatoninMagicDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.Blue };
            con.Cost = new ManaGroup() { Blue = 2 };
            con.UpgradedCost = new ManaGroup() { Blue = 1, Any = 1 };



            con.Value1 = 3;
            con.UpgradedValue1 = 4;

            con.Mana = new ManaGroup() { Colorless = 1 };
            //con.UpgradedMana = new ManaGroup() { Blue = 1 };



            con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_DLKwSE) };

            return con;
        }
    }


    [EntityLogic(typeof(DoremyMelatoninMagicDef))]
    public sealed class DoremyMelatoninMagic : DCard
    {


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new GainManaAction(Mana);
            var drawManyAction = new DrawManyCardAction(Value1);
            yield return drawManyAction;
            
            foreach (var c in drawManyAction.DrawnCards)
            {
                if (!DoremyComatoseForm.IsPositive(c))
                    continue;
                if (c.HasCustomKeyword(DoremyKw.dreamLayerId))
                    yield return new ApplyDLAction(c);
                else
                    c.AddCustomKeyword(DoremyKw.NewDreamLayer);
            }
        }
    }
}
