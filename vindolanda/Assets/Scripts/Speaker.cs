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
    const float SecondsPerWord = 0.25f;
    public LocalizedString ActorName;

    private Dialogue currentDialogue;
    public Dialogue CurrentDialogue {
        get => currentDialogue;
        set {
            currentDialogue?.OnEnd?.Invoke();
            currentDialogue = value;
            value?.OnBegin?.Invoke();
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
                var clip = line.Clip != null && !line.Clip.IsEmpty ? line.Clip.LoadAsset() : null;

                var text = line.Text.GetLocalizedString();
                GameConstants.Instance.Player.Subtitles.Show(ActorName.GetLocalizedString(), text);

                float duration;
                if (clip != null)
                {
                    audio.PlayOneShot(clip);
                    duration = clip.length;
                }
                else duration = text.Split(' ').Length * SecondsPerWord;

                yield return new WaitForSeconds(duration);
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

    private new AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
}
