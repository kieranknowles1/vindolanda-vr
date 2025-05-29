using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    public List<Transform> points;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < points.Count; i++)
        {
            Transform current = points[i];
            Transform next = (i + 1) < points.Count ? points[i + 1] : points[0];

            var nextToCurrentDir = (next.position - current.position).normalized;

            // Use a cube to make the direction clearer. Easier than drawing a triangle
            Gizmos.DrawLine(current.position, next.position);
            Gizmos.DrawWireCube(next.position - nextToCurrentDir, Vector3.one);
        }
    }
}
