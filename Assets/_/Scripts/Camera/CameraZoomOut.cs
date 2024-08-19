using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Camera))]
public class CameraZoomOut : MonoBehaviour
{
    [SerializeField, Range(0f, 10f), Tooltip("The speed at which the camera zooms out")]
    private float zoomSpeed = 2f;

    [SerializeField, Range(0f, 10f), Tooltip("Multipler to control camera zoom based on the larget object's scale")]
    private float zoomScaler = 2f;

    private Camera camera;
    private LargestObjectFinder largestObjectFinder;

    private void Start()
    {
        camera = GetComponent<Camera>();
        largestObjectFinder = FindObjectOfType<LargestObjectFinder>();
    }

    private void Update()
    {
        this.ZoomOut();
    }

    private void ZoomOut()
    {
        GameObject largestObject = largestObjectFinder.GetLargestObject();

        if (largestObject != null)
        {
            // Calculate the target orthographic size based on the largest object's scale
            float targetSize = largestObject.transform.localScale.x * zoomScaler;

            // Smoothly interpolate the camera's orthographic size to the target size
            float newOrthographicSize = Mathf.Lerp(
                camera.orthographicSize,
                targetSize,
                Time.deltaTime * zoomSpeed
            );

            // Only update orthographic size if it's zooming out
            if (newOrthographicSize > camera.orthographicSize)
            {
                camera.orthographicSize = newOrthographicSize;
            }
        }
    }
}
