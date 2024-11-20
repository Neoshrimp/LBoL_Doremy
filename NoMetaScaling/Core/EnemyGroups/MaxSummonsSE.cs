using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader;
using NoMetaScaling.Core.Loc;
using System;
using System.Collections.Generic;
using System.Text;
using LBoLEntitySideloader.Attributes;
using LBoL.Presentation;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.Core.Units;
using LBoL.Core.Battle;
using LBoL.Core;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using Spine;
using NoMetaScalling;
using LBoL.Core.Battle.BattleActions;

namespace NoMetaScaling.Core.EnemyGroups
{

    public sealed class MaxSummonsSEDef : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(MaxSummonsSE);


        public override LocalizationOption LoadLocalization() => Statics.seBatchLoc.AddEntity(this);

        public override Sprite LoadSprite() => ResourcesHelper.TryGetSprite<StatusEffect>(nameof(MoreFriendsSe));

        public override StatusEffectConfig MakeConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Special;
            con.HasLevel = true;
            con.LevelStackType = LBoL.Base.StackType.Add;

            return con;
        }
    }


    [EntityLogic(typeof(MaxSummonsSEDef))]
    public sealed class MaxSummonsSE : StatusEffect
    {
        //private HashSet<Unit> firstSpawns = new HashSet<Unit>();

        public override string Description => Battle == null || Level == 0 ? LocalizeProperty("NoLevelDesc", true) : base.Description;

        protected override void OnAdded(Unit unit)
        {
            //firstSpawns.Clear();
            ReactOwnerEvent(Battle.EnemySpawned, OnFirstSpawns);
        }


        private IEnumerable<BattleAction> OnFirstSpawns(UnitEventArgs args)
        {
            if (args.ActionSource != Owner)
                yield break;

            if (Level > 0)
            {
                Level = Math.Max(Level - 1, 0);
            }
            else
                yield return new ApplyStatusEffectAction<NoPointSummonSE>(args.Unit);
        }
      



    }

}
