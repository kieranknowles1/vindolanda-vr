using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.Behavior.RuntimeSerializationUtility;

public interface IGuidContainer
{
    public static readonly int NoId = 0;
    public int Id { get; }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Unity method")]
    public string name { get; }

    // Unity objects overload == null for legacy reasons, but this doesn't propogate to interfaces
    public bool IsDestroyed { get; }
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

    private GuidManager()
    {
        var objs = GameObject.FindObjectsByType<GuidComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None).Cast<IGuidContainer>();
        objects = objs.ToDictionary(o => o.Id, o => o);
    }

#if UNITY_EDITOR
    static void ClearInstance()
    {
        // Preserve scriptable object registrations, but delete GameObjects
        // Scriptable objects persist when exiting play mode.
        // At runtime, the same instance will be kept perpetually.
        var sos = instance.objects.Where(o => o.Value is ScriptableObject).ToList();
        instance = new GuidManager();
        instance.objects.AddRange(sos);
    }

    // Called when entering play mode
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        ClearInstance();
        EditorApplication.playModeStateChanged += ModeChanged;
    }

    // Called when exiting play mode. Duplicate check will fail ONLY when using
    // interfaces, assuming this is Unity's fault. Without this we will trigger
    // duplicate GUID checks, which causes everything to be reassigned :(
    static void ModeChanged(PlayModeStateChange change)
    {
        if (change != PlayModeStateChange.ExitingPlayMode) return;
        ClearInstance();
    }
#endif

    readonly Dictionary<int, IGuidContainer> objects;
    public T Find<T>(int guid) where T : class, IGuidContainer
    {
        return (T)objects[guid];
    }
    public T TryFind<T>(int guid) where T: class, IGuidContainer
    {
        objects.TryGetValue(guid, out IGuidContainer saveable);
        return (T)saveable;
    }

    /// <summary>
    /// Try to register an object
    /// </summary>
    /// <param name="saveable"></param>
    /// <returns>True on success, false on collision</returns>
    public bool Register(IGuidContainer saveable)
    {
        if (saveable.Id == IGuidContainer.NoId) return false;
        if (objects.TryGetValue(saveable.Id, out var existing) && existing != saveable && !existing.IsDestroyed)
        {
            var fullName = existing is Component cmp ? cmp.transform.FullObjectPath() : existing.name;
            

            Debug.LogWarning($"Attempted to register duplicate GUID {saveable.Id}. Conflicts with {fullName}");
            return false;
        }

        objects[saveable.Id] = saveable;
        return true;
    }

    private static readonly System.Random rng = new();
    private static readonly int maxAllocationAttempts = 128;
    /// <summary>
    /// Generate a new ID
    /// </summary>
    /// <returns>A random ID that is not yet registered</returns>
    /// <exception cref="Exception">If allocation fails after a reasonable number of attempts</exception>
    public int Allocate()
    {
        for (int i = 0; i < maxAllocationAttempts; i++) {
            var id = rng.Next(int.MaxValue);
            if (!objects.ContainsKey(id)) return id;
        }
        throw new Exception("ID allocation failed");
    }

    public string Map(UnityEngine.Object obj)
    {
        if (obj == null) return "";
        var component = obj.GetComponent<GuidComponent>();
        if (component == null)
        {
            Debug.LogError($"{obj} Not saveable");
            throw new Exception("Not saveable");
        }
        return component.Id.ToString();
    }

    public TSerializedType Resolve<TSerializedType>(string mappedValue) where TSerializedType : UnityEngine.Object
    {
        if (mappedValue == "") return null;
        var uid = int.Parse(mappedValue);
        var obj = TryFind<GuidComponent>(uid);

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
