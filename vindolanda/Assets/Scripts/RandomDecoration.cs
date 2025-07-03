using System.Collections.Generic;
using UnityEngine;

public class RandomDecoration : MonoBehaviour
{
    public List<GameObject> variants;
    public float chanceNone = 0.4f;

    void Start()
    {
        if (variants == null || variants.Count == 0) return;
        if (Random.Range(0.0f, 1.0f) < chanceNone) return;

        var choice = variants[Random.Range(0, variants.Count)];
        var obj = GameObject.Instantiate(choice);
        obj.transform.SetPositionAndRotation(
            transform.position,
            Quaternion.Euler(0, Random.Range(0, 360), 0)
        );
        obj.transform.parent = transform;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (0.1f * Vector3.up), Vector3.one * 0.2f);
        UnityEditor.Handles.Label(transform.position, "Random item");
    }
#endif
}
