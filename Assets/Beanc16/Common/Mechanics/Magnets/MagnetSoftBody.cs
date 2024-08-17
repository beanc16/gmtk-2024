using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as a magnet for SoftBody2D objects
/// </summary>
public class MagnetSoftBody : MonoBehaviour
{
    [SerializeField, Range(0, 500), Tooltip("How strongly the magnet pulls objects towards it")]
    private float force = 100;

    [SerializeField]
    private Polarity polarity = Polarity.Positive;

    List<SoftBody2D> caughtSoftBodies = new List<SoftBody2D>();

    /// <summary>
    /// Calculates the center of mass for the given SoftBody2D.
    /// </summary>
    /// <param name="softBody">The SoftBody2D to calculate the center of mass for.</param>
    /// <returns>A Vector3 representing the center of mass of the SoftBody2D.</returns>
    private Vector3 GetCenterOfMass(SoftBody2D softBody)
    {
        return new Vector3(softBody.centerOfMass.x, softBody.centerOfMass.y);
    }



    private void FixedUpdate()
    {
        for (int i = 0; i < caughtSoftBodies.Count; i++)
        {
            if (this.SoftBodyIsMagnetic(caughtSoftBodies[i]))
            {
                this.MoveSoftbodyTowardsMagnet(caughtSoftBodies[i]);
            }
        }
    }

    /// <summary>
    /// Moves a SoftBody2D object relative to the magnet based on its polarity.
    /// </summary>
    /// <param name="softBody">The SoftBody2D object to move.</param>
    private void MoveSoftbodyTowardsMagnet(SoftBody2D softBody)
    {
        // Compute the direction to move the SoftBody2D towards the magnet
        Vector3 directionToMove = softBody.transform.position + this.GetCenterOfMass(softBody);
        Vector3 positionToMove = this.GetPositionToMoveSoftBody(softBody, directionToMove);

        // Move softBody relative to the magnet (static movement speed)
        float scaler = force * Time.deltaTime;
        softBody.velocity = scaler * positionToMove;
    }

    /// <summary>
    /// Determine the position vector for moving a SoftBody2D
    /// object based on its polarity.
    /// </summary>
    /// <param name="softBody">
    /// The SoftBody2D object to determine the movement vector for.
    /// </param>
    /// <param name="directionToMove">
    /// The direction in which to move the SoftBody2D.
    /// </param>
    /// <returns>A Vector3 representing the movement direction.</returns>
    private Vector3 GetPositionToMoveSoftBody(SoftBody2D softBody, Vector3 directionToMove)
    {
        if (this.SoftBodyIsOppositePolarity(softBody))
        {
            return this.transform.position - directionToMove;
        }

        else if (this.SoftBodyIsSamePolarity(softBody))
        {
            return directionToMove - this.transform.position;
        }

        return Vector3.zero;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        SoftBody2D softBody = other.GetComponent<SoftBody2D>();

        if (!SoftBodyIsCaught(softBody) && SoftBodyIsMagnetic(softBody))
        {
            caughtSoftBodies.Add(softBody);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SoftBody2D softBody = other.GetComponent<SoftBody2D>();

        if (SoftBodyIsCaught(softBody) && SoftBodyIsMagnetic(softBody))
        {
            caughtSoftBodies.Remove(softBody);
        }
    }



    /// <summary>
    /// Checks if a given SoftBody2D object is currently caught
    /// by the magnet.
    /// </summary>
    /// <param name="softBody">The SoftBody2D object to check.</param>
    /// <returns>True if the SoftBody2D is caught, otherwise false.</returns>
    private bool SoftBodyIsCaught(SoftBody2D softBody)
    {
        if (softBody != null)
        {
            return caughtSoftBodies.Contains(softBody);
        }
        
        return false;
    }

    /// <summary>
    /// Checks if a given SoftBody2D object has magnetic properties.
    /// </summary>
    /// <param name="softBody">The SoftBody2D object to check.</param>
    /// <returns>True if the SoftBody2D is magnetic, otherwise false.</returns>
    private bool SoftBodyIsMagnetic(SoftBody2D softBody)
    {
        if (softBody != null && softBody.GetComponent<Magnetic>())
        {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Determine if a given SoftBody2D object has opposite
    /// polarity of this magnet.
    /// </summary>
    /// <param name="softBody">The SoftBody2D object to check.</param>
    /// <returns>True if the SoftBody2D has opposite polarity, otherwise false.</returns>
    private bool SoftBodyIsOppositePolarity(SoftBody2D softBody)
    {
        if (this.SoftBodyIsMagnetic(softBody))
        {
            Polarity polarity = softBody.GetComponent<Magnetic>().Polarity;

            if (
                (polarity == Polarity.Positive && this.polarity == Polarity.Negative)
                || (polarity == Polarity.Negative && this.polarity == Polarity.Positive)
                || polarity == Polarity.Any
                || this.polarity == Polarity.Any
            ) {
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Determines if a given SoftBody2D object has the same polarity as the magnet.
    /// </summary>
    /// <param name="softBody">The SoftBody2D object to check.</param>
    /// <returns>True if the SoftBody2D has the same polarity, otherwise false.</returns>
    private bool SoftBodyIsSamePolarity(SoftBody2D softBody)
    {
        if (this.SoftBodyIsMagnetic(softBody))
        {
            Polarity polarity = softBody.GetComponent<Magnetic>().Polarity;

            if (
                (polarity == Polarity.Positive && this.polarity == Polarity.Positive)
                || (polarity == Polarity.Negative && this.polarity == Polarity.Negative)
            ) {
                return true;
            }
        }
        
        return false;
    }
}
