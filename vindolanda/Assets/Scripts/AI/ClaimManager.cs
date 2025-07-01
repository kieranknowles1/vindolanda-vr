using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class ClaimManagerSave : SaveData
{
    public List<int> Available = new();
    public List<int> Reserved = new();

    public ClaimManagerSave() { }

    public ClaimManagerSave(ClaimManager obj) : base(obj)
    {
        Available = obj.Available.Select(o => o.Id).ToList();
        Reserved = obj.Reserved.Select(o => o.Id).ToList();
    }
}

/// <summary>
/// Allow actors to claim one of the direct children of this object
/// </summary>
public class ClaimManager : Saveable
{
    public List<GuidComponent> Available { get; private set; } = new();
    public HashSet<GuidComponent> Reserved { get; private set; } = new();

    void FillAvailable()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Available.Add(transform.GetChild(i).GetComponent<GuidComponent>());
        }
    }

    private void Awake()
    {
        FillAvailable();
    }

    public override SaveData Save()
    {
        return new ClaimManagerSave(this);
    }

    public override void Load(SaveData data)
    {
        base.Load(data);
        var cmData = (ClaimManagerSave)data;
        Available = cmData.Available.Select(o => GuidManager.Instance.Find<GuidComponent>(o)).ToList();
        Reserved = cmData.Reserved.Select(o => GuidManager.Instance.Find<GuidComponent>(o)).ToHashSet();
    }

    private void OnDrawGizmosSelected()
    {
        FillAvailable();
        Gizmos.color = Color.green;
        foreach (var slot in Available)
        {
            Gizmos.DrawWireCube(slot.transform.position, Vector3.one);
        }
    }

    /// <summary>
    /// Claim a random free target
    /// </summary>
    /// <returns>The claimed target, or null if all children are reserved</returns>
    public GuidComponent Claim()
    {
        if (Available.Count == 0) return null;

        var listIndex = Random.Range(0, Available.Count);
        var child = Available[listIndex];

        Reserved.Add(child);

        // The order of free doesn't matter, so swap the last element into the empty
        // slot rather than removing from the middle. O(1) instead of O(n)
        Available.RemoveAtSwapBack(listIndex);

        return child;
    }

    /// <summary>
    /// Release a claim, allowing it to be reused
    /// </summary>
    /// <param name="claimed"></param>
    public void Release(GuidComponent claimed)
    {
        if (!Reserved.Contains(claimed))
        {
            Debug.LogWarning($"Attempted to release a claim that {this.name} didn't give");
            return;
        }
        Reserved.Remove(claimed);
        Available.Add(claimed);
    }
}
