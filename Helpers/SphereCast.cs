using HarmonyLib;
using UnityEngine;

namespace SphereMod
{
    [HarmonyPatch(typeof(GorillaTagger), "get_sphereCastRadius")]
    public static class SphereCastPatch
    {
        public static bool PatchEnabled { get; set; } = false;
        public static float OverrideRadius { get; set; } = 0.1f;

        private static void Postfix(ref float __result)
        {
            if (PatchEnabled)
            {
                __result = OverrideRadius;
            }
        }
    }
}
