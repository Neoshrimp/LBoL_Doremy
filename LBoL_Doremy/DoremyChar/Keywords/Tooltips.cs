using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Keywords
{
    public sealed class DC_SelfNightmareTooltipSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            return con;
        }
    }

    [EntityLogic(typeof(DC_SelfNightmareTooltipSEDef))]
    public sealed class DC_SelfNightmareTooltipSE : DStatusEffect
    {
    }

    public sealed class DC_CreatedTooltipSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            return con;
        }
    }

    [EntityLogic(typeof(DC_CreatedTooltipSEDef))]
    public sealed class DC_CreatedTooltipSE : DStatusEffect
    {
    }


    public sealed class DC_ExileQueueTooltipSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            return con;
        }
    }

    [EntityLogic(typeof(DC_ExileQueueTooltipSEDef))]
    public sealed class DC_ExileQueueTooltipSE : DStatusEffect
    {
        public static string GetSharedLoc(string key, object[] toFormat = null)
        {
            var rez = TypeFactory<StatusEffect>.LocalizeProperty(nameof(DC_ExileQueueTooltipSE), key, true, true);
            if (rez == null)
                return "";
            if (toFormat != null)
                rez = string.Format(rez, toFormat);
            return rez;
        }
    }


    public sealed class DC_VanillaExTooltipSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            return con;
        }
    }

    [EntityLogic(typeof(DC_VanillaExTooltipSEDef))]
    public sealed class DC_VanillaExTooltipSE : DStatusEffect
    {
        public override string Name => StringDecorator.Decorate(base.Name).RuntimeFormat(FormatWrapper);
    }
}
