using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Athrion
{
    public class RigManager : MonoBehaviour
    {
        public static RigManager Instance { get; private set; }

        private static ConfigEntry<float> MaxDistanceForClosestRig;
        private static ConfigEntry<bool> IncludeSelfInRandomSelection;

        private List<VRRig> _cachedRigs;
        private DateTime _lastCacheUpdate;

        public event Action<VRRig> OnClosestRigFound;
        public event Action<Photon.Realtime.Player> OnRandomPlayerSelected;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            MaxDistanceForClosestRig = Config.Bind("General", "MaxDistanceForClosestRig", 10.0f, "");
            IncludeSelfInRandomSelection = Config.Bind("General", "IncludeSelfInRandomSelection", true, "");
        }

        public VRRig GetVRRigFromPlayer(NetPlayer player)
        {
            if (player == null)
            {
                Logger.LogError("Player is null.");
                return null;
            }
            return GorillaGameManager.instance?.FindPlayerVRRig(player);
        }

        public VRRig GetRandomVRRig(bool includeSelf = false)
        {
            var players = includeSelf ? PhotonNetwork.PlayerList : PhotonNetwork.PlayerListOthers;
            if (players == null || players.Length == 0)
            {
                Logger.LogError("No players available.");
                return null;
            }
            var randomPlayer = players[UnityEngine.Random.Range(0, players.Length)];
            OnRandomPlayerSelected?.Invoke(randomPlayer);
            return GetVRRigFromPlayer(randomPlayer);
        }

        public VRRig GetClosestVRRig()
        {
            if (GorillaParent.instance?.vrrigs == null || GorillaTagger.Instance?.bodyCollider == null)
            {
                Logger.LogError("Required components are not initialized.");
                return null;
            }

            float closestDistance = float.MaxValue;
            VRRig closestRig = null;

            foreach (var vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == null || vrrig == GorillaTagger.Instance.myVRRig)
                    continue;

                float currentDistance = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position);

                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestRig = vrrig;
                }
            }

            if (closestDistance <= MaxDistanceForClosestRig.Value)
            {
                OnClosestRigFound?.Invoke(closestRig);
                return closestRig;
            }

            Logger.LogWarning("No rig found within the maximum distance.");
            return null;
        }

        public PhotonView GetPhotonViewFromVRRig(VRRig vrRig)
        {
            if (vrRig == null)
            {
                Logger.LogError("VRRig is null.");
                return null;
            }
            return Traverse.Create(vrRig).Field<PhotonView>("photonView").Value;
        }

        public NetworkView GetNetworkViewFromVRRig(VRRig vrRig)
        {
            if (vrRig == null)
            {
                Logger.LogError("VRRig is null.");
                return null;
            }
            return Traverse.Create(vrRig).Field<NetworkView>("netView").Value;
        }

        public Photon.Realtime.Player GetRandomPlayer(bool includeSelf = false)
        {
            var players = includeSelf ? PhotonNetwork.PlayerList : PhotonNetwork.PlayerListOthers;
            if (players == null || players.Length == 0)
            {
                Logger.LogError("No players available.");
                return null;
            }
            var randomPlayer = players[UnityEngine.Random.Range(0, players.Length)];
            OnRandomPlayerSelected?.Invoke(randomPlayer);
            return randomPlayer;
        }

        public Player NetPlayerToPlayer(NetPlayer netPlayer)
        {
            if (netPlayer == null)
            {
                Logger.LogError("NetPlayer is null.");
                return null;
            }
            return netPlayer.GetPlayerRef();
        }

        public NetPlayer GetPlayerFromVRRig(VRRig vrRig)
        {
            if (vrRig == null)
            {
                Logger.LogError("VRRig is null.");
                return null;
            }
            return vrRig.Creator;
        }

        public NetPlayer GetPlayerFromID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID is null or empty.");
                return null;
            }
            foreach (var target in PhotonNetwork.PlayerList)
            {
                if (target.UserId == id)
                {
                    return target;
                }
            }
            Logger.LogWarning($"Player with ID {id} not found.");
            return null;
        }

        public async Task UpdateCachedRigsAsync()
        {
            if (DateTime.Now - _lastCacheUpdate < TimeSpan.FromSeconds(10))
            {
                Logger.LogInfo("Cache is still fresh.");
                return;
            }
            _cachedRigs = new List<VRRig>(GorillaParent.instance.vrrigs);
            _lastCacheUpdate = DateTime.Now;
            Logger.LogInfo("VRRig cache updated.");
        }
    }

    public static class RigManagerExtensions
    {
        public static Player ToPlayer(this NetPlayer netPlayer)
        {
            return RigManager.Instance.NetPlayerToPlayer(netPlayer);
        }
    }
}
