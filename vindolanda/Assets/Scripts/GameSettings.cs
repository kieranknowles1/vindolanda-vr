using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameSettings
{
    public static string SettingsFile = Path.Combine(Application.persistentDataPath, "Settings.json");

    private static GameSettings instance;
    public static GameSettings Instance
    {
        get {
            instance ??= Load();
            return instance;
        }
    }
    private GameSettings() { }

    // Play nicely with domain reloading disabled
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        instance = null;
    }


    static GameSettings Load()
    {
        Debug.Log($"Reading settings from {SettingsFile}");
        if (File.Exists(SettingsFile)) {
            try {
                var contents = File.ReadAllText(SettingsFile);
                return JsonUtility.FromJson<GameSettings>(contents);
            }
            catch (Exception e) {
                Debug.LogException(e);
                Debug.LogError("Error loading user settings");
            }
        }
        Debug.Log("Could not load settings. Using defaults");
        return new GameSettings();
    }

    public void Save()
    {
        var contents = JsonUtility.ToJson(this, true);
        File.WriteAllText(SettingsFile, contents);
    }

    public enum MovementType
    {
        Teleport = 0,
        SmoothHeadForward = 1,
        SmoothControllerForward = 2
    }

    [Serializable]
    public class MovementSettings
    {
        public MovementType Type = MovementType.Teleport;
        public float VignetteStrength = 0.5f;
    }
    public MovementSettings Movement;
}
