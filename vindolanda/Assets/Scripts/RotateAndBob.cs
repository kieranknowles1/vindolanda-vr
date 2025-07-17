using UnityEngine;

public class RotateAndBob : MonoBehaviour
{
    [Header("Rotation")]
    public Enums.Axis axis;
    public float rotateSpeed = 60;
    [Header("Bobbing")]
    public float bobSpeed = 2;
    public Vector3 bobDistance = Vector3.up;

    Vector3 startPosition;
    Quaternion startRotation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = rotateSpeed * Time.time;

        transform.SetLocalPositionAndRotation(
            startPosition + (bobDistance * Mathf.Sin(Time.time * bobSpeed)),
            startRotation * Quaternion.Euler(
                axis == Enums.Axis.X ? rotation : 0,
                axis == Enums.Axis.Y ? rotation : 0,
                axis == Enums.Axis.Z ? rotation : 0
            )
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        GizmoUtil.DrawCircle(transform.position, transform.AxisDirection(axis), 1.0f);
        Gizmos.DrawLine(transform.position - bobDistance, transform.position + bobDistance);
    }
}
