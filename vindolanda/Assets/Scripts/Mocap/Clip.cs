using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vindolanda.Mocap
{
    public class Clip : ScriptableObject
    {
        [Serializable]
        public struct TransformState
        {
            public Vector3 position;
            public Quaternion rotation;
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
        }

        [Serializable]
        public struct Keyframe
        {
            public float time;
            public TransformState head;
            public HandState leftHand;
            public HandState rightHand;
        }

        public List<Keyframe> keyframes = new();
    }
}