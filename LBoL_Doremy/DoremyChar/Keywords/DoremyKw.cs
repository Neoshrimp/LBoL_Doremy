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
        public static CardKeyword DreamLayer = new CardKeyword(nameof(DoremyDreamLayerKeywordSE), KwDescPos.First);
    }

    public sealed class DoremyDreamLayerKeywordSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.RelativeEffects = new List<string>() { nameof(DoremyDLKwSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DoremyDreamLayerKeywordSEDef))]
    public sealed class DoremyDreamLayerKeywordSE : DStatusEffect
    {
    }



    public sealed class DoremyDLKwSEDef : DStatusEffectDef
    {
        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            return con;
        }
    }

    [EntityLogic(typeof(DoremyDLKwSEDef))]
    public sealed class DoremyDLKwSE : DStatusEffect
    {
        public override string Name => base.Name.RuntimeFormat(FormatWrapper);
    }
}
