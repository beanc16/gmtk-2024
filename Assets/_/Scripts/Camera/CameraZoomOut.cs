using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof (Camera))]
public class CameraZoomOut : MonoBehaviour
{
    [SerializeField, Range(0f, 10f), Tooltip("The speed at which the camera zooms out")]
    private float zoomSpeed = 2f;

    [SerializeField, Range(0f, 1f), Tooltip("Percentage of screen height the largest object should occupy")]
    private float targetHeightPercentage = 0.5f;

    private Camera camera;
    private LargestObjectFinder largestObjectFinder;
    public UnityEvent OnZoomingOut;

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
            // Calculate the object's world space size
            Vector3 objectSize = largestObject.GetComponent<Renderer>().bounds.size;
            float objectHeight = objectSize.y;

            // Calculate the height of the camera's view in world units
            float cameraHeight = 2f * camera.orthographicSize;

            // Calculate the required orthographic size to make the object occupy the target percentage of the screen height
            float targetWorldHeight = targetHeightPercentage * cameraHeight;
            float targetOrthographicSize = (objectHeight / targetWorldHeight) * camera.orthographicSize;

            // Smoothly interpolate the camera's orthographic size to the target size
            float newOrthographicSize = Mathf.Lerp(
                camera.orthographicSize,
                targetOrthographicSize,
                Time.deltaTime * zoomSpeed
            );

            // Only update orthographic size if it's zooming out
            if (newOrthographicSize > camera.orthographicSize)
            {
                camera.orthographicSize = newOrthographicSize;
                OnZoomingOut?.Invoke();
            }
        }
    }
}
