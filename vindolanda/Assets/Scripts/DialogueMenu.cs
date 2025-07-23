using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour
{
    public GameObject entryTemplate;
    public Transform contents;
    public TextMeshProUGUI header;

    /// <summary>
    /// Called when an option is clicked with the index of the selection
    /// </summary>
    public UnityEvent<int> onClicked;

    List<GameObject> entries = new();

    public void Display(string speaker, List<string> options)
    {
        header.text = speaker;
        for (int i = 0; i < options.Count; i++)
        {
            AddOption(i, options[i]);
        }
    }

    public void Clear()
    {
        foreach (var e in entries)
            Destroy(e);
        entries.Clear();
    }

    void AddOption(int index, string option)
    {
        var instance = Instantiate(entryTemplate);
        var text = instance.GetComponentInChildren<TextMeshProUGUI>();
        var button = instance.GetComponentInChildren<Button>();

        text.text = option;
        button.onClick.AddListener(() => onClicked.Invoke(index));

        instance.transform.SetParent(contents, worldPositionStays: false);
        entries.Add(instance);
    }
}
