using UnityEngine;

public class LockedRotation : MonoBehaviour
{
    [Tooltip("Up-down")]
    public bool lockX;
    [Tooltip("Left-right")]
    public bool lockY;
    [Tooltip("Roll")]
    public bool lockZ;  

    // LateUpdate is called once per frame - after headset position has been read
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(
            lockX ? 0 : transform.rotation.eulerAngles.x,
            lockY ? 0 : transform.rotation.eulerAngles.y,
            lockZ ? 0 : transform.rotation.eulerAngles.z
        );
    }
}
