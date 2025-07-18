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

    [Serializable]
    public struct TransformRef
    {
        public Transform reference;
        public Transform node;
    }

    public XRHandSkeletonDriver reference;

    public List<TransformRef> joints;

    // Called after animator has set positions, but before rigging. Gives us an oppurtunity to override them.
    void LateUpdate()
    {
        foreach (var joint in joints)
        {
            // Character skeleton uses Y-forward = straight, hands use Z-forward = straight, convert hand to skeleton

            joint.node.SetPositionAndRotation(joint.reference.position, joint.reference.rotation * RotationOffset);
        }
    }
}
