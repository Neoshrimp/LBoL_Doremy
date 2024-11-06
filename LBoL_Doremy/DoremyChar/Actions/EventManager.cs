using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Presentation;
using LBoL_Doremy.Actions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class EventManager
    {
        static ConditionalWeakTable<BattleController, DoremyEvents> cwt_doremyEvents = new ConditionalWeakTable<BattleController, DoremyEvents>();

        public static BattleController Battle
        {
            get
            {
                var rez = GameMaster.Instance?.CurrentGameRun?.Battle;
                if (rez == null)
                    battle_ref.TryGetTarget(out rez);
                return rez;
            }
        }

        static WeakReference<BattleController> battle_ref;


        public static NightmareEvents NMEvents => GetDoremyEvents(Battle).nightmareEvents;

        public static DLEvents DLEvents => GetDoremyEvents(Battle).dLEvents;

        public static DoremyEvents GetDoremyEvents(BattleController battle) 
        {
            if (battle == null)
            {
                throw new InvalidOperationException("Trying to use DLEvents outside of Battle.");
            }
            if (cwt_doremyEvents.TryGetValue(battle, out var doremyEvents))
                return doremyEvents;

            throw new InvalidOperationException("Battle instance was not registered in CWT.");
        }

        [HarmonyPatch(typeof(BattleController), MethodType.Constructor, new Type[] { typeof(GameRunController), typeof(EnemyGroup), typeof(IEnumerable<Card>) })]
        class BattleController_Patch
        {
            static void Prefix(BattleController __instance)
            {
                var battle = __instance;

                battle_ref = new WeakReference<BattleController>(__instance);

                cwt_doremyEvents.Add(battle, new DoremyEvents());

            }
        }


    }


}
