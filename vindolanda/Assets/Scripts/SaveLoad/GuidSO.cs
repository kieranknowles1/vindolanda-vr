using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Base class for all ScriptableObjects with GUIDs
/// </summary>
public class GuidSO : ScriptableObject, IGuidContainer
{
    [SerializeField] private int id;
    public int Id => id;

    public bool IsDestroyed => this == null;

    protected virtual void OnEnable()
    {
        bool ok = GuidManager.Instance.Register(this);
        if (!ok)
        {
            Debug.LogError($"{this} has non-unique GUID");
        }
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (id == IGuidContainer.NoId || !GuidManager.Instance.Register(this))
        {
            Undo.RecordObject(this, "Assign GUID");
            id = GuidManager.Instance.Allocate();
            Debug.Log($"Auto assigned ID {id}");
            GuidManager.Instance.Register(this);
        }
    }
#endif
}
