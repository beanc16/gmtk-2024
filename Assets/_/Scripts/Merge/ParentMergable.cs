using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ParentMergable : MonoBehaviour
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

    private Transform ParentTransform
    {
        get => this.transform.parent;
    }

    private GameObject Parent
    {
        get => ParentTransform.gameObject;
    }

    private void Start()
    {
        // Set up canvas
        Canvas canvas = ParentTransform.GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;

        // Set up UI
        numOfTimesMergedText = ParentTransform.GetComponentInChildren<TextMeshProUGUI>();
        numOfTimesMergedText.text = numOfTimesMerged.ToString();

        // Find necessary objects
        objectPoolingManager = FindObjectOfType<ObjectPoolingManager>();
        largestObjectFinder = FindObjectOfType<LargestObjectFinder>();

        startingScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        ParentMergable otherMergable = collider.gameObject.GetComponent<ParentMergable>();
        Debug.Log("otherMergable: " + otherMergable?.gameObject?.name);

        if (CanMerge(otherMergable))
        {
            // Get all mergables in the trigger
            List<ParentMergable> mergablesToMerge = GetNearbyMergables(collider, otherMergable);

            // Start the merge process with all mergables involved
            Debug.Log("CanMerge: " + mergablesToMerge);
            StartCoroutine(MergeMergables(mergablesToMerge));
        }
    }


    private bool CanMerge(ParentMergable? otherMergable)
    {
        // Allow merging if the other ParentMergable is not null, and either is not merging or is currently merging but hasn't finished
        return (otherMergable != null && (!otherMergable.isMerging || !this.isMerging));
    }

    private List<ParentMergable> GetNearbyMergables(Collider2D collider, ParentMergable otherMergable)
    {
        // Create a list of all mergables in the trigger
        List<ParentMergable> mergablesToMerge = new List<ParentMergable> { this, otherMergable };

        // Find all colliders within the detection range of the current collider
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            collider.transform.position,
            mergableDetectionRange
        );

        foreach (var nearbyCollider in nearbyColliders)
        {
            ParentMergable nearbyMergable = nearbyCollider.GetComponent<ParentMergable>();

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

    private IEnumerator MergeMergables(List<ParentMergable> mergables)
    {
        // Mark all mergables in the list as currently merging
        foreach (var mergable in mergables)
        {
            mergable.isMerging = true;
        }

        // Determine the largest mergable based on the area
        ParentMergable largestMergable = mergables.OrderByDescending(m => m.Area).First();

        // Calculate the necessary values to animate the merge
        var newMergableObjectVariables = CalculateNewMergableObjectVariables(mergables);

        // Animate the merge
        return largestMergable.AnimateMerge(
            mergables,
            newMergableObjectVariables.totalArea,
            newMergableObjectVariables.newScale
        );
    }

    private MergableCalculationResult CalculateNewMergableObjectVariables(List<ParentMergable> mergables)
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

    private IEnumerator AnimateMerge(List<ParentMergable> mergables, float totalArea, Vector3 newScale)
    {
        // Create initial values for interpolation
        float time = 0f;
        float largestScale = 0f;
        Dictionary<ParentMergable, Vector3> initialScales = new Dictionary<ParentMergable, Vector3>();
        Dictionary<ParentMergable, Vector3> initialPositions = new Dictionary<ParentMergable, Vector3>();
        Rigidbody2D largestMergableRb = GetComponent<Rigidbody2D>();
        Vector3 startingVelocity = largestMergableRb.velocity;

        foreach (var mergable in mergables)
        {
            initialScales[mergable] = mergable.ParentTransform.localScale;
            initialPositions[mergable] = mergable.ParentTransform.position;

            // Find the largest scale in the group
            largestScale = Mathf.Max(largestScale, mergable.ParentTransform.localScale.x);
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
                float scaleFactor = mergable.ParentTransform.localScale.x / largestScale;

                // Shrink each mergable's size to zero over time
                mergable.ParentTransform.localScale = Vector3.Lerp(
                    initialScales[mergable],
                    Vector3.zero,
                    time
                );

                // Move each mergable's position toward the largest ParentMergable's center of mass over time
                mergable.ParentTransform.position = Vector3.Lerp(
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
        transform.parent.gameObject.SetActive(false);

        // Finish merging
        isMerging = false;

        // Reset the scale after merge animation
        ParentTransform.localScale = startingScale;

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
