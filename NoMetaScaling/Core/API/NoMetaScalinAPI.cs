using LBoLEntitySideloader;
using NoMetaScaling.Core.EnemyGroups;
using NoMetaScalling;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMetaScaling.Core.API
{
    public class NoMetaScalinAPI
    {
        /// <summary>
        /// GUID of the plugin.
        /// </summary>
        public const string GUID = PInfo.GUID;

        /// <summary>
        /// Limits how many summoned units will drop resources (P, Money, points) upon death.
        /// </summary>
        /// <param name="summonerId">Unit which is doing the summoning.</param>
        /// <param name="summonLimit">Number of summons which will drop resources.</param>
        public static void AddOrOverwriteSummoner(string summonerId, int summonLimit) => ExposedStatics.summonerInfo.AlwaysAdd(summonerId, new SummonerInfo(summonerId, summonLimit));


        /// <summary>
        /// If a <i>stackable</i> Status Effect provides meta resources (i.e. Sweet Potato) it will be split into two stacks.
        /// One will track the amount added by legitimate cards, the other the amount added by banned cards.
        /// </summary>
        /// <param name="se_CARD_ID">Id of a CARD which adds the Status Effect in question.</param>
        public static void SelectivelyBanMetaScalingSatusEffect(string se_CARD_ID) => ExposedStatics.splitableSE_CARD_ids.Add(se_CARD_ID);

        /// <summary>
        /// Exempt a card from a ban if it was generated/copied by a legal card. 
        /// Should only be applied to specific token(non-pooled) cards. 
        /// The card is still banned after being played.
        /// </summary>
        /// <param name="cardId">Card Id to exempt.</param>
        public static void ExemptFromBanIfGenerated(string cardId) => ExposedStatics.exemptFromBan.Add(cardId);



        //public static void ExemptFromBanIfGeneratedUnlessCopied(string cardId) => ExposedStatics.dontBanUnlessCopied.Add(cardId);

    }
}
