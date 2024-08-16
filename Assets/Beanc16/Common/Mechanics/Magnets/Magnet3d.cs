using System.Collections.Generic;
using UnityEngine;

public class Magnet3d : MonoBehaviour
{
    [SerializeField, Range(0, 500)]
    private float magnetForce = 100;
    List<Rigidbody> caughtRigidbodies = new List<Rigidbody>();



    private void FixedUpdate()
    {
        for (int i = 0; i < caughtRigidbodies.Count; i++)
        {
            caughtRigidbodies[i].velocity = (transform.position - (caughtRigidbodies[i].transform.position + caughtRigidbodies[i].centerOfMass)) * magnetForce * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rBody = other.GetComponent<Rigidbody>();

        if (rBody)
        {
            if(!caughtRigidbodies.Contains(rBody))
            {
                caughtRigidbodies.Add(rBody);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rBody = other.GetComponent<Rigidbody>();

        if (rBody)
        {
            if (caughtRigidbodies.Contains(rBody))
            {
                caughtRigidbodies.Remove(rBody);
            }
        }
    }
}
