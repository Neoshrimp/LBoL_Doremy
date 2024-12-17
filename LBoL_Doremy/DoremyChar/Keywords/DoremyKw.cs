﻿using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.CustomKeywords;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.Keywords
{
    public static class DoremyKw
    {
        public const string dreamLayerId = nameof(DC_DreamLayerKeywordSE);
        public static CardKeyword NewDreamLayer { get => new CardKeyword(dreamLayerId) { descPos = KwDescPos.First }; }


        public const string dLId = nameof(DC_DLKwSE);
        public static DLKeyword NewDLKeyword { get => new DLKeyword(); }
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

        public const int maxDL = 10;

        public int DreamLevel
        {
            get => _dreamLevel;
            internal set
            {
                _dreamLevel = value; 
                _dreamLevel = Math.Clamp(_dreamLevel, 0, maxDL);
                
            }
        }


        public DLKeyword(string kwSEid = DoremyKw.dLId, bool isVerbose = false) : base(kwSEid, isVerbose)
        {
            descPos = KwDescPos.DoNotDisplay;
        }

        [return: MaybeNull]
        public override CardKeyword Clone(CloningMethod cloningMethod)
        {
            switch (cloningMethod)
            {
                case CloningMethod.TwiceToken:
                case CloningMethod.Copy:
                case CloningMethod.DoesntMatter:
                    var clone = DoremyKw.NewDLKeyword;
                    clone.DreamLevel = DreamLevel;
                    return clone;
                case CloningMethod.NonBattle:
                default:
                    return null;
            }
        }

        public override void Merge(CardKeyword other)
        {
            if (other is DLKeyword otherDL)
            {
                if (otherDL.DreamLevel > DreamLevel )
                    DreamLevel = otherDL.DreamLevel;
            }
        }


    }


    public sealed class DC_DLKwSEDef : DStatusEffectDef
    {
        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.RelativeEffects = new List<string>() { nameof(DC_SelfNightmareTooltipSE) };
            return con;
        }
    }

    [EntityLogic(typeof(DC_DLKwSEDef))]
    public sealed class DC_DLKwSE : DStatusEffect/*, IOnTooltipDisplay, IOverrideSEBrief*/
    {
        public string DLMulDesc => (DoremyEvents.defaultDLMult * 100).ToString();

        public override string Name => base.Name.RuntimeFormat(FormatWrapper);

        int dlLevel = 0;

        public void OnTooltipDisplay(Card card)
        {
            if (card.TryGetCustomKeyword(DoremyKw.dLId, out DLKeyword dl))
            {
                dlLevel = dl.DreamLevel;
            }
        }


    }


}
