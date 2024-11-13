using HarmonyLib;
using LBoL.Core;
using LBoL.Presentation;
using LBoLEntitySideloader.PersistentValues;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace NoMetaScaling.Core.API
{
    internal class GrCWT
    {

        static ConditionalWeakTable<GameRunController, GrState> cwt_grState = new ConditionalWeakTable<GameRunController, GrState>();

        static WeakReference<GameRunController> gr_ref;

        [MaybeNull]
        public static GameRunController GR
        {
            get
            {
                var rez = GameMaster.Instance?.CurrentGameRun;
                if (rez == null)
                    gr_ref.TryGetTarget(out rez);
                return rez;
            }
        }

        internal static GrState GetGrState(GameRunController gr) => cwt_grState.GetOrCreateValue(gr);



        [HarmonyPatch(typeof(GameRunController), MethodType.Constructor)]
        [HarmonyPriority(Priority.High)]
        class GameRunController_Patch
        {
            static void Prefix(GameRunController __instance)
            {
                GetGrState(__instance);
                gr_ref = new WeakReference<GameRunController>(__instance);
            }
        }


    }




    public class GrState
    {
        public bool cancelEnnabled = true;
    }

    public class GrStateContainer : CustomGameRunSaveData
    {
        public override void Restore(GameRunController gameRun)
        {
            GrCWT.GetGrState(gameRun).cancelEnnabled = cancelEnnabled;
        }

        public override void Save(GameRunController gameRun)
        {
            this.cancelEnnabled = GrCWT.GetGrState(gameRun).cancelEnnabled;
        }

        public bool cancelEnnabled;

    }
}
