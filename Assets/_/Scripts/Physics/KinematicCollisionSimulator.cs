using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For kinematic rigid bodies that want to simulate
/// dynamic rigidbody behavior for collisions.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class KinematicCollisionSimulator : MonoBehaviour
{
    [SerializeField, Tooltip("List of game objects with colliders for collision simulation.")]
    private List<GameObject> collisionObjects = new List<GameObject>();

    private Rigidbody2D rigidBody;
    private Collider2D collider;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        if (rigidBody.bodyType != RigidbodyType2D.Kinematic)
        {
            Debug.LogError("This script is intended for kinematic rigidbodies only.");
        }
    }

    private void FixedUpdate()
    {
        SimulateCollisions();
    }

    private void SimulateCollisions()
    {
        foreach (GameObject obj in collisionObjects)
        {
            if (obj != null)
            {
                Collider2D otherCollider = obj.GetComponent<Collider2D>();

                if (otherCollider != null)
                {
                    // Check if the collider intersects with this object's collider
                    if (IsColliding(otherCollider))
                    {
                        // Simulate collision response
                        HandleCollision(otherCollider);
                    }
                }
            }
        }
    }

    private bool IsColliding(Collider2D otherCollider)
    {
        // Check if the colliders intersect
        return collider.bounds.Intersects(otherCollider.bounds);
    }

    private void HandleCollision(Collider2D otherCollider)
    {
        // Calculate the collision normal
        Vector2 collisionNormal = GetCollisionNormal(otherCollider);
        
        // Reflect the velocity based on the collision normal
        if (collisionNormal != Vector2.zero)
        {
            // Apply a small offset to prevent sticking
            Vector2 offset = collisionNormal * 0.02f;
            transform.position += (Vector3)offset;

            // Reflect velocity
            rigidBody.velocity = Vector2.Reflect(rigidBody.velocity, collisionNormal);
        }
    }

    private Vector2 GetCollisionNormal(Collider2D otherCollider)
    {
        // Compute the collision normal based on relative positions
        Bounds thisBounds = collider.bounds;
        Bounds otherBounds = otherCollider.bounds;

        // Compute the center of the overlap region
        Vector2 overlapCenter = new Vector2(
            Mathf.Clamp(thisBounds.center.x, otherBounds.min.x, otherBounds.max.x),
            Mathf.Clamp(thisBounds.center.y, otherBounds.min.y, otherBounds.max.y)
        );

        // Compute the normal based on the direction of overlap
        Vector2 collisionNormal = Vector2.zero;
        if (
            Mathf.Abs(overlapCenter.x - thisBounds.center.x)
            < Mathf.Abs(overlapCenter.y - thisBounds.center.y)
        )
        {
            collisionNormal = overlapCenter.y > thisBounds.center.y
                ? Vector2.down
                : Vector2.up;
        }
        else
        {
            collisionNormal = overlapCenter.x > thisBounds.center.x
                ? Vector2.left
                : Vector2.right;
        }

        return collisionNormal;
    }
}
