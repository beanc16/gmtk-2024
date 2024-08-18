using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For kinematic rigid bodies that want to simulate
/// dynamic rigidbody behavior for collisions.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class KinematicCollisionSimulatorWithDynamicBoundaries : KinematicCollisionSimulator
{
    private void Awake()
    {
        base.Awake();

        DynamicScreenBoundaries dynamicScreenBoundaries = FindObjectOfType<DynamicScreenBoundaries>();

        foreach (GameObject wall in dynamicScreenBoundaries.Walls)
        {
            collisionObjects.Add(wall);
        }
    }
}
