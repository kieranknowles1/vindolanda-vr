using UnityEngine;

public static class GizmoUtil
{
    public static void DrawCircle(Vector3 center, Quaternion rotation, float radius, int points = 32)
    {
        Vector3 RotatedPoint(float angle)
        {
            return center + rotation * new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }

        var prev = RotatedPoint(0);
        for (int i = 0; i <= points; i++)
        {
            float radians = i * ((Mathf.PI * 2.0f) / points);
            var next = RotatedPoint(radians);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
