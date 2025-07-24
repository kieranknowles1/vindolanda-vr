using UnityEngine;
using UnityEngine.Serialization;

namespace Vindolanda.Animation
{
    public class NodeIK : IKDriver
    {
        static Quaternion HandRotationOffset = Quaternion.Euler(-10.0f, 0.0f, 0.0f);
        static Vector3 HandPositionOffset = HandRotationOffset * Vector3.back * 0.03f;

        [Header("Reference Nodes")]
        [FormerlySerializedAs("leftController")] public Transform leftController;
        [FormerlySerializedAs("rightController")] public Transform rightController;
        [FormerlySerializedAs("headset")] public Transform headset;

        void PositionHand(AvatarIKGoal goal, Transform target)
        {
            SetIKPositionAndWeight(goal, 1,
                target.position + (target.rotation * HandPositionOffset),
                target.rotation * HandRotationOffset
            );
        }

        void OnAnimatorIK(int layerIndex)
        {
            PositionHand(AvatarIKGoal.LeftHand, leftController);
            PositionHand(AvatarIKGoal.RightHand, rightController);

            // TODO: How to do head tracking. This only covers look, not position
            SetIKLookForward(headset.forward);
            SetIKFootOnGround(AvatarIKGoal.LeftFoot);
            SetIKFootOnGround(AvatarIKGoal.RightFoot);

            PositionBodyFromHead(headset.position, headset.rotation.eulerAngles.y);
        }
    }
}