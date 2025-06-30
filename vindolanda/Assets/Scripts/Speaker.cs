using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using Vindolanda.Quest;

/// <summary>
/// Component to control an actor's speech
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    public LocalizedString ActorName;

    private Dialogue currentDialogue;
    public Dialogue CurrentDialogue {
        get => currentDialogue;
        set {
            currentDialogue?.OnEnd?.Execute();
            currentDialogue = value;
            value?.OnBegin?.Execute();
        }
    }
    Coroutine speakCoroutine;

    public Coroutine Say(Dialogue dialogue)
    {
        IEnumerator SayImpl()
        {
            for (int i = 0; i < dialogue.Lines.Count; i++)
            {
                var line = dialogue.Lines[i];
                var clip = line.Clip != null && !line.Clip.IsEmpty ? line.Clip.LoadAsset() : PlaceholderClip;

                audio.PlayOneShot(clip);
                GameConstants.Instance.Player.Subtitles.Show(ActorName.GetLocalizedString(), line.Text.GetLocalizedString());

                yield return new WaitForSeconds(clip.length);
            }

            CurrentDialogue = null;
            speakCoroutine = null;
            GameConstants.Instance.Player.Subtitles.Hide();
        }

        if (speakCoroutine != null) {
            audio.Stop();
            StopCoroutine(speakCoroutine);
        }
        CurrentDialogue = dialogue;
        speakCoroutine = StartCoroutine(SayImpl());
        return speakCoroutine;
    }

    public AudioClip PlaceholderClip;

    private new AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
}
