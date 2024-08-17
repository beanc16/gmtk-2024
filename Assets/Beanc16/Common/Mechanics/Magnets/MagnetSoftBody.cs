using System.Collections.Generic;
using UnityEngine;

public class MagnetSoftBody : MonoBehaviour
{
    [SerializeField, Range(0, 500), Tooltip("How strongly the magnet pulls objects towards it")]
    private float force = 100;

    [SerializeField]
    private Polarity polarity = Polarity.Positive;

    List<SoftBody2D> caughtSoftBodies = new List<SoftBody2D>();

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

    private void MoveSoftbodyTowardsMagnet(SoftBody2D softBody)
    {
        // Get positional helpers
        Vector3 directionToMove = softBody.transform.position + this.GetCenterOfMass(softBody);
        Vector3 positionToMove = this.GetPositionToMoveSoftBody(softBody, directionToMove);

        // Move softBody towards the magnet (static movement speed)
        float scaler = force * Time.deltaTime;
        softBody.velocity = scaler * positionToMove;
    }

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



    private bool SoftBodyIsCaught(SoftBody2D softBody)
    {
        if (softBody != null)
        {
            return caughtSoftBodies.Contains(softBody);
        }
        
        return false;
    }

    private bool SoftBodyIsMagnetic(SoftBody2D softBody)
    {
        if (softBody != null && softBody.GetComponent<Magnetic>())
        {
            return true;
        }
        
        return false;
    }

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
