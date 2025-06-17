using System;
using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// Component to control an actor's speech
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    public LocalizedString ActorName;

    public enum SpeechResult
    {
        // The dialogue played to completion
        Success,
        // The dialogue was interrupted before completion
        Interrupted,
    }

    public event Action<Dialogue, SpeechResult> OnSpeechComplete;
    private void CompleteCurrentDialogue(SpeechResult result)
    {
        OnSpeechComplete?.Invoke(currentDialogue, result);
        audio.Stop();
        GameConstants.Instance.Player.Subtitles.Hide();
        currentDialogue = null;
    }

    public void Say(Dialogue dialogue)
    {
        if (currentDialogue != null)
        {
            CompleteCurrentDialogue(SpeechResult.Interrupted);
        }
        currentDialogue = dialogue;
        nextLineIndex = 0;
        StartNextLine();
    }

    public AudioClip PlaceholderClip;

    private new AudioSource audio;

    private Dialogue currentDialogue;
    private int nextLineIndex;

    void StartNextLine()
    {
        if (nextLineIndex >= currentDialogue.Lines.Count)
        {
            CompleteCurrentDialogue(SpeechResult.Success);
            return;
        }

        var line = currentDialogue.Lines[nextLineIndex];
        // TODO: Preload assets at the start of the line
        var clip = line.Clip != null && !line.Clip.IsEmpty ? line.Clip.LoadAsset() : PlaceholderClip;

        audio.PlayOneShot(clip);
        GameConstants.Instance.Player.Subtitles.Show(ActorName.GetLocalizedString(), line.Text.GetLocalizedString());

        nextLineIndex++;
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (currentDialogue && !audio.isPlaying)
        {
            StartNextLine();
        }
    }
}
