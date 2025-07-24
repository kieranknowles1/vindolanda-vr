using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands.Gestures;

namespace Vindolanda.Animation
{

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
        [SerializeField] Clip.HandState? shape;
        bool overrideSkeleton;

        public Clip.HandState? Shape => shape;

        public void SetShapeInstant(Clip.HandState shape)
        {
            if (updateCoroutine != null) StopCoroutine(updateCoroutine);
            this.shape = shape;
            LerpPose(initialRotations, shape, 1.0f);
        }

        /// <summary>
        /// Instantly updates current shape to match the given pose. Moderately expensive, don't overuse
        /// </summary>
        /// <param name="shape"></param>
        public void SetShapeInstant(XRHandShape shape)
        {
            var state = new Clip.HandState(transform, shape);
            SetShapeInstant(state);
        }

        Coroutine updateCoroutine;

        public void SetShapeSmooth(Clip.HandState shape, float time)
        {
            this.shape = shape;
            if (updateCoroutine != null) StopCoroutine(updateCoroutine);
            updateCoroutine = StartCoroutine(UpdatePositions(shape, time));
        }

        /// <summary>
        /// Animate current shape to match the given pose. Quite expensive, use sparingly.
        /// </summary>
        /// <param name="shape"></param>
        public void SetShapeSmooth(XRHandShape shape, float time)
        {
            var state = new Clip.HandState(transform, shape);
            SetShapeSmooth(state, time);
        }

        public Transform pinky;
        public Transform ring;
        public Transform middle;
        public Transform index;
        public Transform thumb;

        const float maxRotate = 40;

        Dictionary<Transform, Quaternion> GetCurrentRotations() => AllFingerNodes.ToDictionary(n => n, n => n.localRotation);
        Dictionary<Transform, Quaternion> initialRotations;

        IEnumerable<Transform> AllFingerNodes
        {
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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            initialRotations = GetCurrentRotations();
            if (shape != null)
            {
                SetShapeInstant(shape.Value);
            }
            overrideSkeleton = GetComponentInParent<Animator>() != null;
            print($"{name}: {overrideSkeleton}");
        }

        private void LateUpdate()
        {
            if (overrideSkeleton && shape != null) SetShapeInstant(shape.Value);
        }


        /// <summary>
        /// Interpolate to a pose
        /// </summary>
        /// <param name="from">Starting positions for nodes. Can be <see cref="initialRotations"/> for fully open or <see cref="GetCurrentRotations"/> for a different starting point.</param>
        /// <param name="to">The final pose to reach</param>
        /// <param name="ratio">Ratio to interpolate between</param>
        void LerpPose(Dictionary<Transform, Quaternion> from, Clip.HandState to, float ratio)
        {
            void UpdateFinger(Transform node, float endCurl)
            {
                var rotate = Quaternion.Euler(Mathf.Lerp(0, maxRotate, endCurl), 0, 0);

                while (node.childCount > 0)
                {
                    node.localRotation = Quaternion.Lerp(from[node], initialRotations[node] * rotate, ratio);
                    node = node.GetChild(0);
                }
            }
            UpdateFinger(thumb, to.thumb);
            UpdateFinger(index, to.index);
            UpdateFinger(middle, to.middle);
            UpdateFinger(ring, to.ring);
            UpdateFinger(pinky, to.pinky);
        }

        IEnumerator UpdatePositions(Clip.HandState to, float totalTime)
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
}