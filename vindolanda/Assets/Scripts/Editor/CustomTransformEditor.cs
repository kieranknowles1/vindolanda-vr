using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class CustomTransformEditor : Editor
{
    float RoundToNearest(float value, float roundTo)
    {
        return Mathf.Round(value / roundTo) * roundTo;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var transform = (Transform)target;

        if (GUILayout.Button("Snap to Grid"))
        {
            var size = EditorSnapSettings.gridSize;
            var angle = EditorSnapSettings.rotate;

            Undo.RecordObject(transform, "Snap to grid");
            transform.SetLocalPositionAndRotation(new Vector3(
                RoundToNearest(transform.localPosition.x, size.x),
                RoundToNearest(transform.localPosition.y, size.y),
                RoundToNearest(transform.localPosition.z, size.z)
            ), Quaternion.Euler(new Vector3(
                RoundToNearest(transform.localRotation.eulerAngles.x, angle),
                RoundToNearest(transform.localEulerAngles.y, angle),
                RoundToNearest(transform.localEulerAngles.z, angle)
            )));
        }
    }
}
