using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Beanc16.Common.General;

/// <summary>
/// Represents a soft body with points connected by joints
/// to simulate a flexible (rather than rigid) shape.
/// </summary>
[RequireComponent(typeof (PolygonCollider2D))]
[RequireComponent(typeof (Follow))]
public class SoftBody2D : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;

    private PolygonCollider2D collider;

    [SerializeField, Range(3, 20), Tooltip("Number of points to give this SoftBody")]
    private int numberOfPoints = 10;

    [SerializeField]
    private List<GameObject> points = new List<GameObject>();

    [SerializeField]
    private Sprite sprite;
    private Material spriteMaterial;

    [SerializeField]
    private GameObject pointPrefab;

    private GameObject centerPoint;

    /// <summary>
    /// Gets the radius of the soft body based on its scale.
    /// </summary>
    public float Radius
    {
        get => this.transform.localScale.x / 2f;
    }

    /// <summary>
    /// Calculates the center of mass of the soft body based
    /// on the mass and position of its points.
    /// </summary>
    public Vector3 centerOfMass // (camelCase to match RigidBody2D's casing)
    {
        get
        {
            Vector2 centerOfMass = Vector3.zero;
            float totalMass = 0f;

            foreach (var point in points)
            {
                Rigidbody2D rb = point.GetComponent<Rigidbody2D>();
                centerOfMass += rb.worldCenterOfMass * rb.mass;
                totalMass += rb.mass;
            }

            return centerOfMass / totalMass;
        }
    }

    /// <summary>
    /// Sets the velocity for each point's Rigidbody2D.
    /// </summary>
    public Vector2 velocity // (camelCase to match RigidBody2D's casing)
    {
        set
        {
            foreach (var point in points)
            {
                Rigidbody2D rb = point.GetComponent<Rigidbody2D>();
                rb.velocity = value;
            }
        }
    }

    /// <summary>
    /// Applies a force to each point's Rigidbody2D.
    /// </summary>
    /// <param name="force">The force to apply.</param>
    /// <param name="mode">The mode of the force application.</param>
    public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
    {
        // Apply force to each point's Rigidbody2D
        foreach (var point in points)
        {
            Rigidbody2D rb = point.GetComponent<Rigidbody2D>();
            rb.AddForce(force, mode);
        }
    }

    public void MovePosition(Vector2 newPosition)
    {
        // Calculate the offset from the current position to the new position
        Vector2 offset = newPosition - (Vector2)this.transform.position;

        // Update each point's position based on the calculated offset
        centerPoint.transform.position += (Vector3)offset;
        foreach (GameObject point in points)
        {
            point.transform.position += (Vector3)offset;
        }

        // Update the mesh and collider after moving the points
        UpdateMesh();
        UpdateColliderPath();
    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        InitializeVertices();
        InitializeJoints();
        InitializeFollowers();
        UpdateMesh();
    }

    /// <summary>
    /// Initializes the vertices and points of the soft body.
    /// </summary>
    private void InitializeVertices()
    {
        vertices = new Vector3[numberOfPoints];

        // Create and place central point
        centerPoint = Instantiate(
            pointPrefab,
            this.transform.position,
            Quaternion.identity
        );
        Rigidbody2D centerPointRb = centerPoint.GetComponent<Rigidbody2D>();
        centerPoint.transform.SetParent(this.transform);

        // Initialize points
        for (int index = 0; index < numberOfPoints; index++)
        {
            // Calculate position of vertex
            float angle = index * Mathf.PI * 2f / numberOfPoints;
            Vector3 position = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0
            ) * this.Radius;

            // Instantiate points at vertex
            GameObject obj = Instantiate(
                pointPrefab,
                this.transform.position + position,
                Quaternion.identity
            );
            obj.transform.SetParent(this.transform);
            points.Add(obj);

            // Update vertices
            vertices[index] = obj.transform.localPosition - this.transform.position;
        }

        collider = GetComponent<PolygonCollider2D>();
        UpdateColliderPath();
    }

    /// <summary>
    /// Initializes joints to connect the points and central point.
    /// </summary>
    private void InitializeJoints()
    {
        // Connect points with hinge joints to form a closed loop
        for (int index = 0; index < points.Count; index++)
        {
            SpringJoint2D jointToAdjacentPoint = points[index].AddComponent<SpringJoint2D>();

            // Connect the last point to the first point
            if (index == points.Count - 1)
            {
                jointToAdjacentPoint.connectedBody = points[0].GetComponent<Rigidbody2D>();
            }

            // Connect this point to the next point
            else
            {
                jointToAdjacentPoint.connectedBody = points[index + 1].GetComponent<Rigidbody2D>();
            }

            jointToAdjacentPoint.autoConfigureDistance = false;
            jointToAdjacentPoint.distance = Radius * 0.8f; // Maintain close proximity
            jointToAdjacentPoint.dampingRatio = 0.2f; // Moderate damping
            jointToAdjacentPoint.frequency = 2f; // Higher frequency for stronger connection

            // Connect to the central point using a FixedJoint2D
            SpringJoint2D jointToCenter = points[index].AddComponent<SpringJoint2D>();
            jointToCenter.connectedBody = centerPoint.GetComponent<Rigidbody2D>();
            jointToCenter.autoConfigureDistance = false;
            jointToCenter.distance = Radius * 0.9f; // Slightly less room to stretch
            jointToCenter.dampingRatio = 0.6f; // Stronger damping to prevent collapse
            jointToCenter.frequency = 2.5f; // Higher frequency for more stability
        }

        // Add diagonal spring joints for internal stability
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 2; j < points.Count; j++)
            {
                SpringJoint2D diagonalJoint = points[i].AddComponent<SpringJoint2D>();
                diagonalJoint.connectedBody = points[j].GetComponent<Rigidbody2D>();
                diagonalJoint.autoConfigureDistance = false;
                diagonalJoint.distance = Vector3.Distance(
                    points[i].transform.position,
                    points[j].transform.position
                );
                diagonalJoint.dampingRatio = 0.3f;
                diagonalJoint.frequency = 2f;
            }
        }
    }

    private void InitializeFollowers()
    {
        // Set all child followers to follow the center point
        Follow[] followers = GetComponentsInChildren<Follow>();

        foreach (Follow follower in followers)
        {
            follower.ObjectToFollow = centerPoint;
        }
    }

    private void FixedUpdate()
    {
        // Update vertex positions based on points' positions
        for (int index = 0; index < vertices.Length; index++)
        {
            vertices[index] = points[index].transform.localPosition;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        // Update collider path to match the current positions of points
        UpdateColliderPath();
    }

    /// <summary>
    /// Updates the mesh to reflect the current vertices and
    /// creates triangles for rendering.
    /// </summary>
    private void UpdateMesh()
    {
        mesh.Clear();

        // Create triangles
        int[] triangles = new int[(numberOfPoints - 2) * 3];
        for (int index = 0; index < numberOfPoints - 2; index++)
        {
            triangles[index * 3] = 0;
            triangles[index * 3 + 1] = index + 1;
            triangles[index * 3 + 2] = index + 2;
        }

        // Set mesh properties
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    /// <summary>
    /// Updates the PolygonCollider2D path to match the
    /// current positions of the points.
    /// </summary>
    private void UpdateColliderPath()
    {
        // Update collider path to match the current positions of points
        List<Vector2> colliderPoints = new List<Vector2>();

        foreach (var point in points)
        {
            Vector2 position = new Vector2(
                point.transform.position.x,
                point.transform.position.y
            );
            colliderPoints.Add(position);
        }

        collider.SetPath(0, colliderPoints);
    }

    private void OnDrawGizmos()
    {
        // Draw the soft body center
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(centerOfMass, 0.1f);

        // Draw the screen center
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(screenCenter), 0.1f);
    }
}
