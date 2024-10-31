using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.Presentation;
using LBoL_Doremy.DoremyChar.Cards.Token;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Ults
{
    public sealed class DoremyCavalierWUltDef : DUltimateSkillDef
    {
        public override UltimateSkillConfig MakeConfig()
        {
            return new UltimateSkillConfig(
                "",
                10,
                PowerCost: 120,
                PowerPerLevel: 120,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.OncePerTurn,
                Damage: 0,
                Value1: 1,
                Value2: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );
        }
    }

    [EntityLogic(typeof(DoremyCavalierWUltDef))]
    public sealed class DoremyCavalierWUlt : UltimateSkill
    {
        public DoremyCavalierWUlt()
        {
            TargetType = TargetType.Nobody;
        }

        public override void Initialize()
        {
            Log.LogDebug("Dream ult init");
            base.Initialize();

            // too
            if (Battle != null && Owner != null) 
            {
                Log.LogDebug("Subbing");
                Owner.ReactBattleEvent(Battle.UsUsing, OnSelfUsing);
            }
        }

        private IEnumerable<BattleAction> OnSelfUsing(UsUsingEventArgs arg)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {

/*            var options = EnumerateRelativeCards().ToList();
            var i = GameRun.BattleCardRng.NextInt(0, options.Count - 1);

            options.RemoveAt(i);

            var selection = new SelectCardInteraction(1, Value1, options) { CanCancel = false };*/
            yield break;
        }
    }

    public sealed class DC_ManaOptionDef : OptionCardDef
    {
        public override CardImages LoadCardImages()
        {
            var ci = new CardImages(Sources.imgsSource, (UnityEngine.Texture2D)ResourcesHelper.TryGetCardImage(nameof(CManaCard)));
            return ci;
        }

        public override CardConfig PreConfig()
        {
            var con = base.PreConfig();
            con.Rarity = Rarity.Rare;
            return con;
        }
    }


    [EntityLogic(typeof(DC_ManaOptionDef))]
    public sealed class DC_ManaOption : Card
    {
        public ManaColor ManaColor { get; set; }

        public ManaGroup C2Mana
        {
            get
            {
                var mg = new ManaGroup();
                mg.SetValue(ManaColor, 1);
                return mg;
            }
        }
    }
}
