using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.RootTemplates
{
    public abstract class DStatusEffectDef : StatusEffectTemplate
    {
        public override IdContainer GetId() => this.SelfId();

        public override LocalizationOption LoadLocalization() => Loc.StatusEffectsBatchLoc.AddEntity(this);

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite(GetId()+".png", Sources.imgsSource);
        }

        public override StatusEffectConfig MakeConfig()
        {
            var con = PreConfig();
            return con;

        }

        public new StatusEffectConfig DefaultConfig()
        {
            var con = new StatusEffectConfig(
                Index: 0,
                Id: "",
                Order: 10,

                Type: StatusEffectType.Positive,

                IsVerbose: false,

                IsStackable: true,
                StackActionTriggerLevel: null,
                HasLevel: true,
                LevelStackType: StackType.Add,

                HasDuration: false,
                DurationStackType: StackType.Add,
                DurationDecreaseTiming: DurationDecreaseTiming.Custom,

                HasCount: false,
                CountStackType: StackType.Keep,
                LimitStackType: StackType.Keep,
                ShowPlusByLimit: false,

                Keywords: Keyword.None,
                RelativeEffects: new List<string>(),
                VFX: "Default",
                VFXloop: "Default",
                SFX: "Default");

            return con;
        }

        public abstract StatusEffectConfig PreConfig();
    }

    
    public abstract class DStatusEffect : StatusEffect
    { }
}
