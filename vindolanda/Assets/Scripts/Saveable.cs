using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public struct SerialVector3
{
    public float x, y, z;
    public static implicit operator SerialVector3(Vector3 v) => new() { x = v.x, y = v.y, z = v.z };
    public static implicit operator Vector3(SerialVector3 v) => new(v.x, v.y, v.z);
}

public struct SerialQuaternion
{
    public float x, y, z, w;
    public static implicit operator SerialQuaternion(Quaternion q) => new() { x = q.x, y = q.y, z = q.z, w = q.w };
    public static implicit operator Quaternion(SerialQuaternion q) => new(q.x, q.y, q.z, q.w);
}

public class RigidBodyData
{
    public SerialVector3 linearVelocity;
    public SerialVector3 angularVelocity;
    public int constraints;

    public RigidBodyData() { }
    public RigidBodyData(Rigidbody body)
    {
        linearVelocity = body.linearVelocity;
        angularVelocity = body.angularVelocity;
        constraints = (int)body.constraints;
    }
}

public class SaveData
{
    public int id;
    public SerialVector3 position;
    public SerialQuaternion rotation;
    public SerialVector3 scale;

    public RigidBodyData body;
    public SaveData() { }
    public SaveData(Saveable obj)
    {
        id = obj.Id;
        position = obj.transform.position;
        rotation = obj.transform.rotation;
        scale = obj.transform.localScale;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
            body = new RigidBodyData(rb);
    }
}

/// <summary>
/// Base class for anything that needs to save its state
/// By default, only saves the transform (not recursive), but can be extended
/// </summary>
public class Saveable : GuidComponent
{
    public virtual SaveData Save()
    {
        return new SaveData(this);
    }

    public virtual void Load(SaveData data)
    {
        transform.SetPositionAndRotation(data.position, data.rotation);
        transform.localScale = data.scale;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = data.body.linearVelocity;
            rb.angularVelocity = data.body.angularVelocity;
            rb.constraints = (RigidbodyConstraints)data.body.constraints;
        }
    }
}
