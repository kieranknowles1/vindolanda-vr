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

    public void Show(string speaker, string text)
    {
        canvas.enabled = true;
        if (speaker == null) this.text.text = text;
        else this.text.text = $"{speaker}: {text}";
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
