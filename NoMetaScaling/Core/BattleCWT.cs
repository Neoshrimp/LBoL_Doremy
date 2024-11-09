using HarmonyLib;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Core;
using LBoL.Presentation;
using NoMetaScaling.Core.Trackers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NoMetaScaling.Core
{
    public static class BattleCWT
    {
        internal static int startingId = 1000;

        static ConditionalWeakTable<BattleController, BattleData> cwt_battleData = new ConditionalWeakTable<BattleController, BattleData>();

        static WeakReference<BattleController> battle_ref;

        public static BattleController Battle
        {
            get
            {
                var rez = GameMaster.Instance.CurrentGameRun?.Battle;
                if (rez == null)
                    battle_ref.TryGetTarget(out rez);
                return rez;
            }
        }

        public static BattleData GetBattleData(BattleController battle) => cwt_battleData.GetOrCreateValue(battle);

        public static BanData GetBanData(BattleController battle) => GetBattleData(battle).banData;

        public static CopyHistory GetCopyHistory(BattleController battle) => GetBattleData(battle).copyHistory;

        public static CopyHistory CopyHistory => GetBattleData(Battle).copyHistory;



        [HarmonyPatch(typeof(BattleController), MethodType.Constructor, new Type[] { typeof(GameRunController), typeof(EnemyGroup), typeof(IEnumerable<Card>) })]
        [HarmonyPriority(HarmonyLib.Priority.Last)]
        class BattleController_Patch
        {

            static void Prefix(BattleController __instance)
            {
                var battle = __instance;
                battle_ref = new WeakReference<BattleController>(battle);
                cwt_battleData.Add(battle, new BattleData());

            }
            static void Postfix(BattleController __instance)
            {
                startingId = __instance._cardInstanceId;
            }
        }
    }

    public class BattleData
    {
        public BanData banData = new BanData();
        public CopyHistory copyHistory = new CopyHistory();
    }


}
