using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.Presentation;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LBoLEntitySideloader.Attributes;
using LBoL.Core.Units;
using LBoL.Core;

namespace NoMetaScaling.Core.EnemyGroups
{

    public sealed class NoPointSummonSEDef : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(NoPointSummonSE);


        public override LocalizationOption LoadLocalization() => Statics.seBatchLoc.AddEntity(this);

        public override Sprite LoadSprite() => ResourcesHelper.TryGetSprite<StatusEffect>(nameof(MoreFriendsSe));

        public override StatusEffectConfig MakeConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Special;
            con.HasLevel = false;

            return con;
        }
    }

    [EntityLogic(typeof(NoPointSummonSEDef))]
    public sealed class NoPointSummonSE : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            HandleOwnerEvent(Battle.EnemyPointGenerating, OnPointGenerating, GameEventPriority.Lowest);
        }

        private void OnPointGenerating(DieEventArgs args)
        {

            var unit = args.Unit;
            if (unit != Owner)
                return;

            if (unit.HasStatusEffect<Servant>())
            {
                args.Power = 0;
                args.Money = 0;
                args.BluePoint = 0;
            }
        }
    }
}
