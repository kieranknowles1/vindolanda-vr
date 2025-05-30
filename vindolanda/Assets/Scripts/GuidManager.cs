using System;
using System.Collections.Generic;
using UnityEngine;

public class GuidManager
{
    private static GuidManager instance;
    public static GuidManager Instance
    {
        get
        {
            instance ??= new GuidManager();
            return instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        instance = null;
    }

    Dictionary<Guid, Saveable> objects = new();
    public Saveable Find(Guid guid)
    {
        return objects[guid];
    }

    /// <summary>
    /// Try to register an object
    /// </summary>
    /// <param name="saveable"></param>
    /// <returns>True on success, false on collision</returns>
    public bool Register(Saveable saveable)
    {
        objects.TryGetValue(saveable.Guid, out var existing);
        if (existing && existing != saveable) {
            Debug.LogWarning($"Attempted to register duplicate GUID {saveable.Guid}");
            return false;
        }

        objects[saveable.Guid] = saveable;
        return true;
    }
}