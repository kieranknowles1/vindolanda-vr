using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ActorAnimator : MonoBehaviour
{
    const string SpeedVariable = "SpeedMagnitude";

    Animator animator;
    Vector3 previousPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        float speed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        previousPosition = transform.position;

        animator.SetFloat(SpeedVariable, speed);
    }
}
