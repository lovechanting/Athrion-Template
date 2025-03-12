using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
using Athrion;
using Athrion.Utilities;
using Fusion;
using System.Collections;
using UnityEngine.UIElements;
using Athrion.Library;

namespace Athrion.Core
{
    public class Menu : MonoBehaviour
    {
        public class Config
        {
            public string MenuName { get; set; } = "Athrion";
            public Color MenuTitleColor { get; set; } = Color.white; // Change to Color32 Simply if needed! Example: new Color32(212, 185, 150, 255);
            public Color ButtonEnabledColor { get; set; } = new Color32(212, 185, 150, 255);
            public Color ButtonDisabledColor { get; set; } = new Color32(160, 120, 85, 255);
            public Color OutlineEffectColor1 { get; set; } = new Color(160f / 255f, 120f / 255f, 85f / 255f);
            public Color OutlineEffectColor2 { get; set; } = new Color(212f / 255f, 185f / 255f, 150f / 255f);
            public bool EnableFPSDisplay { get; set; } = true;
            public bool EnableRainbowEffect { get; set; } = true;
            public bool EnableOutlineEffect { get; set; } = true;
        }

        private Config config = new Config();
        public static Menu Instance { get; private set; }

        #region Fields

        public GameObject buttonCollider, fingercollider, handcollider, watchObject, textObject, glassObject, canvasObject, canvasObject2, textObject2, lineText1, lineText2;
        public Font COCFont;
        private Texture2D Mat;

        private float deltaTime = 0.0f;
        private float colorChangeTime = 0;
        private float openTimeout;

        private bool IsAthrionOpen;

        private int pageSize = 7;
        private int currentPage = 0;
        private int currentIndex = 0;
        public static string category = "Home";

        private bool DoneAnim;

        public Color rainbowColor;

        public bool LoadedStaticObjects = false;
        public bool isFPSGoingLeft = false;
        bool open = false;
        bool canOpen = false;

        public static GameObject StaticCameraObject;
        public static GameObject StaticVCam;

        public Dictionary<string, Button[]> Categories = new Dictionary<string, Button[]>
        {
            { "Home", new Button[] {
                new Button("Movement", false, Athrion.Core.Handlers.Categories.Movement),
                new Button("Guardian", false, Athrion.Core.Handlers.Categories.Guardian),
                new Button("Blocks", false, Athrion.Core.Handlers.Categories.Blocks),
                new Button("Visuals", false, Athrion.Core.Handlers.Categories.Visuals),
                new Button("Advantage", false, Athrion.Core.Handlers.Categories.Advantage),
                new Button("Projectiles", false, Athrion.Core.Handlers.Categories.Projectiles),
                new Button("Graphics", false, Athrion.Core.Handlers.Categories.Graphics),
                new Button("Exploits", false, Athrion.Core.Handlers.Categories.Overpowered),
            }},
            { "Movement", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
                new Button("Wall Walk", true, Utilities.Utils.WallWalk),
                new Button("Wall Assist", true, Utilities.Utils.ClimbAssist),
                new Button("Platforms", true, PlatformLibrary.SpawnPlatform),
                new Button("Ten Hertz", true, Utilities.Utils.tenhertz),
                new Button("Controller Fly", true, Utilities.Utils.ControlledFly),
                new Button("Hoverboard Boost", false, Utilities.Utils.HoverboardFoward),
                new Button("Movement EC", true, Utilities.Utils.EnhancedMovement),
                new Button("Low Gravity", true, Utilities.Utils.MovementLerp),
                new Button("Hoverboard Speed", true, Utilities.Utils.HoverBoardBoost),
                new Button("Leap Up", true, Utilities.Utils.LeapForward),
                new Button("Leap Infront", true, Utilities.Utils.LeapInfront),
                new Button("Leap Behind", true, Utilities.Utils.LeapBehind),
                new Button("Ironman", true, Utilities.Utils.IronManFlight),
                new Button("Object Spawner", true, ObjectSpawner.spawnblockorwtv),
            }},
            { "Guardian", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
            }},
            { "Blocks", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
            }},
            { "Visuals", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
            }},
            { "Projectiles", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
                new Button("Projectile Spam", true, ProjectileLib.ExampleMod),
            }},
            { "Advantage", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
                new Button("Low Tag Reach", true, Utilities.Utils.TagRL),
                new Button("Far Teach Reach", true, Utilities.Utils.TagReach),
            }},
            { "Graphics", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
            }},
            { "Exploits", new Button[] {
                new Button("Home", false, Athrion.Core.Handlers.Categories.Home),
                new Button("STA 1", true, Utilities.Utils.STA),
                new Button("STA 2", true, Utilities.Utils.StutterAll),
                new Button("Static Score", false, Utilities.Utils.NetworkString),
                new Button("Dymanic Score", true, Utilities.Utils.stst),
            }}
        };

        #endregion

        #region Helper Functions

        public void RefreshMenu()
        {
            Destroy(Menu.Instance.canvasObject);
            Menu.Instance.canvasObject = null;
            Draw();
        }

        private void LoadStaticGameObjects()
        {
            if (!StaticCameraObject && !StaticVCam)
            {
                StaticCameraObject = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
                StaticVCam = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1");
            }
        }

        private bool IsLastItem<T>(T[] array, T item)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("Array must not be null or empty.");
            }

            return array[array.Length - 1].Equals(item);
        }

        private bool IsFirstItem<T>(T[] array, T item)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("Array must not be null or empty.");
            }

            return array[0].Equals(item);
        }

        #endregion

        #region Startup

        private void Awake()
        {
            Instance = this;
            Debug.Log(config.MenuName + "Menu has been Instantiated.");
        }

        private void LateUpdate()
        {
            try
            {
                foreach (KeyValuePair<string, Button[]> bnx in Categories)
                {
                    foreach (Button btn in bnx.Value)
                    {
                        if (btn.Enabled && btn.OnClick != null)
                        {
                            btn.OnClick();
                        }
                    }
                }

                if (!fingercollider)
                    CreateFingerCollider(ref fingercollider, GorillaLocomotion.Player.Instance.rightControllerTransform);

                if (!LoadedStaticObjects)
                {
                    LoadStaticGameObjects();
                    LoadedStaticObjects = true;
                }

                bool isButtonHeld = ControllerInputPoller.instance.leftControllerSecondaryButton;

                if (isButtonHeld)
                {
                    if (!IsAthrionOpen && Time.time >= openTimeout)
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(120, true, 0.5f);
                        IsAthrionOpen = true;
                        openTimeout = Time.time + 1f;
                    }
                }
                else
                {
                    if (IsAthrionOpen)
                    {
                        IsAthrionOpen = false;
                    }

                    canOpen = true;
                }

                if (IsAthrionOpen)
                {
                    if (canvasObject == null)
                    {
                        Draw();
                        CreateClicker(ref buttonCollider, GorillaLocomotion.Player.Instance.rightControllerTransform);
                    }
                    else
                    {
                        canvasObject.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position + new Vector3(0f, 0.31f, -0.06f);
                        canvasObject.transform.LookAt(Camera.main.transform);
                        FPSMenu();
                    }
                }
                else if (canvasObject != null)
                {
                    canvasObject.AddComponent<UIManager>().StartScalingDown();
                    if (canvasObject.transform.localScale == Vector3.zero)
                    {
                        DoneAnim = false;
                        Destroy(canvasObject);
                        Destroy(buttonCollider);
                        canvasObject = null;
                        buttonCollider = null;
                    }
                }

                if (StaticVCam.activeInHierarchy)
                {
                    StaticVCam.SetActive(false);
                }
                StaticCameraObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
                StaticCameraObject.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
                StaticCameraObject.GetComponent<Camera>().fieldOfView = 100;
            }
            catch (Exception e)
            {
                File.AppendAllText("Athrion\\Errors\\Athrion.log", e.ToString());
            }
        }

        #endregion

        #region Drawing

        private void CreateFingerCollider(ref GameObject reference, Transform parent)
        {
            if (reference != null) return;

            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(reference.GetComponent<SphereCollider>());
            Destroy(reference.GetComponent<Renderer>());

            reference.transform.SetParent(parent);
            reference.transform.localPosition = Vector3.zero;
            reference.transform.localScale = Vector3.one * 0.015f;
            reference.name = "WatchCursor";

            var cursorMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = new Color(0.96f, 0.87f, 0.70f) 
            };

            var renderer = reference.AddComponent<MeshRenderer>();
            renderer.material = cursorMaterial;
        }

        private void CreateClicker(ref GameObject reference, Transform parent)
        {
            if (!reference)
            {
                reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                reference.transform.parent = parent;
                reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                reference.name = "Cursor";
            }
        }

        private void Draw()
        {
            try
            {
                if (COCFont == null)
                    COCFont = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/debugtext").GetComponent<Text>().font;

                canvasObject = new GameObject("Canvas");
                Canvas canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;

                GameObject panelObject = new GameObject("Panel");
                panelObject.transform.SetParent(canvasObject.transform, false);
                RectTransform panelTransform = panelObject.AddComponent<RectTransform>();
                panelTransform.sizeDelta = new Vector2(700, 100);
                panelTransform.localScale = new Vector3(0.0004f, -0.0039f, 1f);
                UnityEngine.UI.Image panelImage = panelObject.AddComponent<UnityEngine.UI.Image>();
                panelImage.color = new Color32(200, 170, 130, 255);

                if (config.EnableOutlineEffect)
                {
                    Outline panelOutline = panelObject.AddComponent<Outline>();
                    panelOutline.effectColor = Color.white;
                    panelOutline.effectDistance = new Vector2(1f, 1f);
                    StartCoroutine(OutlineEffect(panelOutline));
                }

                canvasObject.AddComponent<NonTransparentUI>();
                GraphicRaycaster raycaster = canvasObject.AddComponent<GraphicRaycaster>();

                textObject = new GameObject("Text");
                textObject.transform.localPosition = new Vector3(0.0117f, 0.16f, 0.0002f);
                textObject.transform.localScale = new Vector3(-0.003f, 0.0024f, 1.4281f);
                textObject.transform.SetParent(canvasObject.transform, false);
                RectTransform textTransform = textObject.AddComponent<RectTransform>();
                textTransform.sizeDelta = new Vector2(200, 50);
                UnityEngine.UI.Text textComponent = textObject.AddComponent<UnityEngine.UI.Text>();
                textComponent.text = config.MenuName;
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.font = COCFont;
                textComponent.fontSize = 44;
                textComponent.color = config.MenuTitleColor;

                AddLine(1 * 0.024f, ref lineText1);

                #region Buttons
                // Backward Button
                GameObject btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                btn.GetComponent<BoxCollider>().isTrigger = true;
                btn.name = "backwardbtn";
                btn.transform.rotation = Quaternion.identity;
                btn.transform.localScale = new Vector3(0.02f, -0.0216f, 0.0036f);
                btn.transform.localPosition = new Vector3(0.0982f, -0.165f, 0.0019f);
                btn.transform.SetParent(canvasObject.transform, false);
                btn.AddComponent<ButtonCollider>().clickedButton = new Button("<", false);
                Renderer asfasfas = btn.GetComponent<Renderer>();
                asfasfas.material.color = config.ButtonDisabledColor;

                GameObject btntextObject = new GameObject("btnText");
                btntextObject.transform.localPosition = new Vector3(0.1042f, -0.1632f, 0.0038f);
                btntextObject.transform.localScale = new Vector3(-0.003f, 0.0029f, 1.4281f);
                btntextObject.transform.SetParent(canvasObject.transform, false);
                RectTransform btntextTransform = btntextObject.AddComponent<RectTransform>();
                btntextTransform.sizeDelta = new Vector2(200, 50);
                UnityEngine.UI.Text btntextComponent = btntextObject.AddComponent<UnityEngine.UI.Text>();
                btntextComponent.text = "<";
                btntextComponent.alignment = TextAnchor.MiddleCenter;
                btntextComponent.font = COCFont;
                btntextComponent.fontSize = 44;
                btntextComponent.color = Color.white;

                // Forward Button
                GameObject btn2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                btn2.name = "forwardbtn";
                btn2.GetComponent<BoxCollider>().isTrigger = true;
                btn2.transform.rotation = Quaternion.identity;
                btn2.transform.localScale = new Vector3(0.02f, -0.0216f, 0.0036f);
                btn2.transform.localPosition = new Vector3(0.0697f, -0.165f, 0.0019f);
                btn2.transform.SetParent(canvasObject.transform, false);
                btn2.AddComponent<ButtonCollider>().clickedButton = new Button(">", false);
                Renderer asfa = btn2.GetComponent<Renderer>();
                asfa.material.color = config.ButtonDisabledColor;

                GameObject btntextObject2 = new GameObject("btnText");
                btntextObject2.transform.localPosition = new Vector3(0.0731f, -0.1632f, 0.0038f);
                btntextObject2.transform.localScale = new Vector3(-0.003f, 0.0029f, 1.4281f);
                btntextObject2.transform.SetParent(canvasObject.transform, false);
                RectTransform btntextTransform2 = btntextObject2.AddComponent<RectTransform>();
                btntextTransform2.sizeDelta = new Vector2(200, 50);
                UnityEngine.UI.Text btntextComponent2 = btntextObject2.AddComponent<UnityEngine.UI.Text>();
                btntextComponent2.text = ">";
                btntextComponent2.alignment = TextAnchor.MiddleCenter;
                btntextComponent2.font = COCFont;
                btntextComponent2.fontSize = 44;
                btntextComponent2.color = Color.white;

                // Enter Button
                GameObject etnerbtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                etnerbtn.name = "enterbtn";
                etnerbtn.GetComponent<BoxCollider>().isTrigger = true;
                etnerbtn.transform.rotation = Quaternion.identity;
                etnerbtn.transform.localScale = new Vector3(0.0476f, -0.0216f, 0.0036f);
                etnerbtn.transform.localPosition = new Vector3(0.0218f, -0.165f, 0.0038f);
                etnerbtn.transform.SetParent(canvasObject.transform, false);
                etnerbtn.AddComponent<ButtonCollider>().clickedButton = new Button("flush", false);
                Renderer buttonRenderer = etnerbtn.GetComponent<Renderer>();
                buttonRenderer.material.color = config.ButtonDisabledColor;

                GameObject etnerbtntext = new GameObject("entrbtnText");
                etnerbtntext.transform.localPosition = new Vector3(0.0233f, -0.1634f, 0.0058f);
                etnerbtntext.transform.localScale = new Vector3(-0.0019f, 0.0021f, 1.4281f);
                etnerbtntext.transform.SetParent(canvasObject.transform, false);
                RectTransform etnerbtntexttransform = etnerbtntext.AddComponent<RectTransform>();
                etnerbtntexttransform.sizeDelta = new Vector2(200, 50);
                UnityEngine.UI.Text etnerbtntextcomponent = etnerbtntext.AddComponent<UnityEngine.UI.Text>();
                etnerbtntextcomponent.text = "FLUSH";
                etnerbtntextcomponent.alignment = TextAnchor.MiddleCenter;
                etnerbtntextcomponent.font = COCFont;
                etnerbtntextcomponent.fontSize = 44;
                etnerbtntextcomponent.color = Color.white;

                // Disconnect Button
                GameObject dcbtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                dcbtn.name = "dcbtn";
                dcbtn.GetComponent<BoxCollider>().isTrigger = true;
                dcbtn.transform.rotation = Quaternion.identity;
                dcbtn.transform.localScale = new Vector3(0.0476f, -0.0216f, 0.0036f);
                dcbtn.transform.localPosition = new Vector3(-0.04f, -0.165f, 0.0058f);
                dcbtn.transform.SetParent(canvasObject.transform, false);
                Renderer dc = dcbtn.GetComponent<Renderer>();
                dc.material.color = config.ButtonDisabledColor;
                dcbtn.AddComponent<ButtonCollider>().clickedButton = new Button("leave", false);

                GameObject dcbtntext = new GameObject("dcbtnText");
                dcbtntext.transform.localPosition = new Vector3(-0.039f, -0.1644f, 0.0077f);
                dcbtntext.transform.localScale = new Vector3(-0.0018f, 0.0018f, 1.4281f);
                dcbtntext.transform.SetParent(canvasObject.transform, false);
                RectTransform dcbtntexttransform = dcbtntext.AddComponent<RectTransform>();
                etnerbtntexttransform.sizeDelta = new Vector2(200, 50);
                UnityEngine.UI.Text dcbtntextcomponent = dcbtntext.AddComponent<UnityEngine.UI.Text>();
                dcbtntextcomponent.text = "LEAVE";
                dcbtntextcomponent.alignment = TextAnchor.MiddleCenter;
                dcbtntextcomponent.font = COCFont;
                dcbtntextcomponent.fontSize = 44;
                dcbtntextcomponent.color = Color.white;
                #endregion

                Button[] array2 = Categories[category].Skip(currentPage * pageSize).Take(pageSize).ToArray();
                for (int i = 0; i < array2.Length; i++)
                {
                    AddButton(array2[i], -0.036f * i);
                }

                AddLine(7 * -0.034f, ref lineText2);

                if (!DoneAnim)
                {
                    canvasObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                    canvasObject.AddComponent<UIManager>().StartScalingUp();
                }
                DoneAnim = true;
            }
            catch (Exception e)
            {
                File.AppendAllText("AthrionErrors.log", e.ToString());
            }
        }

        private IEnumerator OutlineEffect(Outline outline)
        {
            while (true)
            {
                float t = Mathf.PingPong(Time.time / 3f, 1f);
                outline.effectColor = Color.Lerp(config.OutlineEffectColor1, config.OutlineEffectColor2, t);
                yield return null;
            }
        }

        private void AddButton(Button button, float offset)
        {
            GameObject buttonparent = new GameObject(Guid.NewGuid().ToString());
            buttonparent.transform.SetParent(canvasObject.transform, false);
            buttonparent.transform.position = buttonparent.transform.position + new Vector3(0.015f, -0.02f + offset, 0);

            GameObject functionbtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            functionbtn.name = "functionbtn";
            functionbtn.GetComponent<BoxCollider>().isTrigger = true;
            functionbtn.transform.rotation = Quaternion.identity;
            functionbtn.transform.localScale = new Vector3(0.02f, -0.0216f, 0.0036f);
            functionbtn.transform.localPosition = new Vector3(0.0784f, 0.1223f, 0.0019f);
            functionbtn.transform.SetParent(buttonparent.transform, false);
            functionbtn.AddComponent<ButtonCollider>().clickedButton = button;
            Renderer buttonRenderer = functionbtn.GetComponent<Renderer>();

            if (button.Enabled)
            {
                buttonRenderer.material.color = config.ButtonEnabledColor;
            }
            else
            {
                buttonRenderer.material.color = config.ButtonDisabledColor;
            }

            if (config.EnableOutlineEffect)
            {
                Outline asdsadas = functionbtn.AddComponent<Outline>();
                asdsadas.effectColor = Color.white;
                asdsadas.effectDistance = new Vector2(1f, 1f);
                StartCoroutine(OutlineEffect(asdsadas));
            }

            GameObject functionbtntext = new GameObject("btnText");
            functionbtntext.transform.localPosition = new Vector3(-0.235f, 0.1237f, 0.0005f);
            functionbtntext.transform.localScale = new Vector3(-0.003f, 0.0029f, 1.4281f);
            functionbtntext.transform.SetParent(buttonparent.transform, false);
            RectTransform functionbtntextTransform = functionbtntext.AddComponent<RectTransform>();
            functionbtntextTransform.sizeDelta = new Vector2(200, 50);
            UnityEngine.UI.Text functionbtntextcomp = functionbtntext.AddComponent<UnityEngine.UI.Text>();
            functionbtntextcomp.text = button.buttonText.ToUpper();
            functionbtntextcomp.alignment = TextAnchor.MiddleLeft;
            functionbtntextcomp.font = COCFont;
            functionbtntextcomp.fontSize = 50;
            functionbtntextcomp.color = Color.white;

            Shadow shadow = functionbtntextcomp.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.3f);
            shadow.effectDistance = new Vector2(1, -1);
        }

        private void AddLine(float offset, ref GameObject functionbtntext)
        {
            GameObject buttonparent = new GameObject(Guid.NewGuid().ToString());
            buttonparent.transform.SetParent(canvasObject.transform, false);
            buttonparent.transform.position = buttonparent.transform.position + new Vector3(0.049f, -0.02f + offset, 0);
            functionbtntext = new GameObject("line");
            functionbtntext.transform.localPosition = new Vector3(-0.235f, 0.1237f, 0.0005f);
            functionbtntext.transform.localScale = new Vector3(-0.003f, 0.0029f, 1.4281f);
            functionbtntext.transform.SetParent(buttonparent.transform, false);
            RectTransform functionbtntextTransform = functionbtntext.AddComponent<RectTransform>();
            functionbtntextTransform.sizeDelta = new Vector2(200, 50);
            UnityEngine.UI.Text functionbtntextcomp = functionbtntext.AddComponent<UnityEngine.UI.Text>();
            string roblox = "<color=#" + ColorUtility.ToHtmlStringRGB(rainbowColor) + ">" + "_______________" + "</color>";
            functionbtntextcomp.text = roblox;
            functionbtntextcomp.alignment = TextAnchor.MiddleLeft;
            functionbtntextcomp.resizeTextForBestFit = true;
            functionbtntextcomp.font = COCFont;
            functionbtntextcomp.fontSize = 44;
            functionbtntextcomp.color = Color.white;
        }

        private void UpdateLine(GameObject objsect)
        {
            objsect.GetComponent<UnityEngine.UI.Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGB(rainbowColor) + ">" + "-----PRIVATE-----" + "</color>";
        }

        public void Toggle(Button button)
        {
            try
            {
                int num = (Categories[category].Length + pageSize - 1) / pageSize;
                switch (button.buttonText)
                {
                    case ">":
                        if (currentPage < num - 1)
                        {
                            currentPage++;
                        }
                        else
                        {
                            currentPage = 0;
                        }
                        RefreshMenu();
                        break;

                    case "<":
                        if (currentPage > 0)
                        {
                            currentPage--;
                        }
                        else
                        {
                            currentPage = num - 1;
                        }
                        RefreshMenu();
                        break;

                    case "leave":
                        PhotonNetwork.Disconnect();
                        return;

                    case "flush":
                        return;

                    case "setting":
                        currentPage = -1;
                        RefreshMenu();
                        return;

                    default:
                        OnClick(button);
                        break;
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("AthrionErrors.log", e.ToString());
            }
        }

        private void OnClick(Button button)
        {
            try
            {
                if (!button.IsToggle)
                {
                    button.OnClick();
                    return;
                }
                button.Enabled = !button.Enabled;
                if (!button.Enabled)
                {
                    button.OnDisable();
                }

                RefreshMenu();
            }
            catch (Exception e)
            {
                File.AppendAllText("AthrionErrors.log", e.ToString());
            }
        }

        public void FPSMenu()
        {
            if (!config.EnableFPSDisplay) return;

            float num = (float)Time.frameCount / 180f % 1f;
            rainbowColor = Color.HSVToRGB(num, 1f, 1f);
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            float fps = 1.0f / deltaTime;
            Text textComponent = textObject.GetComponent<Text>();
            int roundedFPS = Mathf.RoundToInt(fps);

            string menuText = config.MenuName + $" | [{currentPage + 1}]\nFPS: {roundedFPS}";
            char[] letters = menuText.ToCharArray();

            string[] stringArray = letters.Select(c => c.ToString()).ToArray();

            stringArray[currentIndex] = "<color=#" + ColorUtility.ToHtmlStringRGB(rainbowColor) + ">" + letters[currentIndex] + "</color>";

            if (Time.time > colorChangeTime + 0.1f)
            {
                if (isFPSGoingLeft && !IsFirstItem(stringArray, stringArray[currentIndex]))
                {
                    currentIndex--;
                }
                else if (!isFPSGoingLeft && !IsLastItem(stringArray, stringArray[currentIndex]))
                {
                    currentIndex++;
                }
                else
                {
                    isFPSGoingLeft = !isFPSGoingLeft;
                }
                colorChangeTime = Time.time;
            }

            string concatenatedString = string.Join("", stringArray);

            textComponent.text = concatenatedString;

            UpdateLine(lineText1);
            UpdateLine(lineText2);
        }

        public static Texture2D DownloadBackground(string imagestring)
        {
            byte[] imageBytes;
            using (WebClient webClient = new WebClient())
            {
                imageBytes = webClient.DownloadData(imagestring);
            }
            Texture2D ImageTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(ImageTexture, imageBytes);
            return ImageTexture;
        }

        #endregion

        #region Helpers

        public class ButtonCollider : MonoBehaviour
        {
            public Button clickedButton;

            static float framePressCooldown = 0;

            public void OnTriggerEnter(Collider collider)
            {
                try
                {
                    if (collider.gameObject.name == Menu.Instance.buttonCollider.name)
                    {
                        if (Time.time > framePressCooldown + 0.20f)
                        {
                            framePressCooldown = Time.time;
                            Menu.Instance.Toggle(clickedButton);
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, false, 0.2f);
                            GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength, GorillaTagger.Instance.tagHapticDuration / 2);
                            Menu.Instance.RefreshMenu();
                        }
                    }
                }
                catch (Exception e)
                {
                    File.AppendAllText("AthrionErrors.log", e.ToString());
                }
            }
        }

        public class Button
        {
            public string buttonText { get; set; }
            public bool IsToggle { get; set; }
            public bool Enabled { get; set; }
            public Action OnClick { get; set; }
            public Action OnDisable { get; set; }

            public Button(string Name, bool isToggle, Action OnClick = null, Action OnDisable = null, bool isActive = false)
            {
                buttonText = Name;
                IsToggle = isToggle;
                Enabled = isActive;
                this.OnClick = OnClick;
                this.OnDisable = OnDisable;
            }
        }

        public class NonTransparentUI : MonoBehaviour
        {
            void Start()
            {
                Canvas canvas = GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
                if (raycaster != null)
                {
                    raycaster.enabled = false;
                }
            }
        }

        public class UIManager : MonoBehaviour
        {
            public float duration = 0.1f;
            public float duration2 = 0.05f;
            private bool isScalingDown = false;
            private bool isScalingUp = false;
            private float elapsedTime = 0f;
            private Vector3 initialScale;

            private void Start()
            {
                initialScale = transform.localScale;
            }

            public void StartScalingDown()
            {
                isScalingDown = true;
                elapsedTime = 0f;
            }
            public void StartScalingUp()
            {
                isScalingUp = true;
                elapsedTime = 0f;
            }
            private void Update()
            {
                if (isScalingDown)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / duration2;
                    transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

                    if (elapsedTime >= duration)
                    {
                        transform.localScale = Vector3.zero;
                        isScalingDown = false;
                    }
                }
                if (isScalingUp)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / duration;
                    transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1f, 1f, 1f), t);

                    if (elapsedTime >= duration)
                    {
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        isScalingUp = false;
                    }
                }
            }
        }

        #endregion
    }
}
