using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class GestureRenderer : MonoBehaviour
{
    [SerializeField] XRHandShape shape;

    [SerializeField] Transform pinky;
    [SerializeField] Transform ring;
    [SerializeField] Transform middle;
    [SerializeField] Transform index;
    [SerializeField] Transform thumb;

    const float maxRotate = 40;

    Transform GetFinger(XRHandFingerID finger)
    {
        return finger switch
        {
            XRHandFingerID.Little => pinky,
            XRHandFingerID.Ring => ring,
            XRHandFingerID.Middle => middle,
            XRHandFingerID.Index => index,
            XRHandFingerID.Thumb => thumb,
            _ => throw new System.NotImplementedException()
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (shape == null) return;

        foreach (var finger in shape.fingerShapeConditions)
        {
            var node = GetFinger(finger.fingerID);
            var rotate = Quaternion.Euler(Mathf.Lerp(0, maxRotate, finger.targets[0].desired), 0, 0);

            do
            {
                node.rotation *= rotate;
                node = node.GetChild(0);
            } while (node.childCount > 0);
        }
    }
}
