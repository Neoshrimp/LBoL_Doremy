using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL_Doremy.RootTemplates;
using System.Collections.Generic;

namespace LBoL_Doremy.DoremyChar.Cards.Token
{
    public interface IOptionCard
    {
        public IEnumerable<BattleAction> EffectActions();

        public string BriefDesc { get; }
    }

    public abstract class OptionCardDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Common;

            con.FindInBattle = false;
            con.IsPooled = false;
            con.HideMesuem = true;
            con.IsUpgradable = false;

            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 0 };

            con.Illustrator = null;

            return con;
        }

    }

}
