using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using Vindolanda.Quest;

/// <summary>
/// Component to control an actor's speech
/// </summary>
public class Speaker : MonoBehaviour
{
    public new AudioSource audio;

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

    /// <summary>
    /// Say a line, coroutine will run until completion
    /// </summary>
    /// <param name="dialogue">The line to say</param>
    /// <param name="delay">A delay before starting the line</param>
    /// <param name="pitch">The pitch to play at. Even +-5% is noticeable</param>
    /// <returns></returns>
    public Coroutine Say(Dialogue dialogue, float delay = 0.0f, float pitch = 1.0f)
    {
        IEnumerator SayImpl()
        {
            var lines = dialogue.GetLines();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var clip = line.Clip != null && !line.Clip.IsEmpty ? line.Clip.LoadAsset() : null;

                var text = line.Text.GetLocalizedString();
                GameConstants.Instance.Player.Subtitles.Show(ActorName.isDirty ? null : ActorName.GetLocalizedString(), text);

                float duration;
                if (clip != null)
                {
                    audio.clip = clip;
                    audio.pitch = pitch;
                    audio.PlayDelayed(delay);
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
}
