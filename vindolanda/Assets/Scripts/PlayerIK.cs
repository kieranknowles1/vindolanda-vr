using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    static Quaternion HandRotationOffset = Quaternion.Euler(-10.0f, 0.0f, 0.0f);
    static Vector3 HandPositionOffset = HandRotationOffset * Vector3.back * 0.03f;

    private Animator animator;

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

    readonly Vector3[] targetPositions = new Vector3[4];
    public Vector3 GetTargetPosition(AvatarIKGoal goal) => targetPositions[(int)goal];

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void SetPositionAndWeight(AvatarIKGoal goal, float weight, Vector3 position, Quaternion rotation)
    {
        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);
        animator.SetIKPosition(goal, position);
        animator.SetIKRotation(goal, rotation);
        targetPositions[(int)goal] = position;
    }

    void PositionHand(AvatarIKGoal goal, Transform target)
    {
        SetPositionAndWeight(goal, 1,
            target.position + (target.rotation * HandPositionOffset),
            target.rotation * HandRotationOffset
        );
    }

    void PositionFoot(AvatarIKGoal goal)
    {
        var position = animator.GetIKPosition(goal) + (Vector3.up * maxFootRise);
        bool hit = Physics.Raycast(position, Vector3.down, out var hitInfo, maxFootRise + maxFootLower);
        if (!hit)
        {
            SetPositionAndWeight(goal, 0, Vector3.zero, Quaternion.identity);
            return;
        }

        SetPositionAndWeight(goal, 1,
            hitInfo.point,
            Quaternion.LookRotation(transform.forward, hitInfo.normal)
        );
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
        // Face the headset on the Y plane only
        transform.SetPositionAndRotation(
            headset.position + (Vector3.down * playerHeight),
            Quaternion.Euler(0, headset.rotation.eulerAngles.y, 0)
        );
    }
}
