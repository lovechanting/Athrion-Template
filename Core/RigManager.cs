using BepInEx;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using UnityEngine;

namespace Athrion
{
    public class RigManager
    {
        public static VRRig GetVRRigFromPlayer(NetPlayer p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }

        public static VRRig GetRandomVRRig(bool includeSelf)
        {
            Photon.Realtime.Player randomPlayer;
            if (includeSelf)
            {
                randomPlayer = PhotonNetwork.PlayerList[UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length - 1)];
            }
            else
            {
                randomPlayer = PhotonNetwork.PlayerListOthers[UnityEngine.Random.Range(0, PhotonNetwork.PlayerListOthers.Length - 1)];
            }
            return GetVRRigFromPlayer(randomPlayer);
        }

        public static VRRig GetClosestVRRig()
        {
            float closestDistance = float.MaxValue;
            VRRig closestRig = null;

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
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

            if (closestRig == null || closestDistance == float.MaxValue)
                return null;

            if (closestDistance > 10.0f) 
                return null;

            return closestRig;
        }

        public static PhotonView GetPhotonViewFromVRRig(VRRig p)
        {
            return (PhotonView)Traverse.Create(p).Field("photonView").GetValue();
        }

        public static NetworkView GetNetworkViewFromVRRig(VRRig p)
        {
            return (NetworkView)Traverse.Create(p).Field("netView").GetValue();
        }

        public static Photon.Realtime.Player GetRandomPlayer(bool includeSelf)
        {
            if (includeSelf)
            {
                return PhotonNetwork.PlayerList[UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length - 1)];
            }
            else
            {
                return PhotonNetwork.PlayerListOthers[UnityEngine.Random.Range(0, PhotonNetwork.PlayerListOthers.Length - 1)];
            }
        }

        public static Player NetPlayerToPlayer(NetPlayer p)
        {
            return p.GetPlayerRef();
        }

        public static NetPlayer GetPlayerFromVRRig(VRRig p)
        {
            //return GetPhotonViewFromVRRig(p).Owner;
            return p.Creator;
        }

        public static NetPlayer GetPlayerFromID(string id)
        {
            NetPlayer found = null;
            foreach (Photon.Realtime.Player target in PhotonNetwork.PlayerList)
            {
                if (target.UserId == id)
                {
                    found = target;
                    break;
                }
            }
            return found;
        }
    }
}