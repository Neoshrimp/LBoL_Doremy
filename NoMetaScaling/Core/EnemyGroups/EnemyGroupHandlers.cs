using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.BattleModifiers.Actions;
using LBoLEntitySideloader.CustomHandlers;
using NoMetaScalling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoMetaScaling.Core.EnemyGroups
{
    public static class EnemyGroupHandlers
    {
        public static void RegisterHandlers()
        {
            SummonerInfo.AddSummoner(nameof(Rin), 21);
            SummonerInfo.AddSummoner(nameof(Kokoro), 9);
            SummonerInfo.AddSummoner(nameof(Clownpiece), 3);

            CHandlerManager.RegisterBattleEventHandler(bt => bt.BattleStarted, OnBattleStarted);
        }


        private static void OnBattleStarted(GameEventArgs args)
        {
            var battle = GameMaster.Instance.CurrentGameRun.Battle;

            var summoners = battle.AllAliveEnemies.Where(e => SummonerInfo.summonerInfo.ContainsKey(e.Id));

            foreach(var summoner in summoners)
                battle.React(new ApplySEnoTriggers(typeof(MaxSummonsSE), summoner, SummonerInfo.summonerInfo[summoner.Id].limit), null, ActionCause.None);
        }

    }

    public struct SummonerInfo
    {
        internal static Dictionary<string, SummonerInfo> summonerInfo = new Dictionary<string, SummonerInfo>();

        public static void AddSummoner(string summonerId, int summonLimit) => summonerInfo.AlwaysAdd(summonerId, new SummonerInfo(summonerId, summonLimit));

        public string id;
        public int limit;

        public SummonerInfo(string id, int limit)
        {
            this.id = id;
            this.limit = limit;
        }
    }
}
