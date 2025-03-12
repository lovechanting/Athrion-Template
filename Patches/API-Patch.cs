using Athrion.Patches;
using BepInEx;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace Athrion.Patches
{
    [BepInPlugin("com.sdfdsfds.fdsdshgdsgsdwgs", "Cheat Updatec Disabler", "1.0.0")]
    public class CheatUpdatePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony("com.sdfdsfds.fdsdshgdsgsdwgs");
            harmony.PatchAll();
            Debug.Log("Cheat Update Disabler Plugin Loaded");
        }
    }

    [HarmonyPatch]
    public class asd
    {
        [HarmonyPatch(typeof(CheatUpdate), "Start")]
        [HarmonyPrefix]
        private static bool Start_Prefix()
        {
            Debug.Log("Patched Start - CheatUpdate is now disabled.");
            return false;
        }

        [HarmonyPatch(typeof(CheatUpdate), "UpdateNumberOfPlayers")]
        [HarmonyPrefix]
        private static bool UpdateNumberOfPlayers_Prefix(ref IEnumerator __result)
        {
            Debug.Log("Patched UpdateNumberOfPlayers - CheatUpdate is now disabled.");
            __result = null;
            return false;
        }

        [HarmonyPatch(typeof(CheatUpdate), "UpdatePlayerCount")]
        [HarmonyPrefix]
        private static bool UpdatePlayerCount_Prefix(ref IEnumerator __result)
        {
            Debug.Log("Patched UpdatePlayerCount - CheatUpdate is now disabled.");
            __result = null;
            return false;
        }
    }
}
