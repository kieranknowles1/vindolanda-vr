using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Behavior.RuntimeSerializationUtility;

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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        instance = null;
    }

    Dictionary<Guid, GuidComponent> objects = new();
    public GuidComponent Find(Guid guid)
    {
        return objects[guid];
    }
    public GuidComponent TryFind(Guid guid)
    {
        objects.TryGetValue(guid, out GuidComponent saveable);
        return saveable;
    }

    /// <summary>
    /// Try to register an object
    /// </summary>
    /// <param name="saveable"></param>
    /// <returns>True on success, false on collision</returns>
    public bool Register(GuidComponent saveable)
    {
        objects.TryGetValue(saveable.Guid, out var existing);
        if (existing && existing != saveable) {
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
        var obj = TryFind(uid);

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