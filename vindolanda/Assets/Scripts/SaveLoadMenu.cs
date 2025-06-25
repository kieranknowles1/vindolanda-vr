using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    public static readonly string SaveName = "Save.sav";
    public void OnSave()
    {
        SaveLoad.Save(SaveName);
    }

    public void OnLoad()
    {
        SaveLoad.Load(SaveName);
    }

    public void OnQuit()
    {
#if UNITY_STANDALONE // In builds
        Application.Quit();
#endif
#if UNITY_EDITOR // In editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
