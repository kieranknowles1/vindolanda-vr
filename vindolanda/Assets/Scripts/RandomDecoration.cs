using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GuidComponent))]
public class RandomDecoration : MonoBehaviour
{
    public List<GameObject> variants;
    public float chanceNone = 0.4f;

    void Start()
    {
        if (variants == null || variants.Count == 0) return;

        var id = GetComponent<GuidComponent>();
        if ((float)(id.Id % 1024) / 1024 < chanceNone) return;

        var choice = variants[id.Id % variants.Count];
        var obj = GameObject.Instantiate(choice);
        if (obj.TryGetComponent<GuidComponent>(out var newGuid))
        {
            // Give our ID to the new object if it needs one
            GuidManager.Instance.Replace(id, newGuid);
        }
        obj.transform.rotation = Quaternion.Euler(0, id.Id % 360, 0);

        obj.transform.parent = transform;
        obj.transform.position = transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (0.1f * Vector3.up), Vector3.one * 0.2f);
        UnityEditor.Handles.Label(transform.position, "Random item");
    }
#endif
}
