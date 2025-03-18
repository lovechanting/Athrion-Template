using HarmonyLib;
using UnityEngine;

public static class TagPatch
{
    public static void EST()
    {
        var harmony = new Harmony("com.athrion.silenttag");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(VRRig), "CheckDistance")]
    public class CheckDistancePatch
    {
        static bool Prefix(ref bool __result)
        {
            __result = true; 
            return false;
        }
    }

    [HarmonyPatch(typeof(VRRig), "CheckTagDistanceRollback")]
    public class CheckTagDistanceRollbackPatch
    {
        static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
