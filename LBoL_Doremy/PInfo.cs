using HarmonyLib;

namespace LBoL_Doremy
{
    public static class PInfo
    {
        public const string GUID = "neoCollab.lbol.char.Doremy";
        public const string Name = "Doremy Player character";
        public const string version = "0.5.52";
        public static readonly Harmony harmony = new Harmony(GUID);


    }
}
