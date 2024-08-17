using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class NewLargestObjectEvent : UnityEvent<GameObject> { }

public class LargestObjectFinder : MonoBehaviour
{
    private ObjectPoolingManager objectPoolingManager;
    private Vector3 prevLargestObjectScale = Vector3.one;

    public NewLargestObjectEvent OnNewLargestObject;

    private void Start()
    {
        objectPoolingManager = FindObjectOfType<ObjectPoolingManager>();
    }

    public GameObject GetLargestObject()
    {
        GameObject largestObject = null;
        float largestScale = 0f;

        foreach (GameObject obj in objectPoolingManager.Pool)
        {
            if (obj.activeInHierarchy)
            {
                float objScale = obj.transform.localScale.x;

                // Check if this object's scale is larger than the current largest
                if (objScale > largestScale)
                {
                    largestScale = objScale;
                    largestObject = obj;
                }
            }
        }

        return largestObject;
    }

    public void TrySetLargestObject()
    {
        GameObject largestObject = GetLargestObject();

        // Set new largest object scale and send new largest object event
        if (largestObject.transform.localScale.x > prevLargestObjectScale.x)
        {
            prevLargestObjectScale = new Vector3(
                largestObject.transform.localScale.x,
                largestObject.transform.localScale.y,
                largestObject.transform.localScale.z
            );

            OnNewLargestObject?.Invoke(largestObject);
        }
    }
}
