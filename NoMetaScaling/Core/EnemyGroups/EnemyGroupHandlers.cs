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
using NoMetaScaling.Core.API;
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
            NoMetaScalinAPI.AddOrOverwriteSummoner(nameof(Rin), 21);
            NoMetaScalinAPI.AddOrOverwriteSummoner(nameof(Kokoro), 6);
            NoMetaScalinAPI.AddOrOverwriteSummoner(nameof(Clownpiece), 3);


            CHandlerManager.RegisterBattleEventHandler(bt => bt.BattleStarted, OnBattleStarted);
        }


        private static void OnBattleStarted(GameEventArgs args)
        {
            var battle = GameMaster.Instance.CurrentGameRun.Battle;

            var summoners = battle.AllAliveEnemies.Where(e => ExposedStatics.summonerInfo.ContainsKey(e.Id));

            foreach (var summoner in summoners)
            {
                var limit = ExposedStatics.summonerInfo[summoner.Id].limit;
                if(limit >= 0)
                    battle.React(new ApplySEnoTriggers(typeof(MaxSummonsSE), summoner, limit), null, ActionCause.None);
            }
        }

    }

    public struct SummonerInfo
    {
        public string id;
        public int limit;

        public SummonerInfo(string id, int limit)
        {
            this.id = id;
            this.limit = limit;
        }
    }
}
