using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ImageScroller : MonoBehaviour
{
    [SerializeField]
    private RawImage image;

    [SerializeField, Tooltip("The speed of scrolling in pixels per second")]
    private float scrollSpeed = 100f;

    [SerializeField, Range(0f, 1f), Tooltip("Percentage of screen height the largest object should occupy")]
    private float targetHeightPercentage = 0.5f;

    private Vector2 targetPosition;

    private RectTransform imageRectTransform;
    private RectTransform canvasRectTransform;
    private float imageHeight;
    private float canvasHeight;
    private LargestObjectFinder largestObjectFinder;
    private WinDetection winDetection;

    public bool StopMoving { get; set; } = false;

    private void Start()
    {
        // Get the RectTransforms
        imageRectTransform = image.GetComponent<RectTransform>();
        canvasRectTransform = image.canvas.GetComponent<RectTransform>();

        // Initialize the LargestObjectFinder
        largestObjectFinder = FindObjectOfType<LargestObjectFinder>();
        winDetection = FindObjectOfType<WinDetection>();

        // Get the height of the image and canvas
        imageHeight = imageRectTransform.rect.height;
        canvasHeight = canvasRectTransform.rect.height;

        // Ensure the RawImage is aligned to the bottom of the canvas initially
        AlignImageToBottomOfScreen();

        // Reset anything from the scene being won
        StopMoving = false;
    }

    private void Update()
    {
        if (StopMoving)
        {
            return;
        }

        Debug.Log($"targetPosition: ${targetPosition}");
        imageRectTransform.anchoredPosition = Vector2.MoveTowards(
            imageRectTransform.anchoredPosition,
            targetPosition,
            -(Time.deltaTime * scrollSpeed)
        );

        if (IsAtEndOfCanvas())
        {
            StopMoving = true;
            winDetection.RunWin();

            // TODO: This is if the image scrolled infinitely. It doesn't here, so do win detection instead.
            // AlignImageToBottomOfScreen();
            // ScrollImage();
        }
    }

    public void ScrollImage()
    {
        if (StopMoving)
        {
            return;
        }

        if (largestObjectFinder == null)
        {
            Debug.LogWarning("LargestObjectFinder not found!");
            return;
        }

        GameObject largestObject = largestObjectFinder.GetLargestObject();

        if (largestObject != null)
        {
            // Calculate the object's world space size
            Vector3 objectSize = largestObject.GetComponent<Renderer>().bounds.size;
            float objectHeight = objectSize.y;

            // Calculate the required scroll speed based on the object height
            float targetWorldHeight = targetHeightPercentage * canvasHeight;
            float scrollFactor = objectHeight / targetWorldHeight;
            // TODO: Update 100000f later, that's just for testing
            float adjustedScrollSpeed = scrollSpeed * 1000f * scrollFactor;

            // Scroll the image
            float scrollAmount = adjustedScrollSpeed * Time.deltaTime;
            targetPosition += new Vector2(0f, scrollAmount);

            // If the image has moved beyond the edge of the canvas, reset its position
            if (IsAtEndOfCanvas())
            {
                StopMoving = true;
                winDetection.RunWin();
                // TODO: This is if the image scrolled infinitely. It doesn't here, so do win detection instead.
                // AlignImageToBottomOfScreen();
            }
        }
    }

    private void AlignImageToBottomOfScreen()
    {
        // Set the initial position of the image to the bottom of the canvas
        imageRectTransform.anchoredPosition = new Vector2(0f, 0f);
        targetPosition = new Vector2(0f, 0f);
    }

    private bool IsAtEndOfCanvas()
    {
        if (scrollSpeed > 0)
        {
            // If the image has moved beyond the bottom of the canvas
            return (imageRectTransform.anchoredPosition.y >= imageHeight + canvasHeight);
        }

        // If the image has moved beyond the top of the canvas
        return (-imageRectTransform.anchoredPosition.y >= imageHeight - canvasHeight);
    }
}
