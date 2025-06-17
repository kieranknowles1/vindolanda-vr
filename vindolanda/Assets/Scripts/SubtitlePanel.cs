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

    public void Show(ActorController speaker, LocalizedString text)
    {
        canvas.enabled = true;
        template["speaker"] = speaker.Name;
        template["text"] = text;
        this.text.text = template.GetLocalizedString();
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
