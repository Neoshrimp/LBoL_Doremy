using LBoL.ConfigData;
using LBoL.Core;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Keywords
{
    public static class DoremyKw
    {
        public static CardKeyword DreamLayer = new CardKeyword(nameof(DC_DreamLayerKeywordSE), KwDescPos.First);
    }

    public sealed class DC_DreamLayerKeywordSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DC_DreamLayerKeywordSEDef))]
    public sealed class DC_DreamLayerKeywordSE : DStatusEffect
    {
    }





    public sealed class DC_DLKwSEDef : DStatusEffectDef
    {
        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DC_DLKwSEDef))]
    public sealed class DC_DLKwSE : DStatusEffect
    {
        public override string Name => base.Name.RuntimeFormat(FormatWrapper);
    }
}
