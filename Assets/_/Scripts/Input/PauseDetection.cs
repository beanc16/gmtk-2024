using UnityEngine;
using Beanc16.Common.General;
using Beanc16.Common.Scenes;

public class PauseDetection : MonoBehaviour
{
    [SerializeField]
    private GameObjectToggleHandler pausePanel;
    [SerializeField]
    private GameObjectToggleHandler pauseButton;
    private TeleportToMouseObject teleportToMouseObject;
    private bool isPaused = false;

    private void Awake()
    {
        teleportToMouseObject = FindObjectOfType<TeleportToMouseObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                Unpause();
            }

            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        SceneHandler.PauseScene();
        pausePanel.ToggleVisibility(true);
        pauseButton.ToggleVisibility(false);
    }

    public void Unpause()
    {
        isPaused = false;
        SceneHandler.UnpauseScene();
        pausePanel.ToggleVisibility(false);
        pauseButton.ToggleVisibility(true);
        teleportToMouseObject.TeleportToMousePosition();
    }
}
