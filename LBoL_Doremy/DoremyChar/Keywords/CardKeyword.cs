using HarmonyLib;
using JetBrains.Annotations;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Keywords
{
    public enum KwDescPos
    {
        DoNotDisplay,
        First,
        Last
    }

    public struct CardKeyword
    {
        public readonly string kwSEid;

        public readonly KwDescPos descPos;

        public readonly bool isVerbose;



        public CardKeyword(string kwSEid, KwDescPos descPos = KwDescPos.Last, bool isVerbose = false)
        {
            this.kwSEid = kwSEid;
            this.descPos = descPos;
            this.isVerbose = isVerbose;
        }


        public override bool Equals(object obj)
        {
            return obj is CardKeyword keyword &&
                   kwSEid == keyword.kwSEid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(kwSEid);
        }
    }
}
