using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For kinematic rigid bodies that want to simulate
/// dynamic rigidbody behavior for linear drag.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class KinematicDragSimulator : MonoBehaviour
{
    [SerializeField, Range(0f, 1000000f), Tooltip("Used to slow down the object. Applies to positional movement. The higher the drag the more the object slows down.")]
    private float linearDragCoefficient = 1f;

    [SerializeField, Range(0f, 1f), Tooltip("How much to lower velocity by on each FixedUpdate.")]
    private float velocityMultiplier = 1f;

    private Rigidbody2D rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        if (rigidBody.bodyType != RigidbodyType2D.Kinematic)
        {
            Debug.LogError("This script is intended for kinematic rigidbodies only.");
        }
    }

    private void FixedUpdate()
    {
        SimulateLinearDrag();
    }

    private void SimulateLinearDrag()
    {
        // Calculate the drag force based on the current velocity
        Vector2 velocity = rigidBody.velocity;
        if (velocity.magnitude > 0f)
        {
            // Apply drag force proportional to the square of the velocity
            Vector2 dragForce = -linearDragCoefficient * velocity.normalized * velocity.sqrMagnitude;
            rigidBody.velocity += dragForce * Time.fixedDeltaTime;
        }

        // Clamp velocity to prevent it from going negative
        ClampVelocity();
    }

    private void ClampVelocity()
    {
        if (rigidBody.velocity.magnitude < 0.001f)
        {
            rigidBody.velocity = Vector2.zero;
        }

        else
        {
            rigidBody.velocity *= velocityMultiplier;
        }
    }
}
