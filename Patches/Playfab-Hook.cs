using System;
using System.Collections.Generic;
using System.Reflection;
using Athrion.Patches;
using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using PlayFab;
using UnityEngine;

namespace Athrion.Patches
{
    [BepInPlugin("com.gorillagames.playfabpatcher", "PlayFab Patccher", "1.0.0")]
    public class playfabpatching : BaseUnityPlugin
    {
        private Harmony hi;

        void Awake()
        {
            hi = new Harmony("com.gorillagames.playfabpatcher");
            Logger.LogInfo("Applying patches...");
            authpatch.app(hi);
            Logger.LogInfo("Patches applied successfully.");
        }

        void OnDestroy()
        {
            Logger.LogInfo("Removing patches...");
            authpatch.patchremove(hi);
            Logger.LogInfo("Patches removed successfully.");
        }
    }

    public static class authpatch
    {
        public static void app(Harmony hi)
        {
            Type target = typeof(PlayFabAuthenticator);

            MethodPatching15(hi, target, "AuthenticateWithPlayFab", nameof(forcetrue), nameof(authwplayfab));
            MethodPatching15(hi, target, "OnLoginWithSteamResponse", nameof(streamrepr), nameof(streamresponseremoved));
            MethodPatching15(hi, target, "OnPlayFabError", nameof(stupidmethod), nameof(pferror));
            MethodPatching15(hi, target, "SetSafety", nameof(safetycaller), nameof(safetysh));
            MethodPatching15(hi, target, "GetPlayFabSessionTicket", nameof(playfabticketsss), nameof(pfs));
        }

        private static void MethodPatching15(Harmony hi, Type target, string methodName, string prefixName, string postfixName)
        {
            MethodInfo original = target.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo prefix = typeof(authpatch).GetMethod(prefixName, BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo postfix = typeof(authpatch).GetMethod(postfixName, BindingFlags.Static | BindingFlags.NonPublic);

            if (original != null)
            {
                hi.Patch(original, prefix != null ? new HarmonyMethod(prefix) : null, postfix != null ? new HarmonyMethod(postfix) : null);
            }
        }

        public static void patchremove(Harmony hi)
        {
            hi?.UnpatchSelf();
        }

        private static bool forcetrue()
        {
            return true;
        }

        private static void authwplayfab()
        {
        }

        private static bool streamrepr()
        {
            return true;
        }

        private static void streamresponseremoved()
        {
        }

        private static bool stupidmethod(ref PlayFabError obj)
        {
            if (obj != null && obj.ErrorMessage.Contains("banned"))
            {
                obj = null;
                return false;
            }
            return true;
        }

        private static void pferror()
        {
        }

        private static bool safetycaller(ref bool isSafety)
        {
            isSafety = false;
            return true;
        }

        private static void safetysh()
        {
        }

        private static bool playfabticketsss(ref string ticket)
        {
            ticket = "discord.gg/athrion" + UnityEngine.Random.Range(9, 457147);
            return false;
        }

        private static void pfs(ref string ticket)
        {
        }

        public static void lmaostupidpatch()
        {
            Type target = typeof(PlayFabAuthenticator);
            FieldInfo sfaccount = target.GetField("isSafeAccount", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo stf = target.GetField("safetyType", BindingFlags.Instance | BindingFlags.NonPublic);

            if (sfaccount != null && stf != null)
            {
                sfaccount.SetValue(null, true);
                stf.SetValue(null, "discord.gg/athrion");
            }
        }

        public static void rewriteshit()
        {
            Type target = typeof(PlayFabAuthenticator);
            MethodInfo[] methods = target.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                if (!method.IsVirtual && !method.IsAbstract)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    foreach (ParameterInfo parameter in parameters)
                    {
                        if (parameter.ParameterType == typeof(string))
                        {
                            method.Invoke(null, new object[] { "discord.gg/athrion" });
                        }
                    }
                }
            }
        }
    }
}
