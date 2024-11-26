using LBoL.ConfigData;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Keywords
{
    public static class DoremyKw
    {
        public static string dreamLayerId = nameof(DC_DreamLayerKeywordSE);
        public static CardKeyword NewDreamLayer { get => new CardKeyword(dreamLayerId, KwDescPos.First); }


        public static string dLId = nameof(DC_DLKwSE);
        public static DLKeyword NewDLKeyword { get => new DLKeyword(dLId);}
    }

    public sealed class DC_DreamLayerKeywordSEDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            //con.RelativeEffects = new List<string>() { nameof(DC_DLKwSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DC_DreamLayerKeywordSEDef))]
    public sealed class DC_DreamLayerKeywordSE : DStatusEffect
    {
    }




    public class DLKeyword : CardKeyword
    {

        int _dreamLevel = 0;
        public int DreamLevel
        {
            get => _dreamLevel;
            internal set
            {
                _dreamLevel = value; _dreamLevel = Math.Max(0, _dreamLevel);
            }
        }
        public DLKeyword(string kwSEid = nameof(DC_DLKwSE), KwDescPos descPos = KwDescPos.DoNotDisplay, bool isVerbose = false) : base(kwSEid, descPos, isVerbose)
        {
        }

        [return: MaybeNull]
        public override CardKeyword Clone()
        {
            var clone = new DLKeyword();
            clone.DreamLevel = DreamLevel;
            return clone;
        }
    }


    public sealed class DC_DLKwSEDef : DStatusEffectDef
    {
        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            //con.RelativeEffects = new List<string>() { nameof(DC_DreamLayerKeywordSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DC_DLKwSEDef))]
    public sealed class DC_DLKwSE : DStatusEffect
    {
        public string DLMulDesc => (DoremyEvents.defaultDLMult * 100).ToString();

        public override string Name => base.Name.RuntimeFormat(FormatWrapper);
    }
}
