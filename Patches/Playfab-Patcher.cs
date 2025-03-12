using Athrion.Patches;
using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace Athrion.Patches
{
    [BepInPlugin("com.retard.authpatch", "AUTH PATCHEHRHRHERHERHER4ERHESDRGTYISHRVUERDSHUIGHY87RRTYGHV8IO43UJF8534YH sorry i got excited", "1.0.0")]
    public class hopsitality : BaseUnityPlugin
    {
        private Harmony bye;

        private void Start()
        {
            bye = new Harmony("com.retard.authpatch");
            try
            {
                Log("Applying patches...");
                apliiii();
            }
            catch (Exception e)
            {
                Log($"Error during patching: {e}");
            }
        }

        private void destroying()
        {
            try
            {
                Log("Removing patches...");
                bye?.UnpatchSelf();
            }
            catch (Exception e)
            {
                Log($"Error during unpatching: {e}");
            }
        }

        private void apliiii()
        {
            try
            {
                Type target = Type.GetType("PlayFabAuthenticator", throwOnError: true);
                if (target == null)
                {
                    Log("Target type 'PlayFabAuthenticator' not found.");
                    return;
                }

                foreach (MethodInfo method in target.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.Name.Contains("Ban") || method.Name.Contains("Auth") ||
                        method.Name.Contains("Safety") || method.Name.Contains("Login") ||
                        method.Name.Contains("Ticket") || method.Name.Contains("Session"))
                    {
                        pdiddythemethod(target, method.Name, nameof(prefixhandle), nameof(twentyeightteenvuln));
                    }
                }
            }
            catch (Exception e)
            {
                Log($"Error in apliiii: {e}");
            }
        }

        private void pdiddythemethod(Type target, string methodName, string prefixName, string postfixName)
        {
            try
            {
                MethodInfo original = target.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MethodInfo prefix = typeof(hopsitality).GetMethod(prefixName, BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo postfix = typeof(hopsitality).GetMethod(postfixName, BindingFlags.Static | BindingFlags.NonPublic);

                if (original != null)
                {
                    bye.Patch(
                        original,
                        prefix != null ? new HarmonyMethod(prefix) : null,
                        postfix != null ? new HarmonyMethod(postfix) : null
                    );
                }
                else
                {
                    Log($"Method '{methodName}' not found in {target.Name}.");
                }
            }
            catch (Exception e)
            {
                Log($"Error patching method '{methodName}': {e}");
            }
        }

        private static bool prefixhandle(ref object __instance, ref object[] __args)
        {
            try
            {
                if (__args != null && __args.Length > 0)
                {
                    for (int i = 0; i < __args.Length; i++)
                    {
                        if (__args[i] is string str && str.Contains("ban"))
                        {
                            __args[i] = "discord.gg/athrion";
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[athrion] Error in prefixhandle: {e}");
                return true;
            }
        }

        private static void twentyeightteenvuln(ref object __result)
        {
            try
            {
                if (__result is string str && str.Contains("ban"))
                {
                    __result = "discord.gg/athrion";
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[athrion] Error in twentyeightteenvuln: {e}");
            }
        }

        private void Log(string message)
        {
            Debug.Log($"[athrion] {message}");
        }
    }
}
