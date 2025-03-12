using System;
using BepInEx;
using HarmonyLib;

namespace Athrion
{
    [BepInPlugin("System.Core", "System.Core", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new Harmony("System.Runtime").PatchAll();
        }
    }
}
