using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    static Quaternion HandOffset = Quaternion.Euler(-10.0f, 0.0f, 0.0f);

    private Animator animator;

    // [Header("Animation Nodes")]
    // public Transform leftHand;
    // public Transform rightHand;
    // public Transform head;

    [Header("Reference Nodes")]
    public Transform leftController;
    public Transform rightController;
    public Transform headset;

    [Header("Offsets")]
    [Tooltip("Maximum disance to raise feet for \"crouching\"")]
    public float maxFootRise = 0.9f;
    public float maxFootLower = 0.3f;

    // TODO: Properly detect/set this
    public float playerHeight = 1.8f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void SetWeights(AvatarIKGoal goal, float value)
    {
        animator.SetIKPositionWeight(goal, value);
        animator.SetIKRotationWeight(goal, value);
    }

    void PositionHand(AvatarIKGoal goal, Transform target)
    {
        SetWeights(goal, 1);
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation * HandOffset);
    }

    void PositionFoot(AvatarIKGoal goal)
    {
        var position = animator.GetIKPosition(goal) + (Vector3.up * maxFootRise);
        bool hit = Physics.Raycast(position, Vector3.down, out var hitInfo, maxFootRise + maxFootLower);
        if (!hit)
        {
            SetWeights(goal, 0);
            return;
        }

        SetWeights(goal, 1);
        animator.SetIKPosition(goal, hitInfo.point);
        animator.SetIKRotation(goal, Quaternion.LookRotation(transform.forward, hitInfo.normal));
    }

    void OnAnimatorIK(int layerIndex)
    {
        PositionHand(AvatarIKGoal.LeftHand, leftController);
        PositionHand(AvatarIKGoal.RightHand, rightController);

        // TODO: How to do head tracking. This only covers look, not position
        animator.SetLookAtPosition(headset.position + (headset.rotation * Vector3.one));
        animator.SetLookAtWeight(1);
        PositionFoot(AvatarIKGoal.LeftFoot);
        PositionFoot(AvatarIKGoal.RightFoot);

        // Position the lower body based on the head, since we don't have foot tracking
        //bool hit = Physics.Raycast(headset.position, Vector3.down, out var hitInfo, 3.0f);
        //// As a fallback, use 6ft below the headset
        //var target = hit ? hitInfo.point : headset.position - (Vector3.down * 1.8f);
        //transform.position = target;
        // TODO: Play a crouch animation
        transform.position = headset.position + (Vector3.down * playerHeight);

        // Face the headset on the Y plane only
        transform.rotation = Quaternion.Euler(0, headset.rotation.eulerAngles.y, 0);
    }
}
