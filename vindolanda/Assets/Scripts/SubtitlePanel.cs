using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SubtitlePanel : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] LocalizedString template;

    private void Start()
    {
        Hide();
    }

    public void Show(string speaker, string text)
    {
        canvas.enabled = true;
        this.text.text = $"{speaker}: {text}";
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
