using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Vindolanda.Animation
{

    /// <summary>
    /// Component to set the player model's hand/finger positions based on controller references
    /// </summary>
    public class PlayerHandDriver : MonoBehaviour
    {
        // XR hands use a different coordinate system to the default skeleton :(
        // Convert between them here
        static Quaternion RotationOffset = Quaternion.Euler(90.0f, 0, 0);
        // Don't deform fingers if the arm is stretched too far. Most noticable for players with
        // long arms, or if leaning back
        const float PositionToleranceSquared = 0.01f * 0.01f;

        // Lerp between these offsets to further reduce deformation
        static readonly Vector3 PositionOffsetBase = new(0.0135f, -0.0115f, 0);
        // Yes, that's a 3mm offset. Even that level of error is noticable when applied to the knuckles
        static readonly Vector3 PositionOffsetMax = PositionOffsetBase + new Vector3(-0.005f, 0, 0.003f);

        Vector3 FinalPositionOffset(XRHandFingerID finger)
        {
            float curl = stateTracker.GetCurl(finger);
            var raw = Vector3.Lerp(PositionOffsetBase, PositionOffsetMax, curl);
            raw.x *= hand == AvatarIKGoal.LeftHand ? -1 : 1;
            return raw;
        }

        NodeIK ik;

        private void Awake()
        {
            ik = GetComponentInParent<NodeIK>();
            stateTracker = reference.GetComponent<HandStateTracker>();
        }

        [Serializable]
        public struct TransformRef
        {
            public XRHandJointID joint;
            public Transform reference;
            public Transform node;
        }

        public XRHandSkeletonDriver reference;
        HandStateTracker stateTracker;

        public List<TransformRef> joints;
        public AvatarIKGoal hand;

        // Called after animator has set positions, but before skinning. Gives us an oppurtunity to override them.
        // Not exactly clean, but it works
        void LateUpdate()
        {
            // We're stretched too far out, return early to avoid deforming the hand
            var targetPosition = ik.GetTargetPosition(hand);
            float deltaPosition = Vector3.SqrMagnitude(targetPosition - transform.position);
            if (deltaPosition > PositionToleranceSquared) return;

            foreach (var joint in joints)
            {
                var finger = joint.joint.GetFinger();
                // Character skeleton uses Y-forward = straight, hands use Z-forward = straight, convert hand to skeleton
                var finalRotation = joint.reference.rotation * RotationOffset;
                var finalPositionOffset = FinalPositionOffset(finger);

                joint.node.SetPositionAndRotation(joint.reference.position + (finalRotation * finalPositionOffset), finalRotation);
            }
        }
    }
}