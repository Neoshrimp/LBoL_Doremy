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
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.Exhibits;

namespace NoMetaScaling.Core.Exhibits
{
    [OverwriteVanilla]
    public sealed class ReimuRedOrbDef : ExhibitTemplate
    {
        public override IdContainer GetId() => nameof(LBoL.EntityLib.Exhibits.Shining.ReimuR);

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
            var con = ExhibitConfig.FromId(nameof(LBoL.EntityLib.Exhibits.Shining.ReimuR)).Copy();
            con.HasCounter = true;
            con.InitialCounter = ReimuR.counterMax;
            return con;
        }

    }

    [EntityLogic(typeof(ReimuRedOrbDef))]
    public sealed class ReimuR : ShiningExhibit
    {
        public const int counterMax = 9;

        public override string Description => string.Join("", new string[] { base.Description, 
            string.Format(NoMoreMetaScalingLocSE.LocalizeProp("OrbAppend", true), counterMax) }) ;

        protected override void OnEnterBattle()
        {
            Counter = counterMax;
            base.ReactBattleEvent<DieEventArgs>(base.Battle.EnemyDied, new EventSequencedReactor<DieEventArgs>(this.OnEnemyDied));
        }

        protected override void OnLeaveBattle()
        {
            Counter = counterMax;
        }

        private IEnumerable<BattleAction> OnEnemyDied(DieEventArgs arg)
        {
            NotifyActivating();
            if (!Battle.BattleShouldEnd)
            {
                yield return new ApplyStatusEffectAction<Firepower>(Owner, new int?(Value1), null, null, null, 0f, true);
            }
            if (Counter > 0)
            {
                Counter--;
                yield return new HealAction(Owner, Owner, Value2, HealType.Normal, 0.2f);
            }
        }
    }

}
