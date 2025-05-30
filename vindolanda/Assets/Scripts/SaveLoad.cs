using UnityEngine;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;


#if UNITY_EDITOR
using UnityEditor;
#endif

public static class SaveLoad
{
    public static Saveable[] GetSaveables()
    {
        return GameObject.FindObjectsByType<Saveable>(FindObjectsSortMode.None);
    }

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
        var formatter = new BinaryFormatter();
        string path = FinalSavePath(name);
        FileStream stream = new FileStream(path, FileMode.Create);
        var objects = GetSaveables().Select(o => o.Save()).ToArray();
        Debug.Log($"Saving {objects.Length} objects to {path}");

        formatter.Serialize(stream, objects);
        stream.Close();
    }

    public static void Load(string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = FinalSavePath(name);
        Debug.Log($"Loading from {path}");
        FileStream stream = new FileStream(path, FileMode.Open);
        var objects = (SaveData[])formatter.Deserialize(stream);
        Debug.Log($"Found {objects.Length} objects");
        // TODO

        stream.Close();
    }
}
