using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Beanc16.Common.UI;

public class Mergable : MonoBehaviour
{
    [SerializeField, Range(0f, 25f), Tooltip("How quickly the objects should merge")]
    private float mergeSpeed = 5f;

    [SerializeField, Range(0f, 2f), Tooltip("How much larger merged objects should be")]
    private float mergeMultiplier = 2f;

    [SerializeField, Range(0f, 1f), Tooltip("How far to check for other mergables to merge when two mergables collide")]
    private float mergableDetectionRange = 1f;

    [HideInInspector]
    public bool isMerging = false;

    private Vector3 startingScale;

    [SerializeField]
    private MergeType mergeType = MergeType.ON_COLLISION;

    // Other objects
    private ObjectPoolingManager objectPoolingManager;
    private LargestObjectFinder largestObjectFinder;
    private GameSceneAudioHandling gameSceneAudioHandling;

    // For events
    public UnityEvent OnMergeComplete;
    public UnityEvent<int> OnMergeCompleteWithData;

    public float Area
    {
        get
        {
            // Calculate the area of the circle (assuming circular shape)
            float radius = this.transform.localScale.x / 2f;
            return Mathf.PI * Mathf.Pow(radius, 2);
        }
    }

    private void Start()
    {
        // Find necessary objects
        objectPoolingManager = FindObjectOfType<ObjectPoolingManager>();
        largestObjectFinder = FindObjectOfType<LargestObjectFinder>();
        gameSceneAudioHandling = FindObjectOfType<GameSceneAudioHandling>();

        // Set up score incrementing when a merge occurs
        ScoreTextManager scoreTextManager = FindObjectOfType<ScoreTextManager>();
        OnMergeCompleteWithData.AddListener(scoreTextManager.IncrementScore);

        // Set up background image to scroll when a merge occurs
        ImageScroller imageScroller = FindObjectOfType<ImageScroller>();
        OnMergeComplete.AddListener(imageScroller.ScrollImage);

        // Set initial scale
        startingScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mergeType != MergeType.ON_COLLISION)
        {
            return;
        }

        Mergable otherMergable = collision.gameObject.GetComponent<Mergable>();

        if (CanMerge(otherMergable))
        {
            // Get all mergables in the collision
            List<Mergable> mergablesToMerge = GetNearbyMergables(collision, otherMergable);

            // Start the merge process with all mergables involved
            StartCoroutine(MergeMergables(mergablesToMerge));
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (mergeType != MergeType.ON_TRIGGER)
        {
            return;
        }

        Mergable otherMergable = collider.gameObject.GetComponent<Mergable>();

        if (CanMerge(otherMergable))
        {
            // Get all mergables in the trigger
            List<Mergable> mergablesToMerge = GetNearbyMergables(collider, otherMergable);

            // Start the merge process with all mergables involved
            StartCoroutine(MergeMergables(mergablesToMerge));
        }
    }

    private bool CanMerge(Mergable? otherMergable)
    {
        // Allow merging if the other Mergable is not null, and either is not merging or is currently merging but hasn't finished
        return (otherMergable != null && (!otherMergable.isMerging || !this.isMerging));
    }

    private List<Mergable> GetNearbyMergables(Collision2D collision, Mergable otherMergable)
    {
        // Create a list of all mergables in the collision
        List<Mergable> mergablesToMerge = new List<Mergable> { this, otherMergable };

        // Check for additional nearby mergables and add them to the merge list
        foreach (var contact in collision.contacts)
        {
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
                contact.point,
                mergableDetectionRange
            );

            foreach (var collider in nearbyColliders)
            {
                Mergable nearbyMergable = collider.GetComponent<Mergable>();

                // Add the nearby mergable to the merge list if isn't merging and isn't in the list yet
                if (
                    nearbyMergable != null
                    && !nearbyMergable.isMerging
                    && !mergablesToMerge.Contains(nearbyMergable)
                )
                {
                    mergablesToMerge.Add(nearbyMergable);
                }
            }
        }

        return mergablesToMerge;
    }

    private List<Mergable> GetNearbyMergables(Collider2D collider, Mergable otherMergable)
    {
        // Create a list of all mergables in the trigger
        List<Mergable> mergablesToMerge = new List<Mergable> { this, otherMergable };

        // Find all colliders within the detection range of the current collider
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            collider.transform.position,
            mergableDetectionRange
        );

        foreach (var nearbyCollider in nearbyColliders)
        {
            Mergable nearbyMergable = nearbyCollider.GetComponent<Mergable>();

            // Add the nearby mergable to the merge list if it isn't merging and isn't in the list yet
            if (
                nearbyMergable != null
                && !nearbyMergable.isMerging
                && !mergablesToMerge.Contains(nearbyMergable)
            )
            {
                mergablesToMerge.Add(nearbyMergable);
            }
        }

        return mergablesToMerge;
    }

    private IEnumerator MergeMergables(List<Mergable> mergables)
    {
        // Mark all mergables in the list as currently merging
        foreach (var mergable in mergables)
        {
            mergable.isMerging = true;
        }

        // Determine the largest mergable based on the area
        Mergable largestMergable = mergables.OrderByDescending(m => m.Area).First();

        // Calculate the necessary values to animate the merge
        var newMergableObjectVariables = CalculateNewMergableObjectVariables(mergables);

        // Animate the merge
        return largestMergable.AnimateMerge(
            mergables,
            newMergableObjectVariables.totalArea,
            newMergableObjectVariables.newScale
        );
    }

    private MergableCalculationResult CalculateNewMergableObjectVariables(List<Mergable> mergables)
    {
        // Calculate the combined area for the merged object
        float totalArea = 0f;

        // Get the sum of the area and the center of mass
        foreach (var mergable in mergables)
        {
            totalArea += mergable.Area;
        }

        // Calculate the new radius and scale of the merged object
        float newRadius = Mathf.Sqrt(totalArea / Mathf.PI);
        Vector3 newScale = new Vector3(
            newRadius * mergeMultiplier,
            newRadius * mergeMultiplier,
            1f
        );

        return new MergableCalculationResult(totalArea, newScale);
    }

    private IEnumerator AnimateMerge(List<Mergable> mergables, float totalArea, Vector3 newScale)
    {
        // Create initial values for interpolation
        float time = 0f;
        float largestScale = 0f;
        Dictionary<Mergable, Vector3> initialScales = new Dictionary<Mergable, Vector3>();
        Dictionary<Mergable, Vector3> initialPositions = new Dictionary<Mergable, Vector3>();
        Rigidbody2D largestMergableRb = GetComponent<Rigidbody2D>();
        Vector3 startingVelocity = largestMergableRb.velocity;

        gameSceneAudioHandling.PlayBubbleMergeSfx();
        yield return null;

        foreach (var mergable in mergables)
        {
            initialScales[mergable] = mergable.transform.localScale;
            initialPositions[mergable] = mergable.transform.position;

            // Find the largest scale in the group
            largestScale = Mathf.Max(largestScale, mergable.transform.localScale.x);
        }

        while (time < 1f)
        {
            time += Time.deltaTime * mergeSpeed;

            foreach (var mergable in mergables)
            {
                // Scale the largest mergable to the new scale without modifying position
                if (mergable == this)
                {
                    transform.localScale = Vector3.Lerp(
                        initialScales[mergable],
                        newScale,
                        time
                    );
                    continue;
                }

                // Proportionally scale the smaller objects more than the larger objects
                float scaleFactor = mergable.transform.localScale.x / largestScale;

                // Shrink each mergable's size to zero over time
                mergable.transform.localScale = Vector3.Lerp(
                    initialScales[mergable],
                    Vector3.zero,
                    time
                );

                // Move each mergable's position toward the largest Mergable's center of mass over time
                mergable.transform.position = Vector3.Lerp(
                    initialPositions[mergable],
                    transform.position,
                    time * scaleFactor
                );
            }

            yield return null;
        }

        // After merging, retain the largest mergable's momentum
        if (largestMergableRb != null)
        {
            largestMergableRb.velocity = startingVelocity;
        }

        // Finalize the merge by setting the final scale and position
        transform.localScale = newScale;

        // Hide all other mergables that were merged into this one
        foreach (var mergable in mergables)
        {
            if (mergable != this)
            {
                mergable.Hide();
            }
        }

        // Mark the merge process as complete
        isMerging = false;
        largestObjectFinder.TrySetLargestObject();

        // Send merge complete event (don't include this mergable in the count to make it count number of merges rather than number of things merged)
        OnMergeCompleteWithData?.Invoke(mergables.Count - 1);
        OnMergeComplete.Invoke();
    }

    public void Hide()
    {
        // Hide this gameobject
        gameObject.SetActive(false);

        // Finish merging
        isMerging = false;

        // Reset the scale after merge animation
        transform.localScale = startingScale;
    }
}

public class MergableCalculationResult
{
    public float totalArea;
    public Vector3 newScale;

    public MergableCalculationResult(float totalArea, Vector3 newScale)
    {
        this.totalArea = totalArea;
        this.newScale = newScale;
    }
}

public enum MergeType
{
    ON_COLLISION,
    ON_TRIGGER,
}