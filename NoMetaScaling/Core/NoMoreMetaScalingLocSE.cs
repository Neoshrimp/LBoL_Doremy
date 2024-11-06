
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NoMetaScaling.Core
{
    public sealed class NoMoreMetaScalingLocSEDef : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(NoMoreMetaScalingLocSE);


        public override LocalizationOption LoadLocalization() => Statics.seBatchLoc.AddEntity(this);

        public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig()
        {
            var con = DefaultConfig();
            return con;
        }
    }

    [EntityLogic(typeof(NoMoreMetaScalingLocSEDef))]
    public sealed class NoMoreMetaScalingLocSE : StatusEffect
    {
        static NoMoreMetaScalingLocSE  _instance;
        public static NoMoreMetaScalingLocSE Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Library.CreateStatusEffect<NoMoreMetaScalingLocSE>();
                return _instance;
            }
        }

        public static string LocalizeProp(string key, bool decorate = false, bool required = true) => Instance.LocalizeProperty(key, decorate, required);
    }
}
