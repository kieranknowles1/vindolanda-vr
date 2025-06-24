using UnityEngine;

public class RotateAndBob : MonoBehaviour
{
    public float rotateSpeed = 60;
    public float bobSpeed = 2;
    public Vector3 bobDistance = Vector3.up;

    Vector3 startPosition;
    Quaternion startRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + (bobDistance * Mathf.Sin(Time.time * bobSpeed));
        transform.rotation = startRotation * Quaternion.Euler(0, rotateSpeed * Time.time, 0);
    }
}
