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
            var position = EditorSnapSettings.gridSize;
            var angle = EditorSnapSettings.rotate;
            var scale = EditorSnapSettings.scale;

            Undo.RecordObject(transform, "Snap to grid");
            transform.SetLocalPositionAndRotation(new Vector3(
                RoundToNearest(transform.localPosition.x, position.x),
                RoundToNearest(transform.localPosition.y, position.y),
                RoundToNearest(transform.localPosition.z, position.z)
            ), Quaternion.Euler(new Vector3(
                RoundToNearest(transform.localEulerAngles.x, angle),
                RoundToNearest(transform.localEulerAngles.y, angle),
                RoundToNearest(transform.localEulerAngles.z, angle)
            )));
            transform.localScale = new Vector3(
                RoundToNearest(transform.localScale.x, scale),
                RoundToNearest(transform.localScale.y, scale),
                RoundToNearest(transform.localScale.z, scale)
            );
        }

        if (GUILayout.Button("Randomise yaw"))
        {
            Undo.RecordObject(transform, "Randomise yaw");
            transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, Random.Range(0, 360));
        }
    }
}
