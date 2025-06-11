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

        interactable.activated.AddListener(OnActivate);
    }

    private void OnDestroy()
    {
        interactable.activated.RemoveListener(OnActivate);
    }

    void OnActivate(ActivateEventArgs args)
    {
        body.SetFrozen(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
