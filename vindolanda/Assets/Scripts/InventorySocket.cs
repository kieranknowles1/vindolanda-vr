using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InventorySocket : XRSocketInteractor
{
    [Tooltip("If set, only pick up this item.")]
    public XRGrabInteractable itemFilter;

    [Tooltip("If set, snap items back after dropping.")]
    public bool snapReturn;

    bool IsTarget(IXRInteractable interactable) => itemFilter == null || (object)interactable == itemFilter;

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && IsTarget(interactable);
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && IsTarget(interactable);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // Don't tie movement to the player
        args.interactableObject.transform.parent = null;

        if (snapReturn)
        {
            args.interactableObject.selectExited.AddListener(SnapBack);
        }
    }

    void SnapBack(SelectExitEventArgs args)
    {
        if ((object)args.interactorObject == this) return;

        interactionManager.SelectEnter(this, args.interactableObject);
        args.interactableObject.selectExited.RemoveListener(SnapBack);
    }
}
