using BepInEx;
using BepInEx.Logging;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using PlayFab.Internal;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;
using Athrion.Core;
using GorillaTagScripts;

namespace Athrion.Utilities
{
    [BepInPlugin("com.Athrion.utilities", "Athrion Utilities", "1.0.0")]
    public class AthrionPlugin : BaseUnityPlugin
    {
        private static ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("Athrion Utilities plugin is loading...");

            var harmony = new Harmony("com.Athrion.utilities");
            harmony.PatchAll();

            Logger.LogInfo("Athrion Utilities plugin successfully loaded.");
        }
    }

    public class DeafultPatches
    {
        [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate")]
        public class OnAwake
        {
            static GameObject menuObject;

            static void Postfix()
            {
                if (menuObject == null)
                {
                    menuObject = new GameObject("AthrionOS");
                    menuObject.AddComponent<Menu>();
                    GameObject.DontDestroyOnLoad(menuObject);
                    Debug.Log("Successfully Initialized Athrion OS!"); 
                }
            }
        }

        [HarmonyPatch(typeof(GorillaNot), "SendReport")]
        public class NoSendReport : MonoBehaviour
        {
            static bool Prefix(string susReason, string susId, string susNick)
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
        class NoGracePeriod
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "LateUpdate")]
        class NoGracePeriod2
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        public static bool enabled = false;

        [HarmonyPatch(typeof(BuilderPiece), "CanPlayerGrabPiece")]
        public class UnlimitPatch1
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiece), "CanPlayerAttachPieceToPiece")]
        public class UnlimitPatch2
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiecePrivatePlot), "CanPlayerAttachToPlot")]
        public class UnlimitPatch3
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiecePrivatePlot), "CanPlayerAttachToPlot")]
        public class UnlimitPatch4
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiecePrivatePlot), "CanPlayerGrabFromPlot")]
        public class UnlimitPatch5
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateAttachPieceParams")]
        public class UnlimitPatch6
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateCreatePieceParams")]
        public class UnlimitPatch7
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateDropPieceParams")]
        public class UnlimitPatch8
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateDropPieceState")]
        public class UnlimitPatch9
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateFunctionalPieceState")]
        public class UnlimitPatch10
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateGrabPieceParams")]
        public class UnlimitPatch11
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateGrabPieceState")]
        public class UnlimitPatch12
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateGrabPieceState")]
        public class UnlimitPatch13
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePieceWorldTransform")]
        public class UnlimitPatch14
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePieceWorldTransform")]
        public class UnlimitPatch15
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePlacePieceParams")]
        public class UnlimitPatch16
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePlacePieceState")]
        public class UnlimitPatch17
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateRepelPiece")]
        public class UnlimitPatch18
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
        class NoGracePeriod3
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "LateUpdate")]
        class NoGracePeriod4
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaQuitBox), "OnBoxTriggered")]
        internal class GorillaQuitBoxPatcher
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GameObject), "CreatePrimitive")]
        internal class GameObjectPatch
        {
            private static void Postfix(GameObject __result)
            {
                Material renderer = __result.GetComponent<Renderer>().material;
                renderer.shader = Shader.Find("GorillaTag/UberShader");
                renderer.color = Color.white;
            }
        }

        [HarmonyPatch(typeof(Gorillanalytics), "Start")]
        public class GorillanalyticsStart : MonoBehaviour
        {
            static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(VRRig), "OnDisable")]
        public class OnDisable : MonoBehaviour
        {
            public static bool Prefix(VRRig __instance)
            {
                return !(__instance == GorillaTagger.Instance.offlineVRRig);
            }
        }

        [HarmonyPatch(typeof(LegalAgreements), "Awake")]
        public class LegalAgreementsPatch
        {
            static bool Prefix()
            {
                return false;
            }
        }
    }
}
