using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class FreezeUntilFirstGrab : MonoBehaviour
{
    XRGrabInteractable interactable;
    Rigidbody body;

    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        body = GetComponent<Rigidbody>();

        body.SetFrozen(true);

        interactable.selectEntered.AddListener(OnGrab);
    }

    private void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        body.SetFrozen(false);
    }
}
