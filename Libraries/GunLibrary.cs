using BepInEx;
using Photon.Pun;
using Athrion.Libary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Athrion.Libary
{
    public class Config
    {
        public Vector3 PointerScale { get; set; } = new Vector3(0.2f, 0.2f, 0.2f);
        public Color32 PointerColorStart { get; set; } = new Color32(0, 255, 100, 255);
        public Color32 PointerColorEnd { get; set; } = new Color32(0, 200, 255, 255);
        public Color32 TriggeredPointerColorStart { get; set; } = new Color32(255, 100, 50, 255);
        public Color32 TriggeredPointerColorEnd { get; set; } = new Color32(255, 150, 0, 255);
        public float LineWidth { get; set; } = 0.03f;
        public Color32 LineColorStart { get; set; } = new Color32(0, 255, 150, 255);
        public Color32 LineColorEnd { get; set; } = new Color32(0, 180, 255, 255);
        public Color32 TriggeredLineColorStart { get; set; } = new Color32(255, 100, 50, 255);
        public Color32 TriggeredLineColorEnd { get; set; } = new Color32(255, 150, 0, 255);
        public bool EnableAnimations { get; set; } = true;
        public float PulseSpeed { get; set; } = 2f;
        public float PulseAmplitude { get; set; } = 0.04f;
        public bool EnableParticles { get; set; } = true;
        public float ParticleStartSize { get; set; } = 0.1f;
        public float ParticleStartSpeed { get; set; } = 0.5f;
        public int ParticleMaxCount { get; set; } = 100;
        public float ParticleEmissionRate { get; set; } = 20f;
        public bool EnableBoxESP { get; set; } = true;
        public float BoxESPWidth { get; set; } = 1.0f;
        public float BoxESPHeight { get; set; } = 2.0f;
        public Color32 BoxESPColor { get; set; } = new Color32(0, 255, 100, 255);
        public Color32 BoxESPOuterColor { get; set; } = new Color32(255, 150, 0, 255);
        public int LineCurve { get; set; } = 150;
        public float WaveFrequency { get; set; } = 5f;
        public float WaveAmplitude { get; set; } = 0.05f;
    }

    public class AthrionGunLibrary : MonoBehaviour
    {
        public static Config GunConfig = new Config();
        public static GameObject spherepointer;
        public static VRRig LockedRigOrPlayerOrwhatever;
        public static Vector3 lr;
        public static ParticleSystem particleSystem;
        private static float waveTimeOffset = 0f;
        private static bool colorIndexInitialized = false;
        private static int ColorIndex = 0;

        public enum AnimationMode
        {
            None,
            Wave,
            Pulse,
            Zigzag,
            Bouncing,
            Spiral,
            SineWave,
            Helix,
            Sawtooth,
            TriangleWave,
            DefaultBezier
        }

        private static AnimationMode currentAnimationMode = AnimationMode.DefaultBezier;

        public static AnimationMode CurrentAnimationMode
        {
            get => currentAnimationMode;
            set => currentAnimationMode = value;
        }

        private static Vector3 CalculateBezierPoint(Vector3 start, Vector3 mid, Vector3 end, float t)
        {
            return Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * mid + Mathf.Pow(t, 2) * end;
        }

        private static void CurveLineRenderer(LineRenderer lineRenderer, Vector3 start, Vector3 mid, Vector3 end)
        {
            lineRenderer.positionCount = GunConfig.LineCurve;
            waveTimeOffset += Time.deltaTime * 2f;

            for (int i = 0; i < GunConfig.LineCurve; i++)
            {
                float t = (float)i / (GunConfig.LineCurve - 1);
                Vector3 position;

                switch (currentAnimationMode)
                {
                    case AnimationMode.Wave:
                        position = Vector3.Lerp(start, end, t);
                        position.x += Mathf.Sin(t * GunConfig.WaveFrequency + waveTimeOffset) * GunConfig.WaveAmplitude;
                        break;

                    case AnimationMode.Pulse:
                        position = Vector3.Lerp(start, end, t + Mathf.Sin(Time.time * GunConfig.WaveFrequency) * 0.02f);
                        break;

                    case AnimationMode.Zigzag:
                        position = Vector3.Lerp(start, end, t);
                        position.x += (i % 2 == 0 ? 1 : -1) * GunConfig.WaveAmplitude * Mathf.Sin(waveTimeOffset);
                        break;

                    case AnimationMode.Bouncing:
                        position = Vector3.Lerp(start, end, t);
                        position.y += Mathf.Abs(Mathf.Sin(t * GunConfig.WaveFrequency + waveTimeOffset) * GunConfig.WaveAmplitude);
                        break;

                    case AnimationMode.Spiral:
                        position = Vector3.Lerp(start, end, t);
                        position.x += Mathf.Sin(t * GunConfig.WaveFrequency + waveTimeOffset) * GunConfig.WaveAmplitude;
                        position.y += Mathf.Cos(t * GunConfig.WaveFrequency + waveTimeOffset) * GunConfig.WaveAmplitude;
                        break;

                    case AnimationMode.SineWave:
                        position = Vector3.Lerp(start, end, t);
                        position.z += Mathf.Sin(t * GunConfig.WaveFrequency + waveTimeOffset) * GunConfig.WaveAmplitude;
                        break;

                    case AnimationMode.Helix:
                        position = Vector3.Lerp(start, end, t);
                        position.x += Mathf.Sin(2 * Mathf.PI * GunConfig.WaveFrequency * t + waveTimeOffset) * GunConfig.WaveAmplitude;
                        position.z += Mathf.Cos(2 * Mathf.PI * GunConfig.WaveFrequency * t + waveTimeOffset) * GunConfig.WaveAmplitude;
                        break;

                    case AnimationMode.Sawtooth:
                        position = Vector3.Lerp(start, end, t);
                        position.z += GunConfig.WaveAmplitude * (2f * (t * GunConfig.WaveFrequency - Mathf.Floor(t * GunConfig.WaveFrequency + 0.5f)));
                        break;

                    case AnimationMode.TriangleWave:
                        position = Vector3.Lerp(start, end, t);
                        position.y += GunConfig.WaveAmplitude * (2f * Mathf.Abs(2f * (t * GunConfig.WaveFrequency - Mathf.Floor(t * GunConfig.WaveFrequency + 0.5f))) - 1f);
                        break;

                    case AnimationMode.DefaultBezier:
                    default:
                        position = CalculateBezierPoint(start, mid, end, t);
                        break;
                }

                lineRenderer.SetPosition(i, position);
            }
        }

        private static IEnumerator AnimateLineGradient(LineRenderer lineRenderer, Color32 startColor, Color32 endColor)
        {
            float t = 0;
            while (true)
            {
                t += Time.deltaTime;
                lineRenderer.startColor = Color.Lerp(startColor, endColor, Mathf.PingPong(t, 1));
                lineRenderer.endColor = Color.Lerp(endColor, startColor, Mathf.PingPong(t, 1));
                yield return null;
            }
        }

        private static IEnumerator StartCurvyLineRenderer(LineRenderer lineRenderer, Vector3 start, Vector3 mid, Vector3 end, Color32 startColor, Color32 endColor)
        {
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
            lineRenderer.positionCount = GunConfig.LineCurve;
            while (true)
            {
                CurveLineRenderer(lineRenderer, start, mid, end);
                yield return null;
            }
        }

        private static IEnumerator PulsePointer(GameObject pointer)
        {
            Vector3 originalScale = pointer.transform.localScale;
            while (true)
            {
                float scaleFactor = 1 + Mathf.Sin(Time.time * GunConfig.PulseSpeed) * GunConfig.PulseAmplitude;
                pointer.transform.localScale = originalScale * scaleFactor;
                yield return null;
            }
        }

        private static void AddPointerParticles(GameObject pointer)
        {
            if (!GunConfig.EnableParticles) return;

            particleSystem = pointer.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(GunConfig.PointerColorStart, GunConfig.PointerColorEnd);
            main.startSize = GunConfig.ParticleStartSize;
            main.startSpeed = GunConfig.ParticleStartSpeed;
            main.maxParticles = GunConfig.ParticleMaxCount;
            main.duration = 1.0f;
            main.loop = true;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = GunConfig.ParticleEmissionRate;

            ParticleSystem.ShapeModule shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.05f;

            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        private static IEnumerator SmoothStopParticles()
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            float initialRate = emission.rateOverTime.constant;
            emission.rateOverTime = Mathf.Lerp(initialRate, 0, Time.deltaTime * 2f);
            yield return null;
            particleSystem.Stop();
        }

        public static void StartVrGun(Action action, bool LockOn)
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                RaycastHit raycastHit;
                bool hitDetected = Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out raycastHit, float.MaxValue);

                if (spherepointer == null)
                {
                    CreatePointer();
                }

                if (hitDetected)
                {
                    if (LockedRigOrPlayerOrwhatever == null)
                    {
                        spherepointer.transform.position = raycastHit.point;
                        spherepointer.GetComponent<Renderer>().material.color = GunConfig.PointerColorStart;

                        if (LockOn && raycastHit.collider.GetComponentInParent<VRRig>() != null)
                        {
                            LockedRigOrPlayerOrwhatever = raycastHit.collider.GetComponentInParent<VRRig>();

                            if (LockedRigOrPlayerOrwhatever == GorillaTagger.Instance.offlineVRRig)
                            {
                                LockedRigOrPlayerOrwhatever = null;
                                return;
                            }
                        }
                    }
                    else
                    {
                        spherepointer.transform.position = LockedRigOrPlayerOrwhatever.transform.position;
                    }
                }

                HandleLineRendering();

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f)
                {
                    spherepointer.GetComponent<Renderer>().material.color = GunConfig.TriggeredPointerColorStart;

                    if (LockOn && LockedRigOrPlayerOrwhatever != null)
                    {
                        action();
                        if (GunConfig.EnableBoxESP) BoxESP();
                    }
                    else if (!LockOn)
                    {
                        action();
                    }
                }
                else if (LockedRigOrPlayerOrwhatever != null)
                {
                    LockedRigOrPlayerOrwhatever = null;
                }
            }
            else if (spherepointer != null)
            {
                CleanupPointer();
            }
        }

        public static void StartPcGun(Action action, bool LockOn)
        {
            Ray ray = GameObject.Find("Shoulder Camera").activeSelf ? GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition) : GorillaTagger.Instance.mainCamera.GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition);

            if (Mouse.current.rightButton.isPressed)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, float.PositiveInfinity, -32777) && spherepointer == null)
                {
                    if (spherepointer == null)
                    {
                        spherepointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        spherepointer.AddComponent<Renderer>();
                        spherepointer.transform.localScale = GunConfig.PointerScale;
                        spherepointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        Destroy(spherepointer.GetComponent<BoxCollider>());
                        Destroy(spherepointer.GetComponent<Rigidbody>());
                        Destroy(spherepointer.GetComponent<Collider>());
                        lr = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;

                        AthrionGunLibrary instance = spherepointer.AddComponent<AthrionGunLibrary>();
                        instance.StartCoroutine(PulsePointer(spherepointer));
                        AddPointerParticles(spherepointer);
                    }
                }

                if (LockedRigOrPlayerOrwhatever == null)
                {
                    spherepointer.transform.position = raycastHit.point;
                    spherepointer.GetComponent<Renderer>().material.color = GunConfig.PointerColorStart;
                }
                else
                {
                    spherepointer.transform.position = LockedRigOrPlayerOrwhatever.transform.position;
                }

                lr = Vector3.Lerp(lr, (GorillaTagger.Instance.rightHandTransform.position + spherepointer.transform.position) / 2f, Time.deltaTime * 6f);

                GameObject gameObject = new GameObject("Line");
                LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = GunConfig.LineWidth;
                lineRenderer.endWidth = GunConfig.LineWidth;
                Shader lineShader = Shader.Find("Sprites/Default");

                if (lineShader != null)
                {
                    lineRenderer.material = new Material(lineShader);
                }
                AthrionGunLibrary instanceForLine = gameObject.AddComponent<AthrionGunLibrary>();
                instanceForLine.StartCoroutine(StartCurvyLineRenderer(lineRenderer, GorillaTagger.Instance.rightHandTransform.position, lr, spherepointer.transform.position, GunConfig.LineColorStart, GunConfig.LineColorEnd));
                instanceForLine.StartCoroutine(AnimateLineGradient(lineRenderer, GunConfig.LineColorStart, GunConfig.LineColorEnd));

                if (spherepointer.transform.hasChanged)
                {
                    if (GunConfig.EnableParticles && !particleSystem.isPlaying)
                    {
                        particleSystem.Play();
                    }
                }
                else if (GunConfig.EnableParticles)
                {
                    particleSystem.Stop();
                }

                Destroy(lineRenderer, Time.deltaTime);

                if (Mouse.current.leftButton.isPressed)
                {
                    spherepointer.GetComponent<Renderer>().material.color = GunConfig.TriggeredPointerColorStart;
                    if (LockOn)
                    {
                        if (LockedRigOrPlayerOrwhatever == null)
                        {
                            LockedRigOrPlayerOrwhatever = raycastHit.collider.GetComponentInParent<VRRig>();

                            if (LockedRigOrPlayerOrwhatever == GorillaTagger.Instance.offlineVRRig)
                            {
                                LockedRigOrPlayerOrwhatever = null;
                                return;
                            }
                        }
                        if (LockedRigOrPlayerOrwhatever != null)
                        {
                            spherepointer.transform.position = LockedRigOrPlayerOrwhatever.transform.position;
                            action();
                            if (GunConfig.EnableBoxESP) BoxESP();
                        }
                        return;
                    }
                    action();
                    return;
                }
                else if (LockedRigOrPlayerOrwhatever != null)
                {
                    LockedRigOrPlayerOrwhatever = null;
                    return;
                }
            }
            else if (spherepointer != null)
            {
                Destroy(spherepointer);
                spherepointer = null;
                LockedRigOrPlayerOrwhatever = null;
            }
        }

        private static void CreatePointer()
        {
            spherepointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spherepointer.AddComponent<Renderer>();
            spherepointer.transform.localScale = GunConfig.PointerScale;
            spherepointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            Destroy(spherepointer.GetComponent<BoxCollider>());
            Destroy(spherepointer.GetComponent<Rigidbody>());
            Destroy(spherepointer.GetComponent<Collider>());
            lr = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;

            AthrionGunLibrary instance = spherepointer.AddComponent<AthrionGunLibrary>();
            instance.StartCoroutine(PulsePointer(spherepointer));
            AddPointerParticles(spherepointer);
        }

        private static void CleanupPointer()
        {
            Destroy(spherepointer);
            spherepointer = null;
            LockedRigOrPlayerOrwhatever = null;
        }

        private static void HandleLineRendering()
        {
            lr = Vector3.Lerp(lr, (GorillaTagger.Instance.rightHandTransform.position + spherepointer.transform.position) / 2f, Time.deltaTime * 6f);

            GameObject gameObject = new GameObject("Line");
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = GunConfig.LineWidth;
            lineRenderer.endWidth = GunConfig.LineWidth;
            Shader lineShader = Shader.Find("Sprites/Default");

            if (lineShader != null)
            {
                lineRenderer.material = new Material(lineShader);
            }

            AthrionGunLibrary instanceForLine = gameObject.AddComponent<AthrionGunLibrary>();
            instanceForLine.StartCoroutine(StartCurvyLineRenderer(lineRenderer, GorillaTagger.Instance.rightHandTransform.position, lr, spherepointer.transform.position, GunConfig.LineColorStart, GunConfig.LineColorEnd));
            instanceForLine.StartCoroutine(AnimateLineGradient(lineRenderer, GunConfig.LineColorStart, GunConfig.LineColorEnd));

            Destroy(lineRenderer, Time.deltaTime);
        }

        public static void ToggleParticles()
        {
            GunConfig.EnableParticles = !GunConfig.EnableParticles;
            if (!GunConfig.EnableParticles && particleSystem != null)
            {
                AthrionGunLibrary instance = spherepointer.AddComponent<AthrionGunLibrary>();
                instance.StartCoroutine(SmoothStopParticles());
            }
        }

        public static void start2guns(Action action, bool lockOn)
        {
            if (IsXRDeviceActive())
            {
                StartVrGun(action, lockOn);
            }
            else
            {
                StartPcGun(action, lockOn);
            }
        }

        public static bool IsXRDeviceActive()
        {
            List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(displays);
            foreach (XRDisplaySubsystem display in displays)
            {
                if (display.running)
                    return true;
            }
            return false;
        }

        public static void ChangePointerColor()
        {
            Color32[] colors = {
                new Color32(0, 255, 100, 255),
                new Color32(0, 200, 255, 255),
                new Color32(255, 215, 0, 255),
                new Color32(255, 165, 0, 255),
                new Color32(128, 0, 128, 255),
                new Color32(255, 0, 255, 255),
                new Color32(0, 0, 128, 255),
                new Color32(255, 69, 0, 255),
                new Color32(50, 205, 50, 255),
                new Color32(240, 128, 128, 255),
                new Color32(173, 216, 230, 255),
                new Color32(64, 224, 208, 255),
                new Color32(255, 20, 147, 255),
                new Color32(123, 104, 238, 255),
                new Color32(255, 99, 71, 255),
                new Color32(0, 191, 255, 255),
                new Color32(255, 140, 0, 255),
                new Color32(75, 0, 130, 255),
                new Color32(60, 179, 113, 255),
                new Color32(244, 164, 96, 255),
                new Color32(138, 43, 226, 255),
                new Color32(255, 105, 180, 255),
                new Color32(255, 250, 205, 255),
                new Color32(139, 0, 0, 255)
            };

            if (!colorIndexInitialized)
            {
                ColorIndex = 0;
                colorIndexInitialized = true;
            }

            ColorIndex = (ColorIndex + 1) % colors.Length;

            Color32 selectedColorStart = colors[ColorIndex];
            Color32 selectedColorEnd = Color.Lerp(selectedColorStart, new Color32(255, 255, 255, 255), 0.5f);

            GunConfig.PointerColorStart = selectedColorStart;
            GunConfig.PointerColorEnd = selectedColorEnd;

            if (spherepointer != null)
            {
                spherepointer.GetComponent<Renderer>().material.color = selectedColorStart;
            }

            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = new ParticleSystem.MinMaxGradient(selectedColorStart, selectedColorEnd);
            }
        }

        private static IEnumerator AnimateBox(GameObject box, LineRenderer outline)
        {
            Vector3 originalScale = box.transform.localScale;
            while (box != null)
            {
                float scaleFactor = 1 + Mathf.Sin(Time.time * GunConfig.PulseSpeed) * GunConfig.PulseAmplitude;
                box.transform.localScale = originalScale * scaleFactor;
                outline.startColor = Color.Lerp(GunConfig.BoxESPColor, GunConfig.BoxESPOuterColor, Mathf.PingPong(Time.time, 1));
                outline.endColor = Color.Lerp(GunConfig.BoxESPColor, GunConfig.BoxESPOuterColor, Mathf.PingPong(Time.time, 1));
                yield return null;
            }
        }

        public static void BoxESP()
        {
            if (PhotonNetwork.InRoom || PhotonNetwork.InLobby)
            {
                if (LockedRigOrPlayerOrwhatever != null && LockedRigOrPlayerOrwhatever != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject box = new GameObject();
                    LineRenderer outline = box.AddComponent<LineRenderer>();
                    Vector3 rigPosition = LockedRigOrPlayerOrwhatever.transform.position;

                    Vector3[] corners = new Vector3[5];
                    float height = GunConfig.BoxESPHeight;
                    float width = GunConfig.BoxESPWidth;

                    corners[0] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (-width / 2) + LockedRigOrPlayerOrwhatever.transform.up * (height / 2);
                    corners[1] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (width / 2) + LockedRigOrPlayerOrwhatever.transform.up * (height / 2);
                    corners[2] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (width / 2) + LockedRigOrPlayerOrwhatever.transform.up * (-height / 2);
                    corners[3] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (-width / 2) + LockedRigOrPlayerOrwhatever.transform.up * (-height / 2);
                    corners[4] = corners[0];

                    outline.positionCount = corners.Length;
                    outline.SetPositions(corners);
                    outline.startWidth = 0.007f;
                    outline.endWidth = 0.007f;
                    Shader outlineShader = Shader.Find("Sprites/Default");
                    if (outlineShader != null)
                    {
                        outline.material = new Material(outlineShader);
                    }
                    outline.material.renderQueue = 3000;
                    outline.startColor = GunConfig.BoxESPColor;
                    outline.endColor = GunConfig.BoxESPColor;

                    LineRenderer outerOutline = new GameObject().AddComponent<LineRenderer>();
                    outerOutline.transform.parent = box.transform;

                    Vector3[] outerCorners = new Vector3[5];
                    outerCorners[0] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (-width / 2 - 0.02f) + LockedRigOrPlayerOrwhatever.transform.up * (height / 2 + 0.02f);
                    outerCorners[1] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (width / 2 + 0.02f) + LockedRigOrPlayerOrwhatever.transform.up * (height / 2 + 0.02f);
                    outerCorners[2] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (width / 2 + 0.02f) + LockedRigOrPlayerOrwhatever.transform.up * (-height / 2 - 0.02f);
                    outerCorners[3] = rigPosition + LockedRigOrPlayerOrwhatever.transform.right * (-width / 2 - 0.02f) + LockedRigOrPlayerOrwhatever.transform.up * (-height / 2 - 0.02f);
                    outerCorners[4] = outerCorners[0];

                    outerOutline.positionCount = outerCorners.Length;
                    outerOutline.SetPositions(outerCorners);
                    outerOutline.startWidth = 0.01f;
                    outerOutline.endWidth = 0.01f;
                    Shader outerShader = Shader.Find("Sprites/Default");
                    if (outerShader != null)
                    {
                        outerOutline.material = new Material(outerShader);
                    }
                    outerOutline.material.renderQueue = 3000;
                    outerOutline.startColor = GunConfig.BoxESPOuterColor;
                    outerOutline.endColor = GunConfig.BoxESPOuterColor;

                    box.transform.position = LockedRigOrPlayerOrwhatever.transform.position;
                    box.transform.rotation = LockedRigOrPlayerOrwhatever.transform.rotation;

                    AthrionGunLibrary instance = box.AddComponent<AthrionGunLibrary>();
                    instance.StartCoroutine(AnimateBox(box, outerOutline));

                    Destroy(box, 0.05f);
                }
            }
        }

        public static void ToggleAnimationMode()
        {
            currentAnimationMode = (AnimationMode)(((int)currentAnimationMode + 1) % Enum.GetValues(typeof(AnimationMode)).Length);
        }

        public static Vector3 GetPointerPos()
        {
            if (spherepointer != null)
            {
                return spherepointer.transform.position;
            }
            return Vector3.zero;
        }
    }
}
