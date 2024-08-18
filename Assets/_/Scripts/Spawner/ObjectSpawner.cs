using UnityEngine;
using Beanc16.Common.General;

[RequireComponent(typeof (GameObjectStopwatch))]
public class ObjectSpawner : MonoBehaviour
{
    [SerializeField, Range(0, 25), Tooltip("The number of objects to spawn at the start of the scene")]
    private int startingObjectCount = 2;

    [SerializeField, Range(0f, 10f), Tooltip("How often to spawn objects in seconds")]
    private float spawnFrequency = 2f;

    [SerializeField, Range(0f, 5f), Tooltip("How far to check for nearby objects at a spawn point before allowing a spawn")]
    private float spawnScanRange = 1f;

    [SerializeField, Range(1f, 100f), Tooltip("How much to reduce spawnFrequency by whenever a new largest object is made")]
    private float largestObjectScaleDivisor = 1f;
    [SerializeField, Range(0f, 1f), Tooltip("How much to increment largestObjectScaleDivisor by whenever a new largest object is made")]
    private float largestObjectScaleIncrementer = 0.1f;

    private float SpawnFrequency
    {
        // Get the spawn frequency (no lower than 0.5)
        get => Mathf.Max(spawnFrequency / largestObjectScaleDivisor, 0.5f);
        // get => Mathf.Max(spawnFrequency / largestObjectScaleDivisor, 0.15f);
    }

    [SerializeField]
    private ObjectPoolingManager objectPoolingManager;
    private LargestObjectFinder largestObjectFinder;
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
        largestObjectFinder = FindObjectOfType<LargestObjectFinder>();
        stopwatch = GetComponent<GameObjectStopwatch>();

        for (int index = 0; index < startingObjectCount; index++)
        {
            SpawnObject();
        }
    }

    private void Update()
    {
        if (stopwatch.TimeSinceLapMarked >= SpawnFrequency)
        {
            stopwatch.MarkLap();
            this.SpawnObject();
        }
    }

    private void SpawnObject()
    {
        GameObject obj = objectPoolingManager.GetObject();

        Vector2 spawnPoint = Vector2.zero;
        Collider2D[] nearbyColliders = new Collider2D[1] { new Collider2D() };

        // Get a spawn point that's away from other objects or is after 50 attempts to do so failed
        for (int index = 0; index < 50 && nearbyColliders.Length > 0; index++)
        {
            spawnPoint = GetRandomSpawnPoint();
            nearbyColliders = Physics2D.OverlapCircleAll(spawnPoint, spawnScanRange);
        }

        // Set the object's new position
        obj.transform.position = spawnPoint;
    }

    private Vector2 GetRandomSpawnPoint()
    {
        float xPosition = Random.Range(LeftOfScreen + 0.5f, RightOfScreen - 0.5f);
        float yPosition = Random.Range(BottomOfScreen + 0.5f, TopOfScreen - 0.5f);
        return new Vector2(xPosition, yPosition);
    }

    public void UpdateLargestObjectScaleMultiplier()
    {
        largestObjectScaleDivisor += largestObjectScaleIncrementer;
    }
}
