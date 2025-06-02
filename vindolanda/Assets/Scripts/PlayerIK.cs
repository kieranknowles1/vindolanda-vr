using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    private Animator animator;

    // [Header("Animation Nodes")]
    // public Transform leftHand;
    // public Transform rightHand;
    // public Transform head;

    [Header("Reference Nodes")]
    public Transform leftController;
    public Transform rightController;
    public Transform headset;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void SetTarget(AvatarIKGoal goal, Transform target)
    {
        animator.SetIKPositionWeight(goal, 1);
        animator.SetIKRotationWeight(goal, 1);
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }

    void OnAnimatorIK(int layerIndex)
    {
        SetTarget(AvatarIKGoal.LeftHand, leftController);
        SetTarget(AvatarIKGoal.RightHand, rightController);

        // TODO: How to do head tracking
    }
}
