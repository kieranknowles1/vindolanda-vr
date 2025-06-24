using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

/// <summary>
/// Render a gesture to a hand rig. Makes the following assumptions:
/// - Finger nodes follow the structure "root -> joint -> joint..."
/// - All joints have exactly zero or one children
///  - All joints in a finger rotate uniformly (not fully anatomically accurate)
/// - Bones are never deleted at runtime
/// - This component has exclusive ownership of joint rotations
/// </summary>
public class GestureRenderer : MonoBehaviour
{
    [SerializeField] XRHandShape shape;

    public XRHandShape Shape => shape;

    /// <summary>
    /// Instantly updates current shape to match the given pose. Moderately expensive, don't overuse
    /// </summary>
    /// <param name="shape"></param>
    public void SetShapeInstant(XRHandShape shape)
    {
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        this.shape = shape;
        LerpPose(initialRotations, shape, 1.0f);
    }

    Coroutine updateCoroutine;
    /// <summary>
    /// Animate current shape to match the given pose. Quite expensive, use sparingly.
    /// </summary>
    /// <param name="shape"></param>
    public void SetShapeSmooth(XRHandShape shape, float time)
    {
        this.shape = shape;
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdatePositions(shape, time));
    }

    [SerializeField] Transform pinky;
    [SerializeField] Transform ring;
    [SerializeField] Transform middle;
    [SerializeField] Transform index;
    [SerializeField] Transform thumb;

    const float maxRotate = 40;

    Dictionary<Transform, Quaternion> GetCurrentRotations() => AllFingerNodes.ToDictionary(n => n, n => n.localRotation);
    Dictionary<Transform, Quaternion> initialRotations;

    IEnumerable<Transform> AllFingerNodes {
        get
        {
            static IEnumerable<Transform> Walk(Transform node)
            {
                while (node.childCount > 0)
                {
                    yield return node;
                    node = node.GetChild(0);
                }
                yield return node; // Final joint
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
        SetShapeInstant(shape);
    }


    /// <summary>
    /// Interpolate to a pose
    /// </summary>
    /// <param name="from">Starting positions for nodes. Can be <see cref="initialRotations"/> for fully open or <see cref="GetCurrentRotations"/> for a different starting point.</param>
    /// <param name="to">The final pose to reach</param>
    /// <param name="ratio">Ratio to interpolate between</param>
    void LerpPose(Dictionary<Transform, Quaternion> from, XRHandShape to, float ratio)
    {
        foreach (var finger in to.fingerShapeConditions)
        {
            var node = GetFinger(finger.fingerID);
            var rotate = Quaternion.Euler(Mathf.Lerp(0, maxRotate, finger.targets[0].desired), 0, 0);

            while (node.childCount > 0)
            {
                node.localRotation = Quaternion.Lerp(from[node], initialRotations[node] * rotate, ratio);
                node = node.GetChild(0);
            }
        }
    }

    IEnumerator UpdatePositions(XRHandShape to, float totalTime)
    {
        var starting = GetCurrentRotations();
        float time = 0;

        do
        {
            time += Time.deltaTime;
            float ratio = Mathf.Clamp01(time / totalTime);
            // Apply shape
            LerpPose(starting, to, ratio);

            yield return new WaitForEndOfFrame();
        } while (time < totalTime);
    }
}
