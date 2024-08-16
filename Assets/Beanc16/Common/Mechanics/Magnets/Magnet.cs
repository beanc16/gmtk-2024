using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField, Range(0, 500), Tooltip("How strongly the magnet pulls objects towards it")]
    private float force = 100;

    [SerializeField]
    private Polarity polarity = Polarity.Positive;

    List<Rigidbody2D> caughtRigidbodies = new List<Rigidbody2D>();

    private Vector3 GetCenterOfMass(Rigidbody2D rBody)
    {
        return new Vector3(rBody.centerOfMass.x, rBody.centerOfMass.y);
    }



    private void FixedUpdate()
    {
        for (int i = 0; i < caughtRigidbodies.Count; i++)
        {
            if (this.RigidbodyIsMagnetic(caughtRigidbodies[i]))
            {
                this.MoveRigidbodyTowardsMagnet(caughtRigidbodies[i]);
            }
        }
    }

    private void MoveRigidbodyTowardsMagnet(Rigidbody2D rBody)
    {
        // Get positional helpers
        Vector3 directionToMove = rBody.transform.position + this.GetCenterOfMass(rBody);
        Vector3 positionToMove = this.GetPositionToMoveRigidBody(rBody, directionToMove);

        // Move rBody towards the magnet (static movement speed)
        float scaler = force * Time.deltaTime;
        rBody.velocity = scaler * positionToMove;
    }

    private Vector3 GetPositionToMoveRigidBody(Rigidbody2D rBody, Vector3 directionToMove)
    {
        if (this.RigidbodyIsOppositePolarity(rBody))
        {
            return this.transform.position - directionToMove;
        }

        else if (this.RigidbodyIsSamePolarity(rBody))
        {
            return directionToMove - this.transform.position;
        }

        return Vector3.zero;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rBody = other.GetComponent<Rigidbody2D>();

        if (!RigidbodyIsCaught(rBody) && RigidbodyIsMagnetic(rBody))
        {
            caughtRigidbodies.Add(rBody);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rBody = other.GetComponent<Rigidbody2D>();

        if (RigidbodyIsCaught(rBody) && RigidbodyIsMagnetic(rBody))
        {
            caughtRigidbodies.Remove(rBody);
        }
    }



    private bool RigidbodyIsCaught(Rigidbody2D rBody)
    {
        if (rBody)
        {
            return caughtRigidbodies.Contains(rBody);
        }
        
        return false;
    }

    private bool RigidbodyIsMagnetic(Rigidbody2D rBody)
    {
        if (rBody)
        {
            if (rBody.GetComponent<Magnetic>())
            {
                return true;
            }
        }
        
        return false;
    }

    private bool RigidbodyIsOppositePolarity(Rigidbody2D rBody)
    {
        if (this.RigidbodyIsMagnetic(rBody))
        {
            Polarity polarity = rBody.GetComponent<Magnetic>().Polarity;

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

    private bool RigidbodyIsSamePolarity(Rigidbody2D rBody)
    {
        if (this.RigidbodyIsMagnetic(rBody))
        {
            Polarity polarity = rBody.GetComponent<Magnetic>().Polarity;

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
