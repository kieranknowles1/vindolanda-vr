using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Base class for all ScriptableObjects with GUIDs
/// </summary>
public class GuidSO : ScriptableObject, IGuidContainer
{
    private Guid uid;
    [SerializeField] private byte[] uidBytes;
    public Guid Guid => uid;

    protected virtual void OnEnable()
    {
        uid = new Guid(uidBytes);
        bool ok = GuidManager.Instance.Register(this);
        if (!ok)
        {
            Debug.LogError($"{this} has non-unique GUID");
        }
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        uid = uidBytes?.Length == 16 ? new Guid(uidBytes) : Guid.Empty;

        if (uid == Guid.Empty || !GuidManager.Instance.Register(this))
        {
            Undo.RecordObject(this, "Assign GUID");
            uid = Guid.NewGuid();
            uidBytes = uid.ToByteArray();
            Debug.Log($"Auto assigned GUID {uid}");
            GuidManager.Instance.Register(this);
        }
    }
#endif
}
