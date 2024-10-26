using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL_Doremy.DoremyChar.CreatedCardTracking;
using LBoL_Doremy.ExtraAssets;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System.Reflection;
using UnityEngine;


namespace LBoL_Doremy
{
    [BepInPlugin(LBoL_Doremy.PInfo.GUID, LBoL_Doremy.PInfo.Name, LBoL_Doremy.PInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        private static readonly Harmony harmony = LBoL_Doremy.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;




        private void Awake()
        {
            log = Logger;

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            EntityManager.RegisterSelf();

            harmony.PatchAll();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();

            CardIndexGenerator.PromiseClearIndexSet();
            BattleHandlers.RegisterAll();
            ExtraAssetManager.RegisterLoad();
        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }

        KeyboardShortcut debugBind = new KeyboardShortcut(KeyCode.Y, new KeyCode[] { KeyCode.LeftShift });

        private void Update()
        {
            if (debugBind.IsDown())
            {
                try
                {
                    log.LogDebug(BattleHandlers.CreatedCount);
                }
                catch (System.Exception)
                {
                    log.LogWarning("deenuts");
                }
            }
        }
    }
}
