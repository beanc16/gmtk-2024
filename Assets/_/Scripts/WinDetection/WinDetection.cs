using UnityEngine;

public class WinDetection : MonoBehaviour
{
    private ImageScroller imageScroller;

    private void Start()
    {
        imageScroller = FindObjectOfType<ImageScroller>();
        imageScroller.OnReachedEndOfCanvas.AddListener(RunWin);
    }

    public void RunWin()
    {
        imageScroller.StopMoving = true;
    }
}
