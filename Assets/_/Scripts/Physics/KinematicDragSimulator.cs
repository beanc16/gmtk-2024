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

    [SerializeField, Tooltip("Apply the object's scale as mass to the object's drag.")]
    private bool applyMass = true;

    private Rigidbody2D rigidBody;

    /// <summary>
    /// An approximation of mass based on the scale of the object.
    /// Assuming the mass is proportional to the volume (scale^3) of the object.
    /// Adjust the multiplier if necessary to fit the expected mass range
    /// </summary>
    private float Mass
    {
        get => transform.localScale.x * transform.localScale.y * transform.localScale.z;
    }

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
            // Determine the mass to use for drag calculations
            float mass = applyMass ? Mass : 1f;

            // Apply drag force proportional to the the mass and square of the velocity
            Vector2 dragForce = -linearDragCoefficient
                * mass
                * velocity.normalized
                * velocity.sqrMagnitude;

            rigidBody.velocity += dragForce * Time.fixedDeltaTime / mass;
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
