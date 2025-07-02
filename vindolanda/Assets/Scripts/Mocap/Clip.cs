using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace Vindolanda.Mocap
{
    public class Clip : ScriptableObject
    {
        [Serializable]
        public struct TransformState
        {
            public Vector3 position;
            public Quaternion rotation;

            public TransformState(Transform transform)
            {
                position = transform.position;
                rotation = transform.rotation;
            }
        }

        [Serializable]
        public struct HandState
        {
            public TransformState transform;
            public float thumb;
            public float index;
            public float middle;
            public float ring;
            public float pinky;

            public HandState(Transform transform, XRHandShape shape)
            {
                float GetFinger(XRHandFingerID finger)
                {
                    return shape.fingerShapeConditions
                        .FirstOrDefault(f => f.fingerID == finger)
                        .targets.FirstOrDefault(t => t.shapeType == XRFingerShapeType.FullCurl).desired;
                }

                this.transform = new(transform);
                thumb = GetFinger(XRHandFingerID.Thumb);
                index = GetFinger(XRHandFingerID.Index);
                middle = GetFinger(XRHandFingerID.Middle);
                ring = GetFinger(XRHandFingerID.Ring);
                pinky = GetFinger(XRHandFingerID.Little);
            }
        }

        [Serializable]
        public struct Keyframe
        {
            public float time;
            public TransformState head;
            public HandState leftHand;
            public HandState rightHand;
        }

        public float Duration => keyframes.Last().time;
        public List<Keyframe> keyframes = new();
    }
}