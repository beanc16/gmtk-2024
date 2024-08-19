using UnityEngine;

namespace Beanc16.Common.General
{
    public class TeleportToMouseObject : MonoBehaviour
    {
        private Vector2 MousePosition
        {
            get
            {
                // Get the mouse's position in world space
                Vector2 positionInPixels = Input.mousePosition;
                return Camera.main.ScreenToWorldPoint(positionInPixels);
            }
        }

        public void TeleportToMousePosition()
        {
            this.transform.position = MousePosition;
        }
    }
}
