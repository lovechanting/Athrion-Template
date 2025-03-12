using System;
using System.Linq;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using GorillaTagScripts;
using Athrion.Libary;
using Athrion;
using Athrion.Library;
using System.Reflection;
using System.Collections.Generic;

namespace Athrion.Library
{
    [BepInPlugin("org.gorillatag.Athrion.projectilelib", "ProjectileLib", "1.0.0")]
    public class ProjectileLib : BaseUnityPlugin
    {
        public static int Waterballoon = -1674517839;
        public static int Snowball = -675036877;
        public static int Heart = 825718363;
        public static int Paintball = -820530352;
        public static int Leafpile = 1705139863;

        private static GameObject HUDObj;
        private static GameObject HUDObj2;
        private static Text projectileText;
        private static int currentProjectileHash = Waterballoon;

        private static float switchDelay = 1.0f;
        private static float lastSwitchTime = 0f;

        public static Color UnpackColor(short data)
        {
            Color result = default;
            result.r = data % 10 / 9f;
            result.g = data / 10 % 10 / 9f;
            result.b = data / 100 % 10 / 9f;
            return result;
        }

        public static GorillaVelocityEstimator jackshit = null;
        public static float projDebounce;
        public static float projDebounceType = 0.1f;

        public static string[] ExternalProjectileNames = new string[]
        {
            "SnowballLeft",
            "WaterBalloonLeft",
            "LavaRockLeft",
            "BucketGiftFunctional",
            "ScienceCandyLeft",
            "FishFoodLeft",
            "TrickTreatFunctionalAnchor",
            "VotingRockAnchor_LEFT",
            "TrickTreatFunctionalAnchor",
            "TrickTreatFunctionalAnchor",
            "AppleLeftAnchor"
        };

        public static string[] InternalProjectileNames = new string[]
        {
            "LMACE. LEFT.",
            "LMAEX. LEFT.",
            "LMAGD. LEFT.",
            "LMAHQ. LEFT.",
            "LMAIE. RIGHT.",
            "LMAIO. LEFT.",
            "LMAMN. LEFT.",
            "LMAMS. LEFT.",
            "LMAMN. LEFT.",
            "LMAMN. LEFT.",
            "LMAMU. LEFT."
        };

        public static string[] InternalProjectileNamesRight = new string[]
        {
            "LMACF. RIGHT.",
            "LMAEY. RIGHT.",
            "LMAGE. RIGHT.",
            "LMAHR. RIGHT.",
            "LMAIF. RIGHT.",
            "LMAIP. RIGHT.",
            "LMAMO. RIGHT.",
            "LMAMT. RIGHT.",
            "LMAMO. RIGHT.",
            "LMAMO. RIGHT.",
            "LMAMV."
        };

        public static void BetaFireProjectile(string projectileName, Vector3 position, Vector3 velocity, Color color, bool nodelay = false, bool isServerSide = false)
        {
            if (jackshit == null)
            {
                GameObject thepointless = new GameObject("Blank GVE");
                jackshit = thepointless.AddComponent<GorillaVelocityEstimator>() as GorillaVelocityEstimator;
            }
            jackshit.enabled = false;

            SnowballThrowable fart = GetProjectile(InternalProjectileNames[Array.IndexOf(ExternalProjectileNames, projectileName)]);
            if (!fart.gameObject.activeSelf)
            {
                fart.SetSnowballActiveLocal(true);
                fart.velocityEstimator = jackshit;
                fart.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                fart.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
            }
            if (Time.time > projDebounce)
            {
                try
                {
                    Vector3 startpos = position;
                    Vector3 charvel = velocity;

                    Vector3 oldVel = GorillaTagger.Instance.GetComponent<Rigidbody>().velocity;

                    Vector3 oldPos = fart.transform.position;
                    fart.randomizeColor = true;
                    fart.transform.position = startpos;

                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = charvel;
                    GorillaTagger.Instance.offlineVRRig.SetThrowableProjectileColor(true, color);
                    MethodInfo lsm = typeof(SnowballThrowable).GetMethod("PerformSnowballThrowAuthority", BindingFlags.NonPublic | BindingFlags.Instance);
                    lsm.Invoke(fart, new object[] { });
                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = oldVel;
                    Utilities.Utils.rpcprot();

                    fart.transform.position = oldPos;
                    fart.randomizeColor = false;

                    if (isServerSide)
                    {
                        fart.gameObject.SetActive(false);
                    }
                }
                catch (Exception e) { UnityEngine.Debug.Log(e.Message); }

                if (projDebounceType > 0f && !nodelay)
                    projDebounce = Time.time + projDebounceType + 0.5f; // Increased cooldown for server-sided projectiles
            }
        }

        public static SnowballThrowable[] snowballs = new SnowballThrowable[] { };
        public static Dictionary<string, SnowballThrowable> snowballDict = null;
        public static SnowballThrowable GetProjectile(string provided)
        {
            if (snowballDict == null)
            {
                snowballDict = new Dictionary<string, SnowballThrowable>();

                snowballs = UnityEngine.Object.FindObjectsOfType<SnowballThrowable>(true);
                foreach (SnowballThrowable lol in snowballs)
                {
                    try
                    {
                        if (GetFullPath(lol.transform.parent).ToLower() == "player objects/local vrrig/local gorilla player/holdables" || GetFullPath(lol.transform.parent).ToLower().Contains("player objects/local vrrig/local gorilla player/riganchor/rig/body/shoulder.l/upper_arm.l/forearm.l/hand.l/palm.01.l/transferrableitemlefthand") || GetFullPath(lol.transform.parent).ToLower().Contains("player objects/local vrrig/local gorilla player/riganchor/rig/body/shoulder.r/upper_arm.r/forearm.r/hand.r/palm.01.r/transferrableitemrighthand"))
                        {
                            UnityEngine.Debug.Log("Projectile " + lol.gameObject.name + " logged");
                            snowballDict.Add(lol.gameObject.name, lol);
                        }
                    }
                    catch { }
                }
                if (snowballDict.Count < 18)
                {
                    UnityEngine.Debug.Log("Projectile dictionary unfinished (" + snowballDict.Count + "/18)");
                    snowballDict = null;
                }
            }
            if (snowballDict != null && snowballDict.ContainsKey(provided))
            {
                return snowballDict[provided];
            }
            else
            {
                UnityEngine.Debug.Log("No key found for " + provided);
                return null;
            }
        }

        public static string GetFullPath(Transform transform)
        {
            string path = "";
            while (transform.parent != null)
            {
                transform = transform.parent;
                if (path == "")
                {
                    path = transform.name;
                }
                else
                {
                    path = transform.name + "/" + path;
                }
            }
            return path;
        }

        public static short PackColor(Color col)
        {
            return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
        }

        public static bool VectorIsInvalid(Vector3 vec3)
        {
            return float.IsNaN(vec3.x) || float.IsNaN(vec3.y) || float.IsNaN(vec3.z) ||
                   float.IsInfinity(vec3.x) || float.IsInfinity(vec3.y) || float.IsInfinity(vec3.z);
        }

        public static Hashtable SendProjectileHashtable(int projectileHash, long position, long velocity, short color, bool spam)
        {
            return new Hashtable
            {
                { "projlib_hash", projectileHash },
                { "projlib_position", position },
                { "projlib_velocity", velocity },
                { "projlib_color", color },
                { "projlib_enable", spam }
            };
        }

        public static bool enableNetworking = false;

        public static void SetEnableNetworking(bool enable)
        {
            enableNetworking = enable;
        }

        private static void ReadProjectileData(Player target, out int hash, out Vector3 position, out Vector3 velocity, out Color color)
        {
            target.CustomProperties.TryGetValue("projlib_enable", out object enableResult);
            if (enableResult == null)
            {
                Debug.LogError(target.NickName + " is not using projectileLib at this time.");
                position = Vector3.zero;
                velocity = Vector3.zero;
                color = Color.white;
                hash = -1;
                return;
            }

            target.CustomProperties.TryGetValue("projlib_hash", out object hashObj);
            target.CustomProperties.TryGetValue("projlib_position", out object posObj);
            target.CustomProperties.TryGetValue("projlib_velocity", out object velObj);
            target.CustomProperties.TryGetValue("projlib_color", out object colorObj);

            if (hashObj == null || posObj == null || velObj == null || colorObj == null)
            {
                Debug.LogError(target.NickName + ": unknown error occurred. Null values");
                position = Vector3.zero;
                velocity = Vector3.zero;
                color = Color.white;
                hash = -1;
                return;
            }

            long packedPosition = (long)posObj;
            long packedVelocity = (long)velObj;
            short packedColor = (short)colorObj;

            hash = (int)hashObj;
            position = VRRig.UnpackWorldPosFromNetwork(packedPosition);
            if (VectorIsInvalid(position))
            {
                Debug.LogError(target.NickName + " tried to send an illegal position");
                position = Vector3.zero;
            }
            velocity = VRRig.UnpackWorldPosFromNetwork(packedVelocity);
            if (VectorIsInvalid(velocity))
            {
                Debug.LogError(target.NickName + " tried to send an illegal velocity");
                velocity = Vector3.zero;
            }
            color = UnpackColor(packedColor);
        }

        private static NetPlayer ToNetPlayer(Player player)
        {
            foreach (var netP in NetworkSystem.Instance.AllNetPlayers)
            {
                if (netP.GetPlayerRef() == player)
                {
                    return netP;
                }
            }
            return null;
        }

        public static void LaunchProjectileCS(Vector3 pos, Vector3 velocity, int hash, Color color, NetPlayer sender, bool isServerSide = false)
        {
            GameObject go = ObjectPools.instance.Instantiate(hash);
            float num = Mathf.Abs(go.transform.lossyScale.x);
            go.transform.localScale = Vector3.one * num;
            go.GetComponent<SlingshotProjectile>().Launch(
                pos,
                velocity,
                sender,
                false,
                false,
                -1,
                1f,
                true,
                color
            );

            if (isServerSide)
            {
                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }

        static float egraswr23r32;
        public static void SetSendProjectile(int projectileHash, Vector3 position, Vector3 velocity, Color projectileColor, bool enable, bool isServerSide = false)
        {
            if (enable)
            {
                LaunchProjectileCS(position, velocity, projectileHash, projectileColor, NetworkSystem.Instance.LocalPlayer, isServerSide);
            }
            if (PhotonNetwork.InRoom && enableNetworking)
            {
                if (Time.time > egraswr23r32)
                {
                    egraswr23r32 = Time.time + 0.025f;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(SendProjectileHashtable(
                        projectileHash,
                        VRRig.PackWorldPosForNetwork(position),
                        VRRig.PackWorldPosForNetwork(velocity),
                        PackColor(projectileColor),
                        enable
                    ));
                }
            }
        }

        private void Awake()
        {
            Logger.LogInfo("ProjectileLib Plugin Loaded!");
            SetupProjectileTextDisplay();
        }

        private void SetupProjectileTextDisplay()
        {
            GameObject mainCamera = GameObject.Find("Main Camera");
            HUDObj = new GameObject();
            HUDObj2 = new GameObject();
            HUDObj2.name = "PROJECTILELIB_HUD_OBJ";
            HUDObj.name = "PROJECTILELIB_HUD_OBJ";
            HUDObj.AddComponent<Canvas>();
            HUDObj.AddComponent<CanvasScaler>();
            HUDObj.AddComponent<GraphicRaycaster>();
            HUDObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            HUDObj.GetComponent<Canvas>().worldCamera = mainCamera.GetComponent<Camera>();
            HUDObj.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 5f);
            HUDObj.GetComponent<RectTransform>().position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
            HUDObj2.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z - 4.6f);
            HUDObj.transform.parent = HUDObj2.transform;
            HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
            Vector3 eulerAngles = HUDObj.GetComponent<RectTransform>().rotation.eulerAngles;
            eulerAngles.y = -270f;
            HUDObj.transform.localScale = new Vector3(1f, 1f, 1f);
            HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(eulerAngles);

            projectileText = new GameObject { transform = { parent = HUDObj.transform } }.AddComponent<Text>();
            projectileText.text = "[ Waterballoon ]";
            projectileText.fontSize = 30;
            projectileText.font = Font.CreateDynamicFontFromOSFont("Roboto", 16);
            projectileText.rectTransform.sizeDelta = new Vector2(450f, 210f);
            projectileText.alignment = TextAnchor.LowerLeft;
            projectileText.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            projectileText.rectTransform.localPosition = new Vector3(-1f, -1f, -0.5f);
            projectileText.material = new Material(Shader.Find("GUI/Text Shader"));
        }

        private void Update()
        {
            ExampleMod();
        }

        private static float rainbowSpeed = 1.0f;
        private static float hue = 0f;

        public static Color rainbowlgbtq()
        {
            hue += rainbowSpeed * Time.deltaTime;
            if (hue > 1f) hue = 0f;
            return Color.HSVToRGB(hue, 1f, 1f);
        }

        public static void ExampleMod()
        {
            Color rainbowColor = rainbowlgbtq();
            if (ControllerInputPoller.instance.rightGrab)
            {
                SetSendProjectile(currentProjectileHash, GorillaTagger.Instance.rightHandTransform.position, GorillaLocomotion.Player.Instance.rightControllerTransform.forward * 150f, rainbowColor, true, false);
                BetaFireProjectile("SnowballLeft", GorillaTagger.Instance.rightHandTransform.position, GorillaLocomotion.Player.Instance.rightControllerTransform.forward * 150f, rainbowColor, false, true);
            }
            else
            {
                SetSendProjectile(-1, Vector3.zero, Vector3.zero, Color.white, false);

                if (ControllerInputPoller.instance.rightControllerSecondaryButton)
                {
                    SwitchProjectile();
                }
            }
        }

        private static void SwitchProjectile()
        {
            if (Time.time < lastSwitchTime + switchDelay)
                return;

            if (currentProjectileHash == Waterballoon)
            {
                currentProjectileHash = Snowball;
            }
            else if (currentProjectileHash == Snowball)
            {
                currentProjectileHash = Heart;
            }
            else if (currentProjectileHash == Heart)
            {
                currentProjectileHash = Paintball;
                projectileText.text = "[ Paintball ]";
            }
            else if (currentProjectileHash == Paintball)
            {
                currentProjectileHash = Leafpile;
                projectileText.text = "[ Leafpile ]";
            }
            else
            {
                currentProjectileHash = Waterballoon;
            }

            lastSwitchTime = Time.time;
        }
    }
}