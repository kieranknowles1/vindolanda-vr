using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.Behavior.RuntimeSerializationUtility;

public interface IGuidContainer
{
    public Guid Guid { get; }
}

public class GuidManager : IUnityObjectResolver<string>
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

    // Called when entering play mode
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        instance = null;

        EditorApplication.playModeStateChanged += ModeChanged;
    }

    // Called when exiting play mode. Duplicate check will fail ONLY when using
    // interfaces, assuming this is Unity's fault. Without this we will trigger
    // duplicate GUID checks, which causes everything to be reassigned :(
    static void ModeChanged(PlayModeStateChange change)
    {
        if (change != PlayModeStateChange.ExitingPlayMode) return;
        instance = null;
    }

    Dictionary<Guid, IGuidContainer> objects = new();
    public IGuidContainer Find(Guid guid)
    {
        return objects[guid];
    }
    public IGuidContainer TryFind(Guid guid)
    {
        objects.TryGetValue(guid, out IGuidContainer saveable);
        return saveable;
    }

    /// <summary>
    /// Try to register an object
    /// </summary>
    /// <param name="saveable"></param>
    /// <returns>True on success, false on collision</returns>
    public bool Register(IGuidContainer saveable)
    {
        if (objects.TryGetValue(saveable.Guid, out var existing) && existing != saveable)
        {
            Debug.LogWarning($"Attempted to register duplicate GUID {saveable.Guid}");
            return false;
        }

        objects[saveable.Guid] = saveable;
        return true;
    }

    public string Map(UnityEngine.Object obj)
    {
        var component = obj.GetComponent<GuidComponent>();
        if (component == null)
        {
            Debug.LogError($"{obj} Not saveable");
            throw new Exception("Not saveable");
        }
        return obj.GetComponent<GuidComponent>().Guid.ToString();
    }

    public TSerializedType Resolve<TSerializedType>(string mappedValue) where TSerializedType : UnityEngine.Object
    {
        var uid = Guid.Parse(mappedValue);
        var obj = (GuidComponent)TryFind(uid);

        if (typeof(TSerializedType) == typeof(GameObject))
        {
            return obj.gameObject as TSerializedType;
        }
        if (typeof(Component).IsAssignableFrom(typeof(TSerializedType)))
        {
            var cast = obj.GetComponent<TSerializedType>();
            if (cast != null) return cast;
        }

        Debug.LogWarning($"Failed to find {mappedValue} {uid}");
        return null;
    }
}
