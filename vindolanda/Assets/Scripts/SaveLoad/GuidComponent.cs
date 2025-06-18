#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

// Based on https://github.com/Unity-Technologies/guid-based-reference
public class GuidComponent : MonoBehaviour, IGuidContainer
{
    [SerializeField] private int id;
    public int Id => id;

    public bool IsDestroyed => this == null;

    protected virtual void Start()
    {
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

        if (!registered && id != IGuidContainer.NoId) registered = GuidManager.Instance.Register(this);

        // Assign a GUID if we don't have one yet or found a collision (should only happen on copy)
        if (id == IGuidContainer.NoId || !registered)
        {
            Undo.RecordObject(this, "Assign GUID");
            id = GuidManager.Instance.Allocate();
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            print($"Auto assigned ID {id}");
            registered = GuidManager.Instance.Register(this);
        }
    }
#endif
}
