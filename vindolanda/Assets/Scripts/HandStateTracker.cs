using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

[RequireComponent(typeof(XRHandTrackingEvents))]
public class HandStateTracker : MonoBehaviour
{
    XRHandTrackingEvents events;

    readonly float[] curls = new float[5];

    public float GetCurl(XRHandFingerID finger) => curls[(int)finger];

    private void Awake()
    {
        events = GetComponent<XRHandTrackingEvents>();
    }

    private void OnEnable()
    {
        events.jointsUpdated.AddListener(OnPoseUpdated);
    }

    private void OnDisable()
    {
        events.jointsUpdated.RemoveListener(OnPoseUpdated);
    }

    void OnPoseUpdated(XRHandJointsUpdatedEventArgs evnt)
    {
        float GetFingerValue(XRHandFingerID id)
        {
            var shape = evnt.hand.CalculateFingerShape(id, XRFingerShapeTypes.FullCurl);
            shape.TryGetFullCurl(out var result);
            return result;
        }
        for (int i = 0; i < curls.Length; i++)
        {
            var id = (XRHandFingerID)i;
            curls[i] = GetFingerValue(id);
        }
    }
}