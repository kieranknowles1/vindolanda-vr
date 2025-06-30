using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationClip))]
public class CustomAnimationEditor : Editor
{
    AnimationClip Target => (AnimationClip)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Playback Speed Mult", (Target.averageSpeed.magnitude / Target.averageDuration).ToString());
    }
}
