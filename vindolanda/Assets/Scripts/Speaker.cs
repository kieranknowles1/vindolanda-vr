using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to control an actor's speech
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
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

        var clip = currentDialogue.Lines[nextLineIndex].Clip != null ? currentDialogue.Lines[nextLineIndex].Clip : PlaceholderClip;
        audio.PlayOneShot(clip);
        GameConstants.Instance.Player.Subtitles.Show(currentDialogue.Lines[nextLineIndex].Text);

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
