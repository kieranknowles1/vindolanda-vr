using UnityEngine;
using UnityEngine.AI;

public static class ComponentExtensions
{
    /// <summary>
    /// Get the first <see cref="MonoBehaviour"/> that implements <typeparamref name="T"/>, or null on failure.
    /// </summary>
    public static T GetInterface<T>(this Component go) where T: class
    {
        foreach (var cmp in go.GetComponents<MonoBehaviour>())
        {
            if (cmp is T t) return t;
        }
        return null;
    }

    /// <summary>
    /// Freeze the RigidBody in place, preventing all movement
    /// </summary>
    /// <param name="rb"></param>
    public static void SetFrozen(this Rigidbody rb, bool state)
    {
        rb.constraints = state ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
    }

    public static string FullObjectPath(this Transform t)
    {
        if (t.parent == null) return t.name;
        return $"{t.parent.FullObjectPath()}/{t.name}";
    }

    public static float GetDistance(this Transform t1, Transform t2)
    {
        return (t1.position - t2.position).magnitude;
    }

    /// <summary>
    /// Teleport an object to the target object, optionally rotating them to face the same direction
    /// </summary>
    /// <param name="target"></param>
    /// <param name="alignRotation"></param>
    public static void Teleport(this Transform obj, Transform target, bool alignRotation = true, bool snapToGround = true)
    {
        Vector3 adjustedPosition = target.position; // Fallback if raycast fails
        if (snapToGround)
        {
            if (Physics.Raycast(new Ray(target.position, Vector3.down), out var hit))
            {
                adjustedPosition = hit.point;
            }
        }

        obj.position = adjustedPosition;
        if (obj.TryGetComponent<NavMeshAgent>(out var nav))
        {
            // Forcibly move any NavMeshAgent to the target, changing its
            // navmesh if required
            nav.Warp(adjustedPosition);
        }

        if (alignRotation)
        {
            if (obj.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                // The player's head moves around the origin, so we need to adjust for that
                obj.rotation = target.rotation * Quaternion.Euler(0, -player.head.localRotation.eulerAngles.y, 0);
            }
            else
            {
                obj.rotation = target.rotation;
            }
        }
    }

    public static Vector3 AxisDirection(this Transform obj, Enums.Axis axis) => axis switch
    {
        Enums.Axis.X => obj.right,
        Enums.Axis.Y => obj.up,
        Enums.Axis.Z => obj.forward,
        _ => throw new UnreachableException()
    };
}
