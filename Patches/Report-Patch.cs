using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[BepInPlugin("com.athrion.ACBypass", "Anti Cheat Bypass", "1.0.0")]
public class ACBypass : BaseUnityPlugin
{
    private Harmony _harmony;

    private void Awake()
    {
        _harmony = new Harmony("com.athrion.ACBypass");
        _harmony.PatchAll();
        BypassLol();
    }

    private void BypassLol()
    {
        Type NiggerNot = typeof(GorillaNot);
        FieldInfo sendReportField = NiggerNot.GetField("_sendReport", BindingFlags.NonPublic | BindingFlags.Instance);
        sendReportField?.SetValue(GorillaNot.instance, false);

        typeof(Player).GetProperty("UserId")?.SetValue(PhotonNetwork.LocalPlayer, "lemming");
        PhotonNetwork.LocalPlayer.NickName = "lemming";

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.MaxPlayers = 0;
        }

        FieldInfo rpplayers = NiggerNot.GetField("reportedPlayers", BindingFlags.NonPublic | BindingFlags.Instance);
        ((List<string>)rpplayers?.GetValue(GorillaNot.instance))?.Clear();

        FieldInfo checkreportcoroutine = NiggerNot.GetField("checkReportsCoroutine", BindingFlags.NonPublic | BindingFlags.Instance);
        Coroutine checkReportsCoroutine = (Coroutine)checkreportcoroutine?.GetValue(GorillaNot.instance);
        if (checkReportsCoroutine != null)
        {
            GorillaNot.instance.StopCoroutine(checkReportsCoroutine);
        }
    }

    [HarmonyPatch(typeof(PhotonNetwork), "RaiseEvent")]
    public class PatchRaiseEvent
    {
        static bool Prefix(byte eventCode)
        {
            if (eventCode == 8) return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom")]
    public class PreventInvalidRoomClose
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CheckReports", MethodType.Enumerator)]
    public class PreventReportCheck
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "QuitDelay", MethodType.Enumerator)]
    public class PreventDelayBan
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
    public class PreventDelayBan2
    {
        private static bool Prefix()
        {
            return false;
        }
    }
}
