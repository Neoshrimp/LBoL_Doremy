using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using NoMetaScaling.Core.Loc;

namespace NoMetaScaling.Core.Exhibits
{
    [OverwriteVanilla]
    public sealed class BirdFacedUrnDef : ExhibitTemplate
    {
        public override IdContainer GetId() => nameof(LBoL.EntityLib.Exhibits.Common.HuanxiangxiangYuanqi);

        [DontOverwrite]
        public override LocalizationOption LoadLocalization()
        {
            throw new NotImplementedException();
        }

        [DontOverwrite]
        public override ExhibitSprites LoadSprite()
        {
            throw new NotImplementedException();
        }

        public override ExhibitConfig MakeConfig()
        {
            var con = ExhibitConfig.FromId(nameof(LBoL.EntityLib.Exhibits.Common.HuanxiangxiangYuanqi)).Copy();
            con.HasCounter = true;
            con.InitialCounter = HuanxiangxiangYuanqi.counterMax;
            return con;
        }

    }

    [EntityLogic(typeof(BirdFacedUrnDef))]
    public sealed class HuanxiangxiangYuanqi : Exhibit
    {
        public const int counterMax = 7;

        public override string Description => string.Join("", new string[] { base.Description, 
            string.Format(NoMoreMetaScalingLocSE.LocalizeProp("UrnAppend", true), counterMax) }) ;

        protected override void OnEnterBattle()
        {
            Counter = counterMax;
            base.ReactBattleEvent<CardUsingEventArgs>(base.Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
        }

        protected override void OnLeaveBattle()
        {
            Counter = counterMax;
        }


        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (args.Card.CardType == CardType.Ability && Counter > 0)
            {
                NotifyActivating();
                Counter--;
                yield return new HealAction(base.Owner, base.Owner, base.Value1, HealType.Normal, 0.2f);
            }
            yield break;
        }
    }

}
