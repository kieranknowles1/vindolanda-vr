using UnityEngine;
using UnityEngine.Serialization;

namespace Vindolanda.Animation
{
    public class NodeIK : IKDriver
    {
        public Quaternion handRotationOffset = Quaternion.Euler(-10.0f, 0.0f, 0.0f);
        [Tooltip("Position to offset hands by, applied after rotation offsets")]
        public Vector3 handPositionOffset = Vector3.back * 0.03f;

        [Header("Reference Nodes")]
        [FormerlySerializedAs("leftController")] public Transform leftHand;
        [FormerlySerializedAs("rightController")] public Transform rightHand;
        [FormerlySerializedAs("headset")] public Transform head;

        void PositionHand(AvatarIKGoal goal, Transform target)
        {
            var finalRot = target.rotation * handRotationOffset;
            SetIKPositionAndWeight(goal, 1,
                target.position + (finalRot * handPositionOffset),
                finalRot
            );
        }

        void OnAnimatorIK(int layerIndex)
        {
            PositionHand(AvatarIKGoal.LeftHand, leftHand);
            PositionHand(AvatarIKGoal.RightHand, rightHand);

            // TODO: How to do head tracking. This only covers look, not position
            SetIKLookForward(head.forward);
            SetIKFootOnGround(AvatarIKGoal.LeftFoot);
            SetIKFootOnGround(AvatarIKGoal.RightFoot);

            PositionBodyFromHead(head.position, head.rotation.eulerAngles.y);
        }
    }
}