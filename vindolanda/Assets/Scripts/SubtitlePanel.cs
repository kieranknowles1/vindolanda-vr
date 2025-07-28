using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SubtitlePanel : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        Hide();
    }

    /// <summary>
    /// Show subtitles on the screen. Does nothing if subtitles are disabled unless <paramref name="force"/> is <see langword="true"/>
    /// </summary>
    /// <param name="force">Show subtitles even if the user's preferences disable them. Should only be used for lines that do not have any associated recording.</param>
    public void Show(string speaker, string text, bool force = false)
    {
        if (!GameSettings.Instance.ShowSubtitles && !force) return;

        canvas.enabled = true;
        if (speaker == null) this.text.text = text;
        else this.text.text = $"{speaker}: {text}";
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
