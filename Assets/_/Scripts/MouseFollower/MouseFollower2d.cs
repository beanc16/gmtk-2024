using UnityEngine;

namespace Beanc16.Common.Mechanics.Follow
{
    [RequireComponent(typeof (Rigidbody2D))]
    public class MouseFollower2d : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f), Tooltip("How quickly to follow the mouse (1 follows perfectly with no delay)")]
        private float moveSpeed = 0.1f;

        private Vector3 mousePosition;
        private Collider2D collider;
        private Rigidbody2D rigidBody;
        private Vector2 position = new Vector2(0f, 0f);

        private void Awake()
        {
            /*
             * Temporarily disable collision detection. For some reason
             * Unity gets weird when the game starts and can cause the
             * follower to jitter and snap to the mouse's location on
             * game startup. So disable collision detection until later.
             */
            this.collider = GetComponent<Collider2D>();
            this.collider.enabled = false;
        }

        private void Start()
        {
            this.rigidBody = GetComponent<Rigidbody2D>();
            this.collider.enabled = true;
        }

        private void Update()
        {
            this.UpdateMousePosition();
            this.UpdateFollowerPosition();
        }

        private void FixedUpdate()
        {
            this.UpdateGameObjectPosition();
        }

        /// <summary>
        /// Update the mouse position variable based on the mouse
        /// position in world space
        /// </summary>
        private void UpdateMousePosition()
        {
            // Set the mouse's position in world space
            Vector3 positionInPixels = Input.mousePosition;
            this.mousePosition = Camera.main.ScreenToWorldPoint(positionInPixels);
        }

        /// <summary>
        /// Update the follower position variable based on the mouse
        /// position, such that it follows just behind the mouse's
        /// position based on the move speed variable
        /// </summary>
        private void UpdateFollowerPosition()
        {
            this.position = Vector2.Lerp(
                transform.position,
                this.mousePosition,
                this.moveSpeed
            );
        }

        /// <summary>
        /// Update this game object's position based on the
        /// position variable
        /// </summary>
        private void UpdateGameObjectPosition()
        {
            this.rigidBody.MovePosition(this.position);
        }
    }
}