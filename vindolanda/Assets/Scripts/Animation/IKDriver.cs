using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKDriver : MonoBehaviour
{
    [Header("Offsets")]
    [Tooltip("Maximum disance to raise feet for \"crouching\"")]
    public float maxFootRise = 0.9f;
    public float maxFootLower = 0.3f;

    // TODO: Properly detect/set this
    public float height = 1.8f;

    protected Animator Animator { get; private set; }

    readonly Vector3[] targetPositions = new Vector3[4];
    public Vector3 GetTargetPosition(AvatarIKGoal goal) => targetPositions[(int)goal];

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    protected void SetIKPositionAndWeight(AvatarIKGoal goal, float weight, Vector3 position, Quaternion rotation)
    {
        Animator.SetIKPositionWeight(goal, weight);
        Animator.SetIKRotationWeight(goal, weight);
        Animator.SetIKPosition(goal, position);
        Animator.SetIKRotation(goal, rotation);
        targetPositions[(int)goal] = position;
    }

    protected void SetIKFootOnGround(AvatarIKGoal goal)
    {
        var position = Animator.GetIKPosition(goal) + (Vector3.up * maxFootRise);
        bool hit = Physics.Raycast(position, Vector3.down, out var hitInfo, maxFootRise + maxFootLower);
        if (!hit)
        {
            SetIKPositionAndWeight(goal, 0, Vector3.zero, Quaternion.identity);
            return;
        }

        SetIKPositionAndWeight(goal, 1,
            hitInfo.point,
            Quaternion.LookRotation(transform.forward, hitInfo.normal)
        );
    }

    protected void SetIKLookForward(Vector3 forward)
    {
        var head = Animator.GetBoneTransform(HumanBodyBones.Head);
        Animator.SetLookAtPosition(head.position + forward);
        Animator.SetLookAtWeight(1);
    }

    protected void PositionBodyFromHead(Vector3 headPosition, float yaw)
    {
        // Position the body based on the head. IK will make the actor crouch, but we don't
        // currently have an animation so it will look silly in many cases
        // TODO: Play a crouch animation
        // Face the headset on the Y plane only
        transform.SetPositionAndRotation(
            headPosition + (Vector3.down * height),
            Quaternion.Euler(0, yaw, 0)
        );
    }
}
