using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicScreenBoundaries : MonoBehaviour
{
    [SerializeField]
    private GameObject topWall;

    [SerializeField]
    private GameObject bottomWall;

    [SerializeField]
    private GameObject leftWall;

    [SerializeField]
    private GameObject rightWall;

    [HideInInspector]
    public List<GameObject> Walls
    {
        get => new List<GameObject> { topWall, bottomWall, leftWall, rightWall };
    }

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        AdjustWalls();
    }

    private void Update()
    {
        AdjustWalls();
    }

    private void AdjustWalls()
    {
        // Calculate the screen bounds based on the camera's size and aspect ratio
        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Adjust the scale and position of each wall
        topWall.transform.localScale = new Vector3(screenWidth * 3, screenWidth, 1f);
        topWall.transform.position = new Vector3(
            0f,
            mainCamera.orthographicSize + (screenWidth / 2),
            0f
        );

        bottomWall.transform.localScale = new Vector3(screenWidth * 3, screenWidth, 1f);
        bottomWall.transform.position = new Vector3(
            0f,
            -mainCamera.orthographicSize - (screenWidth / 2),
            0f
        );

        leftWall.transform.localScale = new Vector3(screenWidth, screenHeight, 1f);
        leftWall.transform.position = new Vector3(
            -mainCamera.orthographicSize * mainCamera.aspect - (screenWidth / 2),
            0f,
            0f
        );

        rightWall.transform.localScale = new Vector3(screenWidth, screenHeight, 1f);
        rightWall.transform.position = new Vector3(
            mainCamera.orthographicSize * mainCamera.aspect + (screenWidth / 2),
            0f,
            0f
        );
    }
}
