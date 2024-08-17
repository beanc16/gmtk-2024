using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    // For UI
    private int numOfTimesMerged = 1;
    public int NumOfTimesMerged { get => numOfTimesMerged; }
    private TextMeshProUGUI numOfTimesMergedText;

    // Other objects
    private ObjectPoolingManager objectPoolingManager;
    private LargestObjectFinder largestObjectFinder;

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
        // Set up canvas
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;

        // Set up UI
        numOfTimesMergedText = this.GetComponentInChildren<TextMeshProUGUI>();
        numOfTimesMergedText.text = numOfTimesMerged.ToString();

        // Find necessary objects
        objectPoolingManager = FindObjectOfType<ObjectPoolingManager>();
        largestObjectFinder = FindObjectOfType<LargestObjectFinder>();

        startingScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Mergable otherMergable = collision.gameObject.GetComponent<Mergable>();

        if (CanMerge(otherMergable))
        {
            // Get all mergables in the collision
            List<Mergable> mergablesToMerge = GetNearbyMergables(collision, otherMergable);

            // Start the merge process with all mergables involved
            StartCoroutine(MergeMergables(mergablesToMerge));
        }
    }

    private bool CanMerge(Mergable? otherMergable)
    {
        // The given mergable exists, isn't merging, and neither is this mergable
        return (otherMergable != null && !otherMergable.isMerging && !this.isMerging);
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
                mergableDetectionRange * (transform.localScale.x / 2.5f) // Account for object scale
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

    private IEnumerator MergeMergables(List<Mergable> mergables)
    {
        // Mark all mergables in the list as currently merging
        foreach (var mergable in mergables)
        {
            mergable.isMerging = true;
        }

        // Calculate the necessary values to animate the merge
        MergableCalculationResult newMergableObjectVariables = CalculateNewMergableObjectVariables(mergables);

        // Animate the merge
        return AnimateMerge(
            mergables,
            newMergableObjectVariables.totalArea,
            newMergableObjectVariables.centerOfMass,
            newMergableObjectVariables.newScale
        );
    }

    private MergableCalculationResult CalculateNewMergableObjectVariables(List<Mergable> mergables)
    {
        // Calculate the combined area and center of mass for the merged object
        float totalArea = 0f;
        Vector3 centerOfMass = Vector3.zero;

        // Get the sum of the area and the center of mass
        foreach (var mergable in mergables)
        {
            totalArea += mergable.Area;
            centerOfMass += mergable.transform.position;
        }

        // Convert the center of mass from a sum to an average
        centerOfMass /= mergables.Count;

        // Calculate the new radius and scale of the merged object
        float newRadius = Mathf.Sqrt(totalArea / Mathf.PI);
        Vector3 newScale = new Vector3(
            newRadius * mergeMultiplier,
            newRadius * mergeMultiplier,
            1f
        );

        return new MergableCalculationResult(totalArea, centerOfMass, newScale);
    }

    private IEnumerator AnimateMerge(List<Mergable> mergables, float totalArea, Vector3 centerOfMass, Vector3 newScale)
    {
        // Store initial scales and positions for interpolation
        float time = 0f;
        Dictionary<Mergable, Vector3> initialScales = new Dictionary<Mergable, Vector3>();
        Dictionary<Mergable, Vector3> initialPositions = new Dictionary<Mergable, Vector3>();

        foreach (var mergable in mergables)
        {
            initialScales[mergable] = mergable.transform.localScale;
            initialPositions[mergable] = mergable.transform.position;
        }

        // Animate the merging process
        while (time < 1f)
        {
            time += Time.deltaTime * mergeSpeed;

            foreach (var mergable in mergables)
            {
                // Shrink each mergable's size to zero over time
                mergable.transform.localScale = Vector3.Lerp(initialScales[mergable], Vector3.zero, time);

                // Move each mergable's position toward the center of mass over time
                mergable.transform.position = Vector3.Lerp(initialPositions[mergable], centerOfMass, time);
            }
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, time);
            transform.position = Vector3.Lerp(transform.position, centerOfMass, time);

            yield return null;
        }

        // Finalize the merge by setting the final scale and position
        transform.localScale = newScale;
        transform.position = centerOfMass;

        // Hide all other mergables that were merged into this one
        foreach (var mergable in mergables)
        {
            if (mergable != this)
            {
                this.numOfTimesMerged += mergable.NumOfTimesMerged;
                mergable.Hide();
            }
        }

        // Mark the merge process as complete
        isMerging = false;
        largestObjectFinder.TrySetLargestObject();

        // Set the number of times merged
        SetNumOfTimesMerged();
    }

    public void Hide()
    {
        // Hide this gameobject
        gameObject.SetActive(false);

        // Finish merging
        isMerging = false;

        // Reset the scale after merge animation
        transform.localScale = startingScale;

        // Reset the number of times merged
        SetNumOfTimesMerged(1);
    }

    private void SetNumOfTimesMerged()
    {
        SetNumOfTimesMerged(this.numOfTimesMerged);
    }

    private void SetNumOfTimesMerged(int numOfTimesMerged)
    {
        // Update the value
        this.numOfTimesMerged = numOfTimesMerged;

        // Update the UI
        numOfTimesMergedText.text = numOfTimesMerged.ToString();
    }
}

public class MergableCalculationResult
{
    public float totalArea;
    public Vector3 centerOfMass;
    public Vector3 newScale;

    public MergableCalculationResult(float totalArea, Vector3 centerOfMass, Vector3 newScale)
    {
        this.totalArea = totalArea;
        this.centerOfMass = centerOfMass;
        this.newScale = newScale;
    }
}
