using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class ClaimManagerSave : SaveData
{
    public List<int> Free = new();
    public Dictionary<int, int> Reserved = new();

    public ClaimManagerSave() { }

    public ClaimManagerSave(ClaimManager obj) : base(obj)
    {
        Free = obj.Free;
        Reserved = obj.Reserved.ToDictionary(kv => kv.Key.GetComponent<GuidComponent>().Id, kv => kv.Value);
    }
}

/// <summary>
/// Allow actors to claim one of the direct children of this object
/// </summary>
public class ClaimManager : Saveable
{
    public override SaveData Save()
    {
        return new ClaimManagerSave(this);
    }

    public override void Load(SaveData data)
    {
        base.Load(data);
        var cmData = (ClaimManagerSave)data;
        Free = cmData.Free;
        Reserved = cmData.Reserved.ToDictionary(kv => GuidManager.Instance.Find<GuidComponent>(kv.Key).gameObject, kv => kv.Value);
    }

    // Assuming that transform.children is an array and not a linked list

    // Indexes of free children
    public List<int> Free { get; private set; } = new();
    // Reserved children to index
    public Dictionary<GameObject, int> Reserved { get; private set; } = new();

    protected void Start()
    {
        Free.AddRange(Enumerable.Range(0, transform.childCount));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Gizmos.DrawWireCube(child.position, Vector3.one);
        }
    }

    /// <summary>
    /// Claim a random free target
    /// </summary>
    /// <returns>The claimed target, or null if all children are reserved</returns>
    public GameObject Claim()
    {
        if (Free.Count == 0) return null;

        var listIndex = Random.Range(0, Free.Count);
        var childIndex = Free[listIndex];
        var child = transform.GetChild(childIndex).gameObject;

        Reserved.Add(child, childIndex);

        // The order of free doesn't matter, so swap the last element into the empty
        // slot rather than removing from the middle. O(1) instead of O(n)
        Free.RemoveAtSwapBack(listIndex);

        return child;
    }

    /// <summary>
    /// Release a claim, allowing it to be reused
    /// </summary>
    /// <param name="claimed"></param>
    public void Release(GameObject claimed)
    {
        var index = Reserved[claimed];
        Reserved.Remove(claimed);

        Free.Add(index);
    }
}
