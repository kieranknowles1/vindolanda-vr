using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>
/// Component to set the player model's hand/finger positions based on controller references
/// </summary>
public class PlayerHandDriver : MonoBehaviour
{
    static Quaternion RotationOffset = Quaternion.Euler(90.0f, 0, 0);
    const float PositionToleranceSquared = 0.01f * 0.01f;

    PlayerIK ik;

    private void Awake()
    {
        ik = GetComponentInParent<PlayerIK>();
    }

    [Serializable]
    public struct TransformRef
    {
        public Transform reference;
        public Transform node;
    }

    public XRHandSkeletonDriver reference;

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
            // Character skeleton uses Y-forward = straight, hands use Z-forward = straight, convert hand to skeleton

            joint.node.SetPositionAndRotation(joint.reference.position, joint.reference.rotation * RotationOffset);
        }
    }
}
