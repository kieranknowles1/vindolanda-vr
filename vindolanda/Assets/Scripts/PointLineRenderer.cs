using System.Collections.Generic;
using UnityEngine;

[Tooltip("LineRenderer that follows a series of transforms")]
[RequireComponent(typeof(LineRenderer))]
public class PointLineRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] List<Transform> points = new();
    Vector3[] positions;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Avoid allocating an array every frame
        // Only need to do this once as points is private
        positions = new Vector3[points.Count];
        lineRenderer.positionCount = points.Count;
    }

    void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i].position;
        }
        lineRenderer.SetPositions(positions);
    }
}
