using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Hands;

[CustomEditor(typeof(PlayerHandDriver))]
public class PlayerHandDriverEditor : Editor
{
    string GetJointNodePrefix(XRHandJointID jointId)
    {
        return jointId switch
        {
            XRHandJointID.MiddleProximal => "f_middle.01",
            XRHandJointID.MiddleIntermediate => "f_middle.02",
            XRHandJointID.MiddleDistal => "f_middle.03",

            XRHandJointID.IndexProximal => "f_index.01",
            XRHandJointID.IndexIntermediate => "f_index.02",
            XRHandJointID.IndexDistal => "f_index.03",

            XRHandJointID.RingProximal => "f_ring.01",
            XRHandJointID.RingIntermediate => "f_ring.02",
            XRHandJointID.RingDistal => "f_ring.03",

            XRHandJointID.LittleProximal => "f_pinky.01",
            XRHandJointID.LittleIntermediate => "f_pinky.02",
            XRHandJointID.LittleDistal => "f_pinky.03",

            XRHandJointID.ThumbMetacarpal => "thumb.01",
            XRHandJointID.ThumbProximal => "thumb.02",
            XRHandJointID.ThumbDistal => "thumb.03",
            _ => null,
        };
    }

    PlayerHandDriver Target => (PlayerHandDriver)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Fill joints"))
        {
            var fingers = Target.transform.GetComponentsInChildren<Transform>();

            Undo.RecordObject(Target, "Fill joints");
            Target.joints ??= new();
            Target.joints.Clear();

            for (int i = (int)XRHandJointID.BeginMarker; i < (int)XRHandJointID.EndMarker; i++)
            {
                var id = (XRHandJointID)i;
                var prefix = GetJointNodePrefix(id);
                if (prefix == null) continue;
                var finger = fingers.First(f => f.name.StartsWith(prefix));
                var reference = Target.reference.jointTransformReferences.First(j => j.xrHandJointID == id).jointTransform;

                Target.joints.Add(new PlayerHandDriver.TransformRef() {
                    node = finger, reference = reference
                });
            }
        }
    }
}
