using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ActorAnimator : MonoBehaviour
{
    const string SpeedVariable = "SpeedMagnitude";

    Animator animator;
    Vector3 previousPosition;

    bool halted;
    public bool Halted
    {
        get => halted;
        set {
            halted = value;
            animator.SetFloat(SpeedVariable, 0);
            previousPosition = transform.position;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        if (halted) return;
        float speed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        previousPosition = transform.position;

        animator.SetFloat(SpeedVariable, speed);
    }
}
