using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vindolanda.Quest.Editor
{
    [CustomPropertyDrawer(typeof(QuestEvent))]
    public class QuestEventDrawer : PropertyDrawer
    {
        static readonly string[] labels = new string[]
        {
            "None",
            "Set Objective"
        };

        // Must be kept in sync with labels
        static readonly Type[] types = new Type[]
        {
            null,
            typeof(SetObjectiveEvent),
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.PrefixLabel(property.displayName);
            var index = Array.IndexOf(types, property.boxedValue?.GetType());
            int selected = EditorGUILayout.Popup(index, labels);

            if (property.boxedValue?.GetType() != types[selected])
            {
                property.boxedValue = selected switch
                {
                    0 => null,
                    1 => new SetObjectiveEvent(),
                    _ => throw new Exception(),
                };
            }


            if (property.boxedValue != null)
            {
                EditorGUI.indentLevel++;

                SerializedProperty it = property.Copy();
                SerializedProperty end = it.GetEndProperty();

                bool firstLoop = true;
                while (it.NextVisible(firstLoop) && !SerializedProperty.EqualContents(it, end))
                {
                    EditorGUILayout.PropertyField(it, true);
                    firstLoop = false;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}