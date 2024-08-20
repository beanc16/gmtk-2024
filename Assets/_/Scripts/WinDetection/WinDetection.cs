using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Beanc16.Common.General;
using Beanc16.Common.Mechanics.Follow;
using Beanc16.Common.UI;

public class WinDetection : MonoBehaviour
{
    [SerializeField]
    private GameObjectToggleHandler winPanel;
    [SerializeField]
    private GameObjectToggleHandler pauseButton;
    [SerializeField]
    private ScoreTextManager scoreTextManager;
    [SerializeField]
    private TextMeshProUGUI winTextComponent;
    private MouseFollower2d mouseFollower;
    private ObjectSpawner objectSpawner;

    private void Awake()
    {
        mouseFollower = FindObjectOfType<MouseFollower2d>();
        objectSpawner = FindObjectOfType<ObjectSpawner>();
    }

    public void RunWin()
    {
        // Turn on/off necessary stuff
        List<GameObject> mergables = ToggleStuff(false);

        // Play victory sound
        AudioController.PlaySfx("Victory");

        // Update final score text
        winTextComponent.text = $"You merged {scoreTextManager.Score} bubbles!";

        // Make mergables float up
        foreach (GameObject obj in mergables)
        {
            StartCoroutine(FloatUpwards(obj));
        }
    }

    private IEnumerator FloatUpwards(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        // Float upwards
        while (!IsObjectOffScreen(obj))
        {
            rb.velocity = new Vector2(rb.velocity.x / 2, 0.5f);
            yield return null;
        }

        // Once off-screen, deactivate the GameObject
        obj.SetActive(false);
    }

    private bool IsObjectOffScreen(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null)
        {
            Debug.LogWarning("The GameObject does not have a Renderer component.");
            return false;
        }

        // Get the object's bounds in world space
        Bounds bounds = renderer.bounds;

        // Check if the entire bounds are outside the camera's viewport
        Vector3[] corners = new Vector3[8];
        corners[0] = Camera.main.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
        corners[1] = Camera.main.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
        corners[2] = Camera.main.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
        corners[3] = Camera.main.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
        corners[4] = Camera.main.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
        corners[5] = Camera.main.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
        corners[6] = Camera.main.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
        corners[7] = Camera.main.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));

        // Check if all corners are outside the viewport
        foreach (var corner in corners)
        {
            if (corner.z > 0 && corner.x >= 0 && corner.x <= 1 && corner.y >= 0 && corner.y <= 1)
            {
                return false; // At least one corner is on screen
            }
        }

        return true; // All corners are off screen
    }

    public void RestartAfterWin()
    {
        // Turn on/off necessary stuff
        ToggleStuff(true);
    }

    public List<GameObject> ToggleStuff(bool enabled)
    {
        // Show win panel
        winPanel.ToggleVisibility(!enabled);

        // Hide pause button
        pauseButton.ToggleVisibility(enabled);

        // Hide score text
        scoreTextManager.gameObject.SetActive(enabled);

        // Turn off mouse follower
        mouseFollower.gameObject.SetActive(enabled);

        // Turn off object spawner
        objectSpawner.gameObject.SetActive(enabled);

        // Turn off normal movement for mergables
        List<GameObject> mergables = FindObjectsOfType<Mergable>()
            .Select(mergable => mergable.gameObject)
            .ToList();
        ToggleMergableComponents(mergables, false);

        return mergables;
    }

    public void ToggleMergableComponents(List<GameObject> mergables, bool enabled)
    {
        foreach (GameObject mergable in mergables)
        {
            // Turn off custom physics
            mergable.GetComponent<KinematicDragSimulator>().enabled = enabled;
            mergable.GetComponent<KinematicCollisionSimulatorWithDynamicBoundaries>().enabled = enabled;

            // Turn off magnetic
            mergable.GetComponent<Magnetic>().enabled = enabled;

            // Turn off mergable
            mergable.GetComponent<Mergable>().enabled = enabled;
        }
    }
}
