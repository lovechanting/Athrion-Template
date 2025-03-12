using BepInEx;
using GorillaTagScripts;
using HarmonyLib;
using System.Reflection;

namespace ExamplePatcher
{
    [BepInPlugin("com.block.patcher", "Block Patcher", "1.0.0")]
    public class BlockPatcher : BaseUnityPlugin
    {
        private void Awake()
        {
            var harmony = new Harmony("com.block.patcher");

            var originalTransformValidation = AccessTools.Method(typeof(BuilderTable), "ValidatePieceWorldTransform");
            var patchedTransformValidation = AccessTools.Method(typeof(SEXX), nameof(SEXX.ttreueyhe));
            harmony.Patch(originalTransformValidation, new HarmonyMethod(patchedTransformValidation));

            var originalParamsValidation = AccessTools.Method(typeof(BuilderTable), "ValidateCreatePieceParams");
            var patchedParamsValidation = AccessTools.Method(typeof(SEXX), nameof(SEXX.asfewsafwqe));
            harmony.Patch(originalParamsValidation, new HarmonyMethod(patchedParamsValidation));

            var originalBuildAreaValidation = AccessTools.Method(typeof(BuilderTable), "IsLocationWithinSharedBuildArea");
            var patchedBuildAreaValidation = AccessTools.Method(typeof(SEXX), nameof(SEXX.asdfecqwcq));
            harmony.Patch(originalBuildAreaValidation, new HarmonyMethod(patchedBuildAreaValidation));
        }
    }

    public static class SEXX
    {
        public static bool ttreueyhe(ref bool __result)
        {
            __result = true;
            return false;
        }

        public static bool asfewsafwqe(ref bool __result)
        {
            __result = true;
            return false;
        }

        public static bool asdfecqwcq(ref bool __result)
        {
            __result = true;
            return false; 
        }
    }
}
