using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageScroller : MonoBehaviour
{
    [SerializeField]
    private RawImage image;

    [SerializeField, Tooltip("The speed of scrolling in pixels per second")]
    private float scrollSpeed = 100f;

    [SerializeField]
    private bool shouldAutomaticallyScroll = true;

    private RectTransform imageRectTransform;
    private RectTransform canvasRectTransform;

    private float imageHeight;
    private float canvasHeight;

    private void Start()
    {
        // Get the RectTransforms
        imageRectTransform = image.GetComponent<RectTransform>();
        canvasRectTransform = image.canvas.GetComponent<RectTransform>();

        // Ensure the RawImage is aligned to the bottom of the canvas initially
        AlignImageToBottomOfScreen();

        // Get the height of the image and canvas
        imageHeight = imageRectTransform.rect.height;
        canvasHeight = canvasRectTransform.rect.height;

        // Start the scrolling coroutine
        if (shouldAutomaticallyScroll)
        {
            StartCoroutine(AutomaticallyScrollImage());
        }
    }

    private IEnumerator AutomaticallyScrollImage()
    {
        // Set the initial position of the image to the bottom of the canvas
        // (this assumes a pivot position aligned to the bottom of the screen)
        AlignImageToBottomOfScreen();

        while (true)
        {
            ScrollImage();
            yield return null;
        }
    }

    public void ScrollImage()
    {
        // Scroll the image
        float scrollAmount = scrollSpeed * Time.deltaTime;
        imageRectTransform.anchoredPosition += new Vector2(0f, scrollAmount);

        // If the image has moved beyond the edge of the canvas, reset its position
        if (IsAtEndOfCanvas())
        {
            AlignImageToBottomOfScreen();
        }
    }

    private void AlignImageToBottomOfScreen()
    {
        // Set the initial position of the image to the bottom of the canvas
        // (this assumes a pivot position aligned to the bottom of the screen)
        imageRectTransform.anchoredPosition = new Vector2(0f, 0f);
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
