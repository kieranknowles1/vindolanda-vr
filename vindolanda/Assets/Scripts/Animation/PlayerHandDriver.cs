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
        public PlayerHandDriverSettings opts;

        Vector3 FinalPositionOffset(XRHandFingerID finger)
        {
            float curl = stateTracker.GetCurl(finger);
            var raw = Vector3.Lerp(opts.PositionOffsetBase, opts.PositionOffsetMax, curl);
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
            if (deltaPosition > opts.PositionToleranceSquared) return;

            transform.rotation *= opts.SelfRotationOffset;
            transform.position += transform.rotation * opts.SelfPositionOffset;

            foreach (var joint in joints)
            {
                var finger = joint.joint.GetFinger();
                // Character skeleton uses Y-forward = straight, hands use Z-forward = straight, convert hand to skeleton
                var finalRotation = joint.reference.rotation * opts.RotationOffset;
                var finalPositionOffset = FinalPositionOffset(finger);

                var transformedPosition = joint.reference.position + (finalRotation * finalPositionOffset);
                var handToJoint = transformedPosition - transform.position;
                handToJoint = opts.RotateAroundOrigin * handToJoint;

                joint.node.SetPositionAndRotation(transform.position + handToJoint, finalRotation);
            }
        }
    }
}