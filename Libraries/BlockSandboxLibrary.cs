using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    // I got this off a sandbox github I would say who but I forgot.
    private static List<GameObject> spawnedplatforms = new List<GameObject>();
    private static bool rightgrabbeingheld = false;
    private static bool leftgrabbeingheld = false;
    private static bool isLeftControllerSecondaryButtonPressed = false;

    public static void spawnblockorwtv()
    {
        if (ControllerInputPoller.instance.rightGrab)
        {
            if (!rightgrabbeingheld)
            {
                rightgrabbeingheld = true;

                GameObject spawnedObject = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity);
                spawnedObject.name = "cunrqwerqwrqw32v43f";
                spawnedObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                spawnedplatforms.Add(spawnedObject);

                Collider[] colliders = spawnedObject.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    collider.enabled = false;
                }
            }
        }
        else
        {
            if (rightgrabbeingheld)
            {
                rightgrabbeingheld = false;

                foreach (var spawnedObject in spawnedplatforms)
                {
                    Collider[] colliders = spawnedObject.GetComponentsInChildren<Collider>();
                    foreach (var collider in colliders)
                    {
                        collider.enabled = true;
                    }
                }
            }
        }

        if (ControllerInputPoller.instance.leftGrab)
        {
            if (!leftgrabbeingheld)
            {
                leftgrabbeingheld = true;
            }

            if (!isLeftControllerSecondaryButtonPressed)
            {
                string dataToCopy = "";
                foreach (var spawnedObject in spawnedplatforms)
                {
                    dataToCopy += $"Position: {spawnedObject.transform.position}, Scale: {spawnedObject.transform.localScale}\n";
                }
                GUIUtility.systemCopyBuffer = dataToCopy;
                isLeftControllerSecondaryButtonPressed = true;
            }
        }
        else
        {
            if (leftgrabbeingheld)
            {
                leftgrabbeingheld = false;
                isLeftControllerSecondaryButtonPressed = false;
            }
        }
    }
}
