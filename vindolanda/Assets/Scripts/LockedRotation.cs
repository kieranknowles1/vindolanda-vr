using UnityEngine;

[Tooltip("Lock one or more axes to zero, otherwise inherit parent rotation")]
public class LockedRotation : MonoBehaviour
{
    [Tooltip("Up-down")]
    public bool lockX;
    [Tooltip("Left-right")]
    public bool lockY;
    [Tooltip("Roll")]
    public bool lockZ;  

    // Called once per frame - after headset position has been read
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(
            lockX ? 0 : transform.parent.rotation.eulerAngles.x,
            lockY ? 0 : transform.parent.rotation.eulerAngles.y,
            lockZ ? 0 : transform.parent.rotation.eulerAngles.z
        );
    }
}
