using UnityEngine;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class SaveLoad
{
    public static Saveable[] GetSaveables()
    {
        return GameObject.FindObjectsByType<Saveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    static readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,
    };

#if UNITY_EDITOR
    [MenuItem("Tools/Select Saveable Objects")]
    static void FilterSaveable()
    {
        Selection.objects = GetSaveables().Select(s => s.gameObject).ToArray();
    }
#endif

    public static string FinalSavePath(string name) =>
        Path.Combine(Application.persistentDataPath, name);

    public static void Save(string name)
    {
        string path = FinalSavePath(name);
        var objects = GetSaveables().Select(o => o.Save()).ToArray();
        Debug.Log($"Saving {objects.Length} objects to {path}");
        var json = JsonConvert.SerializeObject(objects, settings);
        File.WriteAllText(path, json);
    }

    public static void Load(string name)
    {
        string path = FinalSavePath(name);
        Debug.Log($"Loading from {path}");
        var json = File.ReadAllText(path);
        var objects = JsonConvert.DeserializeObject<SaveData[]>(json, settings);
        Debug.Log($"Found {objects.Length} objects");

        foreach (var obj in objects)
        {
            var gameObj = GuidManager.Instance.TryFind<Saveable>(obj.id);
            if (gameObj == null)
            {
                Debug.Log($"Unable to find object with GUID {obj.id}");
                continue;
            }

            gameObj.Load(obj);
        }
    }
}
