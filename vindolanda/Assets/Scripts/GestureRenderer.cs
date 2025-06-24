using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class GestureRenderer : MonoBehaviour
{
    [SerializeField] XRHandShape shape;

    /// <summary>
    /// On set, updates shape to match the requested pose.
    /// Quite expensive, use sparingly
    /// </summary>
    public XRHandShape Shape
    {
        get => shape;
        set
        {
            if (shape == value) return;
            shape = value;
            SmoothUpdatePose();
        }
    }

    [SerializeField] Transform pinky;
    [SerializeField] Transform ring;
    [SerializeField] Transform middle;
    [SerializeField] Transform index;
    [SerializeField] Transform thumb;

    public float transitionTime = 1.0f;

    const float maxRotate = 40;

    Dictionary<Transform, Quaternion> GetCurrentRotations() => AllFingerNodes.ToDictionary(n => n, n => n.localRotation);
    Dictionary<Transform, Quaternion> initialRotations;

    IEnumerable<Transform> AllFingerNodes {
        get
        {
            static IEnumerable<Transform> Walk(Transform node)
            {
                do
                {
                    yield return node;
                    node = node.GetChild(0);
                } while (node.childCount > 0);
            }
            return Walk(pinky).Concat(Walk(ring)).Concat(Walk(middle)).Concat(Walk(index)).Concat(Walk(thumb));
        }
    }

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
        initialRotations = GetCurrentRotations();
        SmoothUpdatePose();
    }

    Coroutine updateCoroutine;
    void SmoothUpdatePose()
    {
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdatePositions());
    }

    IEnumerator UpdatePositions()
    {
        var starting = GetCurrentRotations();
        float time = 0;

        do
        {
            time += Time.deltaTime;
            float ratio = time / transitionTime;
            // Apply shape
            foreach (var finger in shape.fingerShapeConditions)
            {
                var node = GetFinger(finger.fingerID);
                var rotate = Quaternion.Euler(Mathf.Lerp(0, maxRotate, finger.targets[0].desired), 0, 0);

                while (node.childCount > 0)
                {
                    node.localRotation = Quaternion.Lerp(starting[node], initialRotations[node] * rotate, ratio);
                    node = node.GetChild(0);
                }
            }

            yield return new WaitForEndOfFrame();
        } while (time < transitionTime);


        // TODO: For testing
        var tmp = open;
        open = shape;
        Shape = tmp;
    }

    // TODO: For testing
    public XRHandShape open;
}
