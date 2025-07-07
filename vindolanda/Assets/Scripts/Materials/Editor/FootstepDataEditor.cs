using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FootstepData))]
public class FootstepDataEditor : Editor
{
    FootstepData Target => (FootstepData)target;

    List<AudioClip> GetAllSounds(string type)
    {
        var dir = AssetDatabase.GetAssetPath(Target.sourceDirectory);
        var soundDir = Directory.GetDirectories(dir).First(sub => sub.Contains(type));

        return Directory.GetFiles(soundDir)
            .Where(f => !f.EndsWith(".meta"))
            .Select(f => AssetDatabase.LoadAssetAtPath<AudioClip>(f))
            .ToList();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Uses the first subdirectory of target.sourceDirectory\n" +
            "that contains Walk/Run etc.");
        if (GUILayout.Button("Set sounds"))
        {
            Target.walk = GetAllSounds("Walk");
            Target.run = GetAllSounds("Run");
            EditorUtility.SetDirty(Target);
        }
    }
}
