using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Based on https://github.com/Unity-Technologies/guid-based-reference
public class GuidComponent : MonoBehaviour
{
    private Guid uid;
    [SerializeField] private byte[] uidBytes;
    public Guid Guid
    {
        get => uid;
    }

    protected virtual void Start()
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

#if UNITY_EDITOR
    bool registered = false;

    private void OnValidate()
    {
        // Bit convoluted, but makes sure we are not in the prefab editor
        var mainStage = StageUtility.GetMainStageHandle();
        var currentStage = StageUtility.GetStageHandle(gameObject);
        if (currentStage != mainStage && PrefabStageUtility.GetPrefabStage(gameObject) != null) return;
        if (PrefabUtility.IsPartOfPrefabAsset(this)) return;

        uid = uidBytes?.Length == 16 ? new Guid(uidBytes) : Guid.Empty;

        if (!registered && uid != Guid.Empty) registered = GuidManager.Instance.Register(this);

        // Assign a GUID if we don't have one yet or found a collision (should only happen on copy)
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