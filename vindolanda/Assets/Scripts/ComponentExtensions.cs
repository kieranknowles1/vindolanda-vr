using Mono.Cecil;
using System.Collections.Generic;
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
            // The player's head moves around the origin, so we need to adjust for that
            // FIXME: This breaks subtitle and inventory positioning
            //transform.rotation = target.rotation * Quaternion.Euler(0, -head.localRotation.eulerAngles.y, 0);
        }
    }

#if UNITY_EDITOR
    public static List<T> GetAllScriptableObjects<T>() where T : ScriptableObject
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
        List<T> output = new()
        {
            Capacity = guids.Length
        };
        foreach (var guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            output.Add(asset);
        }
        return output;
    }
#endif
}
