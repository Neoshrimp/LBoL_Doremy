using BepInEx;
using HarmonyLib;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using NoMetaScaling;
using NoMetaScaling.Core;
using NoMetaScaling.Core.EnemyGroups;
using System.Reflection;
using UnityEngine;


namespace NoMetaScalling
{
    [BepInPlugin(NoMetaScalling.PInfo.GUID, NoMetaScalling.PInfo.Name, NoMetaScalling.PInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        private static readonly Harmony harmony = NoMetaScalling.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;




        private void Awake()
        {
            log = Logger;

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            PConfig.DoBinds(Config);

            EntityManager.RegisterSelf();
            harmony.PatchAll();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();

            CardFilter.RegisterHandlers();
            ActionCancel.RegisterHandlers();
            EnemyGroupHandlers.RegisterHandlers();
        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }


    }
}
