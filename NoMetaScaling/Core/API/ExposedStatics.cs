using LBoL.EntityLib.Cards.Neutral.Green;
using LBoL.EntityLib.Cards.Neutral.MultiColor;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using NoMetaScaling.Core.EnemyGroups;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMetaScaling.Core.API
{
    internal static class ExposedStatics
    {

        internal static Dictionary<string, SummonerInfo> summonerInfo = new Dictionary<string, SummonerInfo>();

        // 2do magic is problematic with mixing real and fake copies but in reality such situations should be rare
        internal static HashSet<string> splitableSE_CARD_ids = new HashSet<string>() { nameof(RangziFanshu), nameof(MeihongPower), /*nameof(BailianMagic)*/ };

        internal static HashSet<string> alwaysBanned = new HashSet<string>() { nameof(JinziDoppelganger) };

        //internal static HashSet<string> dontBanUnlessCopied = new HashSet<string>();

        internal static HashSet<string> exemptFromGenBan = new HashSet<string>();
        internal static HashSet<string> exemptFromPlayBan = new HashSet<string>();




    }
}
