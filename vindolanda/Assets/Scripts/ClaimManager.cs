using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

/// <summary>
/// Allow actors to claim one of the direct children of this object
/// </summary>
public class ClaimManager : MonoBehaviour
{
    // Assuming that transform.children is an array and not a linked list

    // Indexes of free children
    readonly List<int> free = new();
    // Reserved children to index
    readonly Dictionary<Transform, int> reserved = new();

    private void Start()
    {
        free.AddRange(Enumerable.Range(0, transform.childCount));
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
    public Transform Claim()
    {
        if (free.Count == 0) return null;
        
        var listIndex = Random.Range(0, free.Count);
        var childIndex = free[listIndex];
        var child = transform.GetChild(childIndex);

        reserved.Add(child, childIndex);

        // The order of free doesn't matter, so swap the last element into the empty
        // slot rather than removing from the middle. O(1) instead of O(n)
        free.RemoveAtSwapBack(listIndex);

        return child;
    }

    /// <summary>
    /// Release a claim, allowing it to be reused
    /// </summary>
    /// <param name="claimed"></param>
    public void Release(Transform claimed)
    {
        var index = reserved[claimed];
        reserved.Remove(claimed);

        free.Add(index);
    }
}
