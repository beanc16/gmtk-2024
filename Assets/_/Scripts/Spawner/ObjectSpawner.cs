using UnityEngine;
using Beanc16.Common.General;

[RequireComponent(typeof (GameObjectStopwatch))]
public class ObjectSpawner : MonoBehaviour
{
    [SerializeField, Range(0, 25), Tooltip("The number of objects to spawn at the start of the scene")]
    private int startingObjectCount = 2;

    [SerializeField, Range(0f, 10f), Tooltip("How often to spawn objects in seconds")]
    private float spawnFrequency = 2f;

    [SerializeField]
    private ObjectPoolingManager objectPoolingManager;
    private GameObjectStopwatch stopwatch;

    private float TopOfScreen
    {
        get => Camera.main.orthographicSize;
    }

    private float BottomOfScreen
    {
        get => -Camera.main.orthographicSize;
    }

    private float LeftOfScreen
    {
        get => -Camera.main.orthographicSize * Camera.main.aspect;
    }

    private float RightOfScreen
    {
        get => Camera.main.orthographicSize * Camera.main.aspect;
    }

    private void Start()
    {
        stopwatch = GetComponent<GameObjectStopwatch>();

        for (int index = 0; index < startingObjectCount; index++)
        {
            SpawnObject();
        }
    }

    private void Update()
    {
        if (stopwatch.TimeSinceLapMarked >= spawnFrequency)
        {
            stopwatch.MarkLap();
            this.SpawnObject();
        }
    }

    private void SpawnObject()
    {
        // TODO: Find a way to make sure this doesn't overlap with other objects
        GameObject obj = objectPoolingManager.GetObject();
        Vector2 spawnPoint = GetRandomSpawnPoint();

        // Set the object's new position
        obj.transform.position = spawnPoint;
    }

    private Vector2 GetRandomSpawnPoint()
    {
        float xPosition = Random.Range(LeftOfScreen, RightOfScreen);
        float yPosition = Random.Range(BottomOfScreen, TopOfScreen);
        return new Vector2(xPosition, yPosition);
    }
}
