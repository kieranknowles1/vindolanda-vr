using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SubtitlePanel : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        // Canvas positioning is handled by LazyFollow
        canvas.transform.SetParent(null, worldPositionStays: false);
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
