using LBoL.ConfigData;
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
}
