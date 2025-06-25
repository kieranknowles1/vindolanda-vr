using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[Tooltip("Button that must be clicked twice to confirm.")]
[RequireComponent(typeof(Button))]
public class ConfirmButton : MonoBehaviour
{
    public enum Status
    {
        Ready,
        FirstClick
    }

    public LocalizedString confirmPrompt;
    LocalizedString initialString;

    public LocalizeStringEvent stringEvent;

    public UnityEvent onConfirm;

    Status status;
    public Status CurrentStatus
    {
        get => status;
        set
        {
            var newString = value switch
            {
                Status.Ready => initialString,
                Status.FirstClick => confirmPrompt,
                _ => throw new System.Exception()
            };
            stringEvent.StringReference = newString;
            status = value;
        }
    }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        initialString = stringEvent.StringReference;
    }

    private void OnEnable()
    {
        if (CurrentStatus != Status.Ready) CurrentStatus = Status.Ready;
    }

    void OnClick()
    {
        if (CurrentStatus == Status.Ready)
        {
            CurrentStatus = Status.FirstClick;
        }
        else if (CurrentStatus == Status.FirstClick)
        {
            CurrentStatus = Status.Ready;
            onConfirm?.Invoke();
        }
    }
}
