using UnityEngine;

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
}
