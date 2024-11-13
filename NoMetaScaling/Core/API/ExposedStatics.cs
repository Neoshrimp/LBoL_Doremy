using LBoL.EntityLib.Cards.Neutral.Green;
using LBoL.EntityLib.Cards.Neutral.Red;
using NoMetaScaling.Core.EnemyGroups;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMetaScaling.Core.API
{
    internal static class ExposedStatics
    {

        internal static Dictionary<string, SummonerInfo> summonerInfo = new Dictionary<string, SummonerInfo>();

        internal static HashSet<string> splitableSE_CARD_ids = new HashSet<string>() { nameof(RangziFanshu), nameof(MeihongPower) };


        //internal static HashSet<string> dontBanUnlessCopied = new HashSet<string>();

        internal static HashSet<string> exemptFromGenBan = new HashSet<string>();
        internal static HashSet<string> exemptFromPlayBan = new HashSet<string>();


    }
}
