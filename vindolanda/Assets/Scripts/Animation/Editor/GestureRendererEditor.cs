using UnityEditor;
using UnityEngine;
using Vindolanda.Editor;

namespace Vindolanda.Animation.Editor
{
    [CustomEditor(typeof(GestureRenderer))]
    public class GestureRendererEditor : CustomEditor<GestureRenderer>
    {
        Transform ChildNameLike(string contains)
        {
            for (int i = 0; i < Target.transform.childCount; i++) {
                var child = Target.transform.GetChild(i);
                if (child.name.Contains(contains)) return child;
            }
            return null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Find Joints"))
            {
                Target.pinky = ChildNameLike("pinky");
                Target.ring = ChildNameLike("ring");
                Target.middle = ChildNameLike("middle");
                Target.index = ChildNameLike("index");
                Target.thumb = ChildNameLike("thumb");
            }
        }
    }
}