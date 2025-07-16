using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T PopBack<T>(this List<T> list)
    {
        var last = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return last;
    }
}
