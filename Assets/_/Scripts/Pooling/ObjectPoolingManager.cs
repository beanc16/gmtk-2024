using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField, Range(0, 100), Tooltip("The number of objects to start with in the pool")]
    private int startingObjectCount = 20;

    private List<GameObject> pool;

    public List<GameObject> Pool
    {
        get => pool;
    }

    private void Awake()
    {
        // Initialize list
        pool = new List<GameObject>(startingObjectCount);

        // Create objects in the pool
        for (int index = 0; index < startingObjectCount; index++)
        {
            InstantiateNewObject(false);
        }
    }

    public GameObject GetObject()
    {
        return GetObject(true);
    }

    public GameObject GetObject(bool shouldBeActive)
    {
        GameObject? obj = GetObjectFromPool();

        if (obj != null)
        {
            obj.SetActive(shouldBeActive);
            return obj;
        }

        return InstantiateNewObject(shouldBeActive);
    }

    private GameObject? GetObjectFromPool()
    {
        foreach (GameObject obj in pool)
        {
            // Get a gameobject from the pool that's currently inactive
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }

    private GameObject InstantiateNewObject(bool shouldBeActive)
    {
        // Create a new object
        GameObject obj = Instantiate(prefab);

        // Set the newly created object as a child of ObjectPoolingManager
        obj.transform.SetParent(this.transform);

        // Set the object as active/inactive
        obj.SetActive(shouldBeActive);

        // Add the new object to the pool
        pool.Add(obj);

        return obj;
    }
}
