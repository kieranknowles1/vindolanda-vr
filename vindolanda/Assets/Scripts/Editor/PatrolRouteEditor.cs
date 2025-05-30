using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatrolRoute))]
public class PatrolRouteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var route = (PatrolRoute)target;

        if (GUILayout.Button("Add Point"))
        {
            var end = route.points.Count > 0 ? route.points[route.points.Count - 1].position : route.transform.position;

            var newObj = new GameObject($"Point{route.points.Count}");
            newObj.transform.parent = route.transform;
            newObj.transform.position = end;
            route.points.Add(newObj.transform);

            Selection.activeObject = newObj;
        }

        if (route.points.Any(p => p == null) && GUILayout.Button("Fix nulls"))
        {
            route.points = route.points.Where(p => p != null).ToList();
        }
    }
}
