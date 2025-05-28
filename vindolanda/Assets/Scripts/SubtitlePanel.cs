using TMPro;
using UnityEngine;

public class SubtitlePanel : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;

    public void Show(string str)
    {
        canvas.enabled = true;
        text.text = str;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
