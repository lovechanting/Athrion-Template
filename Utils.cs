using Athrion;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts;
using GorillaTagScripts.ModIO;
using HarmonyLib;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Networking;
using Photon.Voice;
using UnityEngine.UIElements;
using Technie.PhysicsCreator.Skinned;
using UnityEngine.InputSystem;
using Athrion.Utilitites;
using UnityEngine.Assertions.Must;
using Valve.VR;
using UnityEngine.ProBuilder;
using UnityEngine.Splines.Interpolators;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Voice.Unity;
using System.Reflection.Emit;
using UnityEngine.UI;
using GorillaTag.Reactions;
using Fusion;
using System.Data.Common;
using System.Threading;
using static UnityEngine.GraphicsBuffer;
using static Athrion.Utilities.DeafultPatches;
using GorillaTag.Cosmetics;
using static UnityEngine.Terrain;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Fusion.Sockets;
using System.Runtime.Serialization.Json;
using GorillaTag.GuidedRefs;
using Photon.Voice.PUN.UtilityScripts;
using SphereMod;
using Athrion.Libary;

namespace Athrion.Utilities
{
    public class Utils
    {
        #region Fields
        private static Vector3 wpos;
        private static Vector3 wnormal;
        private static Vector3 gpoint;
        private static Vector3 gnormal;
        private static float stickshit = 0.87f;
        private static float pulladjust = 0.45f;
        public static int score = 1;
        public static bool increasing = true;
        private static float movementeh = 0.04f;
        private static bool lasttouch = false;
        private static bool lasttoruchruight = false;
        private static float GTZoneDelay = 0f;
        #endregion
        public static void WallWalk()
        {
            if ((GorillaLocomotion.Player.Instance.wasLeftHandColliding || GorillaLocomotion.Player.Instance.wasRightHandColliding) && ControllerInputPoller.instance.rightGrab)
            {
                FieldInfo fieldInfo = typeof(GorillaLocomotion.Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                RaycastHit ray = (RaycastHit)fieldInfo.GetValue(GorillaLocomotion.Player.Instance);
                wpos = ray.point;
                wnormal = ray.normal;
            }

            if (!ControllerInputPoller.instance.rightGrab)
            {
                wpos = Vector3.zero;
            }

            if (wpos != Vector3.zero)
            {
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(wnormal * -9.81f, ForceMode.Acceleration);
                ZeroGravity();
            }
        }

        internal static void TeleportGun()
        {
            Libary.AthrionGunLibrary.start2guns(() =>
            {
                if (PhotonNetwork.InRoom)
                {
                    Vector3 pointerpos = AthrionGunLibrary.GetPointerPos();

                    if (pointerpos != Vector3.zero)
                    {
                        GorillaLocomotion.Player.Instance.transform.position = pointerpos + new Vector3(0, 0.1f, 0);
                    }
                }
            }, false);
        }

        internal static void ExampleGunLib(/* */)
        {
            Libary.AthrionGunLibrary.start2guns(() =>
            {
                if (Libary.AthrionGunLibrary.LockedRigOrPlayerOrwhatever != null && PhotonNetwork.InRoom)
                {
                    VRRig rig = Libary.AthrionGunLibrary.LockedRigOrPlayerOrwhatever;
                    int targetActorNumber = rig.Creator.ActorNumber;
                    Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber);

                    if (targetPlayer != null && rig.Creator.ActorNumber == targetActorNumber)
                    {
                        // code
                    }
                }
            }, true);
        }

        public static void ZeroGravity()
        {
            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (9.81f / Time.deltaTime)), ForceMode.Acceleration);
        }

        public static void PullMod()
        {
            bool asfdsds = GorillaLocomotion.Player.Instance.IsHandTouching(true);
            bool purebordem = GorillaLocomotion.Player.Instance.IsHandTouching(false);

            if ((!asfdsds && lasttouch) || (!purebordem && lasttoruchruight))
            {
                Vector3 currentVelocity = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity;
                Vector3 enhancedMovement = new Vector3(currentVelocity.x * movementeh, 0f, currentVelocity.z * movementeh);
                GorillaLocomotion.Player.Instance.transform.position += enhancedMovement;
            }

            lasttouch = asfdsds;
            lasttoruchruight = purebordem;
        }

        public static void ClimbAssist()
        {
            var ply = GorillaLocomotion.Player.Instance;
            bool holding = ControllerInputPoller.instance.rightGrab || ControllerInputPoller.instance.leftGrab;

            if ((ply.wasLeftHandColliding || ply.wasRightHandColliding) && holding)
            {
                FieldInfo hitData = typeof(GorillaLocomotion.Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                RaycastHit hit = (RaycastHit)hitData.GetValue(ply);

                gpoint = hit.point;
                gnormal = hit.normal;
            }

            if (!holding)
            {
                gpoint = Vector3.zero;
            }

            if (gpoint != Vector3.zero)
            {
                Rigidbody rb = ply.bodyCollider.attachedRigidbody;
                Vector3 force = gnormal * (-9.81f * pulladjust);
                rb.AddForce(force, ForceMode.Acceleration);

                ajvel(rb);
            }
        }

        private static void ajvel(Rigidbody rb)
        {
            if (rb == null) return;

            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * (1f - Mathf.Clamp01(stickshit)));
        }
        public static void IronManFlight()
        {
            var p = GorillaLocomotion.Player.Instance;
            var rightHand = GorillaTagger.Instance.rightHandTransform;
            var rb = p.bodyCollider.attachedRigidbody;

            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                Vector3 thrust = -rightHand.up * 40f + rightHand.forward * 15f;
                fireparticles(rightHand);
                rb.velocity = Vector3.Lerp(rb.velocity, thrust, 0.25f);
            }
            else
            {
                stopparts(rightHand);
            }
        }

        public static void rpcprot()
        {
            var GorillaNotShit = GorillaNot.instance;
            if (GorillaNotShit == null || PhotonNetwork.LocalPlayer == null) return;

            var LocalVRRIG = RigManager.GetPhotonViewFromVRRig(GorillaTagger.Instance.offlineVRRig);

            GorillaNotShit.rpcErrorMax = int.MaxValue; // This is useless btw i just use it for future cases
            GorillaNotShit.rpcCallLimit = int.MaxValue; // This is useless btw i just use it for future cases
            GorillaNotShit.logErrorMax = int.MaxValue; // This is useless btw i just use it for future cases

            PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue; // This is useless btw i just use it for future cases
            PhotonNetwork.QuickResends = int.MaxValue; // This is useless btw i just use it for future cases
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = int.MaxValue; // This is useless btw i just use it for future cases

            PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(PhotonNetwork.LocalPlayer.ActorNumber);

            while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.QueuedOutgoingCommands > 0)
            {
                PhotonNetwork.SendAllOutgoingCommands();
            }

            PhotonNetwork.OpCleanRpcBuffer(LocalVRRIG);
            PhotonNetwork.OpRemoveCompleteCache();

            PhotonNetwork.NetworkingClient.LoadBalancingPeer.TransportProtocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.LimitOfUnreliableCommands = int.MaxValue;

            GorillaNotShit.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer); // This is useless btw i just use it for future cases

            PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = int.MaxValue;
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.WarningSize = int.MaxValue;
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.TimePingInterval = 1;

            PhotonNetwork.NetworkingClient.LoadBalancingPeer.ByteArraySlicePool.ByteSerialize();
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = false;
        }
        private static void fireparticles(Transform hand /* Whoever made particles needs to suicide I had to spend 30 Minutes learning how particles work. */ )
        {
            var sadsadsa = hand.GetComponentInChildren<ParticleSystem>();
            if (sadsadsa == null)
            {
                var newParticles = new GameObject("IronManThrusters");
                newParticles.transform.parent = hand;
                newParticles.transform.localPosition = Vector3.zero;
                var ps = newParticles.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = new ParticleSystem.MinMaxGradient(Color.red, Color.yellow);
                main.startSpeed = 15f;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.startSize = 0.7f;
                main.startLifetime = 0.6f;
                main.maxParticles = 500;
                main.gravityModifier = -0.5f;
                var emission = ps.emission;
                emission.rateOverTime = 100f;
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 15f;
                shape.radius = 0.2f;
                ps.Play();
            }
            else if (!sadsadsa.isPlaying)
            {
                sadsadsa.Play();
            }
        }
        private static void stopparts(Transform hand)
        {
            var sadsadsa = hand.GetComponentInChildren<ParticleSystem>();
            if (sadsadsa != null && sadsadsa.isPlaying)
            {
                sadsadsa.Stop();
            }
        }
        public static void LeapForward()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                var p = GorillaLocomotion.Player.Instance;
                var t = p.bodyCollider.transform;

                Vector3 leaparea = t.forward * 10f;
                leaparea.y += 5f;

                p.bodyCollider.attachedRigidbody.velocity = leaparea;
            }
        }
        public static void LeapInfront()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                var p = GorillaLocomotion.Player.Instance;
                var t = p.bodyCollider.transform;

                Vector3 leaparea = t.forward * 10f;

                p.bodyCollider.attachedRigidbody.velocity = leaparea;
            }
        }
        public static void ControlledFly()
        {
            var p = GorillaLocomotion.Player.Instance;
            var t = p.bodyCollider.transform;
            var rb = p.bodyCollider.attachedRigidbody;
            var hand = GorillaTagger.Instance.rightHandTransform;

            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                Vector3 flyDirection = (hand.position - t.position).normalized * 10f;
                rb.velocity = flyDirection;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        public static void LeapBehind()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                var p = GorillaLocomotion.Player.Instance;
                var t = p.bodyCollider.transform;

                Vector3 leaparea = -t.forward * 10f;

                p.bodyCollider.attachedRigidbody.velocity = leaparea;
            }
        }

        public static void Name()
        {
            string playerName = new NetworkString<_512>("athrion".PadRight(3) + "on".PadRight(3) + "top").ToString();
            ChangeName(playerName);
        }
        public static void ChangeName(string playerName)
        {
            GorillaComputer.instance.currentName = playerName;
            PhotonNetwork.LocalPlayer.NickName.ToLower();
            PhotonNetwork.LocalPlayer.NickName = playerName;
            GorillaComputer.instance.offlineVRRigNametagText.text = playerName;
            GorillaComputer.instance.savedName = playerName;
            PlayerPrefs.SetString("playerName", playerName);
            PlayerPrefs.Save();

            try
            {
                if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) ||
                    CosmeticWardrobeProximityDetector.IsUserNearWardrobe(PhotonNetwork.LocalPlayer.UserId))
                {
                    GorillaTagger.Instance.myVRRig.SendRPC(
                        "RPC_InitializeNoobMaterial",
                        RpcTarget.All,
                        new object[] {
                    GorillaTagger.Instance.offlineVRRig.playerColor.r,
                    GorillaTagger.Instance.offlineVRRig.playerColor.g,
                    GorillaTagger.Instance.offlineVRRig.playerColor.b
                        }
                    );
                    rpcprot();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in ChangeName: {ex.Message}");
            }
        }
        public static void NetworkString()
        {
            NetworkString<_512> gds = new NetworkString<_512>("999999");

            if (int.TryParse(gds.ToString(), out int score))
            {
                GorillaTagger.Instance.offlineVRRig.SetQuestScore(score);
            }
            else
            {
                Console.WriteLine($"Can't not convert {gds.ToString()}");
            }
        }
        public static void stst()
        {
            float t = Time.time;
            int score = Mathf.RoundToInt(Mathf.Lerp(1, 100, Mathf.PingPong(t, 1)));

            NetworkString<_512> gds = new NetworkString<_512>(score.ToString());

            if (int.TryParse(gds.ToString(), out int parsedScore))
            {
                GorillaTagger.Instance.offlineVRRig.SetQuestScore(parsedScore);
            }
            else
            {
                Debug.Log($"Can't convert {gds.ToString()}");
            }
        }
    
        public static void STA()
        {
            FriendingManager friendingManager = UnityEngine.Object.FindObjectOfType<FriendingManager>();

            if (ControllerInputPoller.instance.rightGrab)
            {
                if (Time.time > GTZoneDelay)
                {
                    GTZoneDelay = Time.time + 0.6f;
                    for (int i = 0; i < 250; i++)
                    {
                        FriendshipGroupDetection.Instance.photonView.RPC("VerifyPartyMember", RpcTarget.Others, new object[1]);
                    }
                }
            }
        }

        public static void playerid()
        {
            var asdfsd = PhotonNetwork.LocalPlayer.UserId.ToString();
            Debug.Log(asdfsd);
        }

        public static void RemoveDistanceCheck()
        {
            typeof(VRRig).GetMethod("CheckDistance").Invoke(null, new object[] { Vector3.zero, 99999f });
            typeof(VRRig).GetMethod("CheckTagDistanceRollback").Invoke(null, new object[] { null, 99999f, 0f });
            TagPatch.EST();
        }
        public static void TagThroughWalls()
        {
            var gorillaTagger = GameObject.FindObjectOfType<GorillaTagger>();
            if (gorillaTagger == null) return;

            var gorillaTagColliderLayerMaskField = typeof(GorillaTagger).GetField(
                "gorillaTagColliderLayerMask",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (gorillaTagColliderLayerMaskField != null)
            {
                gorillaTagColliderLayerMaskField.SetValue(gorillaTagger, LayerMask.GetMask("Default"));
            }
            else
            {
                Debug.LogError("nono work");
            }
        }
        public static bool PlayerIsTagged(VRRig player)
        {
            if (player == null || player.mainSkin?.material == null)
                return false;

            string matname = player.mainSkin.material.name.ToLower();
            return matname.Contains("fected") || matname.Contains("it") ||
                   matname.Contains("stealth") || matname.Contains("ice") ||
                   !player.nameTagAnchor.activeSelf;
        }
        public static void BypassTagCooldown()
        {
            var gorillaTagger = GameObject.FindObjectOfType<GorillaTagger>();
            if (gorillaTagger == null) return;

            gorillaTagger.tagCooldown = 0f;
        }
        public static void SetTagReach(float radius)
        {
            bool isTagged = PlayerIsTagged(GorillaTagger.Instance?.offlineVRRig);
            SphereCastPatch.PatchEnabled = isTagged;
            SphereCastPatch.OverrideRadius = isTagged ? radius : 0.1f;
        }
        public static void TagReach() => SetTagReach(3f);
        public static void TagRL() => SetTagReach(1.3f);
    }
}
