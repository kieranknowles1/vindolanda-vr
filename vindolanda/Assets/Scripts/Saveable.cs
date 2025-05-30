using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[Serializable]
public struct SerialVector3
{
    float x, y, z;
    public static implicit operator SerialVector3(Vector3 v) => new() { x = v.x, y = v.y, z = v.z };
    public static implicit operator Vector3(SerialVector3 v) => new(v.x, v.y, v.z);
}

[Serializable]
public struct SerialQuaternion
{
    float x, y, z, w;
    public static implicit operator SerialQuaternion(Quaternion q) => new() { x = q.x, y = q.y, z = q.z, w = q.w };
    public static implicit operator Quaternion(SerialQuaternion q) => new(q.x, q.y, q.z, q.w);
}

[Serializable]
public class SaveData
{
    public byte[] id;
    public SerialVector3 position;
    public SerialQuaternion rotation;
    public SerialVector3 scale;

    public SaveData(Saveable obj)
    {
        id = obj.Guid.ToByteArray();
        position = obj.transform.position;
        rotation = obj.transform.rotation;
        scale = obj.transform.localScale;
    }
}

/// <summary>
/// Base class for anything that needs to save its state
/// By default, only saves the transform (not recursive), but can be extended
/// </summary>
public class Saveable : MonoBehaviour
{
    // Based on https://github.com/Unity-Technologies/guid-based-reference
    private Guid uid;
    [SerializeField] private byte[] uidBytes;
    public Guid Guid
    {
        get => uid;
    }

    private void Start()
    {
        uid = new Guid(uidBytes);
        if (uid == Guid.Empty)
        {
            Debug.LogError($"{this} has no GUID");
        }
        bool ok = GuidManager.Instance.Register(this);
        if (!ok)
        {
            Debug.LogError($"{this} has non-unique GUID");
        }
    }

    public SaveData Save()
    {
        return new SaveData(this);
    }

#if UNITY_EDITOR
    bool registered = false;

    private void OnValidate()
    {
        // Bit convoluted, but makes sure we are not in the prefab editor
        var mainStage = StageUtility.GetMainStageHandle();
        var currentStage = StageUtility.GetStageHandle(gameObject);
        if (currentStage != mainStage && PrefabStageUtility.GetPrefabStage(gameObject) != null) return;
        if (PrefabUtility.IsPartOfPrefabAsset(this)) return;

        uid = uidBytes.Length == 16 ? new Guid(uidBytes) : Guid.Empty;

        if (!registered && uid != Guid.Empty) registered = GuidManager.Instance.Register(this);

        if (uid == Guid.Empty || !registered)
        {
            Undo.RecordObject(this, "Assign GUID");
            uid = Guid.NewGuid();
            uidBytes = uid.ToByteArray();
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            print($"Auto assigned GUID {uid}");
            registered = GuidManager.Instance.Register(this);
        }
    }
#endif
}
