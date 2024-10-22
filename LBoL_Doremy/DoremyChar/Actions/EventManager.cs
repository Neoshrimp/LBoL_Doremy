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

        public static BattleController Battle { get => GameMaster.Instance?.CurrentGameRun?.Battle; }


        public static DLEvents DLEvents
        {
            get
            {
                return GetDoremyEvents().dLEvents;
            }
        }


        static DoremyEvents GetDoremyEvents() 
        {
            if (Battle == null)
            {
                throw new InvalidOperationException("Trying to use DLEvents outside of Battle.");
            }
            if (cwt_doremyEvents.TryGetValue(Battle, out var doremyEvents))
                return doremyEvents;

            throw new InvalidOperationException("Battle instance was not registered in CWT.");
        }

        [HarmonyPatch(typeof(BattleController), MethodType.Constructor, new Type[] { typeof(GameRunController), typeof(EnemyGroup), typeof(IEnumerable<Card>) })]
        class BattleController_Patch
        {
            static void Prefix(BattleController __instance)
            {
                var battle = __instance;

                cwt_doremyEvents.Add(battle, new DoremyEvents());

            }
        }


    }


}
