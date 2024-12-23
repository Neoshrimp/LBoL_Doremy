using HarmonyLib;

namespace NoMetaScalling
{
    public static class PInfo
    {
        // each loaded plugin needs to have a unique GUID. usually author+generalCategory+Name is good enough
        public const string GUID = "neo.lbol.modifiers.noMetaScaling";
        public const string Name = "No More Meta scaling";
        public const string version = "1.1.9";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
