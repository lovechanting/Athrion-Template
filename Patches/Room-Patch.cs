using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;

[BepInPlugin("com.athrionroomconfuser.mod", "athrionroomconfuser", "1.0.0")]
public class MaxPlayerBypass : BaseUnityPlugin
{
    private void Awake()
    {
        var harmony = new Harmony("com.athrionroomconfuser.mod");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(NetworkSystem), "ConnectToRoom")]
public class ConnectToRoomPatch
{
    static void Prefix(ref string roomName, ref RoomConfig roomConfig)
    {
        if (roomConfig == null)
            return;

        RoomOptions roomOptions = roomConfig.ToPUNOpts();
        roomOptions.PublishUserId = true;

        FieldInfo maxplayersf = typeof(RoomConfig).GetField("MaxPlayers", BindingFlags.NonPublic | BindingFlags.Instance);
        if (maxplayersf != null)
            maxplayersf.SetValue(roomConfig, (byte)207);

        roomConfig.SetFriendIDs(new List<string> { "discord.gg/athrion", "lemmingsucks" });

        UnityEngine.Debug.Log("worked.");
    }
}
