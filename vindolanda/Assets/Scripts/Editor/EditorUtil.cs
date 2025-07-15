using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Vindolanda.Editor
{
    public static class EditorUtil
    {
        public static List<T> GetAllScriptableObjects<T>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets("t:" + typeof(T).Name) // Returns GUIDs
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
        }
    }
}