using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Config
{
    public Vector3 PlatformScale { get; set; } = new Vector3(0.6f, 0.03f, 0.6f); // Platform Scale/Size
    public Color PlatformColor { get; set; } = new Color(0.95f, 0.85f, 0.65f); // Platform Color
    public bool EnableAnimations { get; set; } = false; // Enable Animations
    public PlatformAnimationType AnimationType { get; set; } = PlatformAnimationType.None; // Platform animation type change none to a Enum
    public Vector3 SpawnOffset { get; set; } = new Vector3(0, -0.1f, 0); // Keep it like this if you want it to spawn bellow the hand!
}

public enum PlatformAnimationType
{
    None,
    FadeOut,
    Grow,
    Shrink
}

public class PlatformLibrary : MonoBehaviour
{
    private static List<GameObject> spawnedplats = new List<GameObject>();
    private static bool rgheld = false;
    private static bool lgheld = false;
    private static Config config = new Config();

    public static void SpawnPlatform()
    {
        if (ControllerInputPoller.instance.rightGrab)
        {
            if (!rgheld)
            {
                rgheld = true;
                GameObject platform = CreatePlatform(GorillaTagger.Instance.rightHandTransform.position + config.SpawnOffset, "Platform_Right");
                spawnedplats.Add(platform);
            }
        }
        else if (rgheld)
        {
            rgheld = false;
            DestroyLastPlatform("Platform_Right");
        }

        if (ControllerInputPoller.instance.leftGrab)
        {
            if (!lgheld)
            {
                lgheld = true;
                GameObject platform = CreatePlatform(GorillaTagger.Instance.leftHandTransform.position + config.SpawnOffset, "Platform_Left");
                spawnedplats.Add(platform);
            }
        }
        else if (lgheld)
        {
            lgheld = false;
            DestroyLastPlatform("Platform_Left");
        }
    }

    private static GameObject CreatePlatform(Vector3 position, string name)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.transform.position = position;
        platform.transform.localScale = config.PlatformScale;
        platform.name = name;
        Renderer renderer = platform.GetComponent<Renderer>();
        renderer.material.color = config.PlatformColor;

        if (config.EnableAnimations)
        {
            switch (config.AnimationType)
            {
                case PlatformAnimationType.FadeOut:
                    platform.AddComponent<FadeOutAnimation>();
                    break;
                case PlatformAnimationType.Grow:
                    platform.AddComponent<GrowAnimation>();
                    break;
                case PlatformAnimationType.Shrink:
                    platform.AddComponent<ShrinkAnimation>();
                    break;
            }
        }
        return platform;
    }

    private static void DestroyLastPlatform(string platformName)
    {
        for (int i = spawnedplats.Count - 1; i >= 0; i--)
        {
            if (spawnedplats[i] != null && spawnedplats[i].name == platformName)
            {
                Destroy(spawnedplats[i]);
                spawnedplats.RemoveAt(i);
                break;
            }
        }
    }
}

public class FadeOutAnimation : MonoBehaviour
{
    private void Start() => StartCoroutine(FadeOut());
    private IEnumerator FadeOut()
    {
        Renderer renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            color.a = t;
            renderer.material.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
}

public class GrowAnimation : MonoBehaviour
{
    private void Start() => StartCoroutine(Grow());
    private IEnumerator Grow()
    {
        Vector3 originalScale = transform.localScale;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            transform.localScale = originalScale * (1 + t * 0.5f);
            yield return null;
        }
    }
}

public class ShrinkAnimation : MonoBehaviour
{
    private void Start() => StartCoroutine(Shrink());
    private IEnumerator Shrink()
    {
        Vector3 originalScale = transform.localScale;
        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            transform.localScale = originalScale * t;
            yield return null;
        }
        Destroy(gameObject);
    }
}
