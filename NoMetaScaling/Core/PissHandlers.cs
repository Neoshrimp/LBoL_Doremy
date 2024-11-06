using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.CustomHandlers;
using NoMetaScalling;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMetaScaling.Core
{
    public static class PissHandlers
    {
        public static void RegisterHandlers()
        {
            CHandlerManager.RegisterBattleEventHandler(bt => bt.EnemySpawned, OnFirst3Spawn, FilterGroupAndReset);

        }

        private static void OnFirst3Spawn(UnitEventArgs args)
        {
            if (spawnCount++ < 3)
                firstSpawns.Add(args.Unit);
        }


        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Die))]
        class BattleController_Patch
        {
            static void Prefix(BattleController __instance, Unit unit, ref int power)
            {
                var battle = __instance;
                if (battle.EnemyGroup.Id != "Clownpiece")
                    return;

                if (unit.HasStatusEffect<Servant>())
                    if (firstSpawns.Contains(unit))
                        firstSpawns.Remove(unit);
                    else
                        power = 0;
            }
        }


        static HashSet<Unit> firstSpawns = new HashSet<Unit>();
        static int spawnCount = 0;

        private static bool FilterGroupAndReset(BattleController battle)
        {
            if (battle.EnemyGroup.Id == "Clownpiece")
            {
                spawnCount = 0;
                firstSpawns.Clear();
                return true;
            }
            return false;
        }
    }
}
