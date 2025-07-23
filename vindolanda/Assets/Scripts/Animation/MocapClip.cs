using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace Vindolanda.Animation
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

            public static TransformState Lerp(TransformState a, TransformState b, float ratio)
            {
                return new TransformState()
                {
                    position = Vector3.Lerp(a.position, b.position, ratio),
                    rotation = Quaternion.Lerp(a.rotation, b.rotation, ratio)
                };
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
            public bool hasItem;

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
                hasItem = false;
            }

            public static HandState Lerp(HandState a, HandState b, float ratio)
            {
                return new HandState()
                {
                    transform = TransformState.Lerp(a.transform, b.transform, ratio),
                    thumb = Mathf.Lerp(a.thumb, b.thumb, ratio),
                    index = Mathf.Lerp(a.index, b.index, ratio),
                    middle = Mathf.Lerp(a.middle, b.middle, ratio),
                    ring = Mathf.Lerp(a.ring, b.ring, ratio),
                    pinky = Mathf.Lerp(a.pinky, b.pinky, ratio),
                    hasItem = a.hasItem
                };
            }
        }

        [Serializable]
        public struct Keyframe
        {
            [FormerlySerializedAs("time")]
            public float startTime;
            public TransformState head;
            public HandState leftHand;
            public HandState rightHand;

            public static Keyframe Lerp(Keyframe a, Keyframe b, float ratio)
            {
                return new Keyframe()
                {
                    head = TransformState.Lerp(a.head, b.head, ratio),
                    leftHand = HandState.Lerp(a.leftHand, b.leftHand, ratio),
                    rightHand = HandState.Lerp(a.rightHand, b.rightHand, ratio)
                };
            }
        }

        public Vector3 originPos;
        public Quaternion originRot;

        public List<Keyframe> keyframes = new();
        public GameObject leftHandItem;
        public bool leftFollowsRight;
        public GameObject rightHandItem;
        public bool rightFollowsLeft;
    }
}