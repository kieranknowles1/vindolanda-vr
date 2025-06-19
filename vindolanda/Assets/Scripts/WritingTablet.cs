using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WritingTablet : XRGrabInteractable
{
    public GameObject detailedText;
    public Transform leftAttach;
    public Transform rightAttach;

    public override Transform GetAttachTransform(IXRInteractor interactor)
    {
        return interactor.handedness switch
        {
            InteractorHandedness.Left => leftAttach,
            InteractorHandedness.Right => rightAttach,
            InteractorHandedness.None => base.GetAttachTransform(interactor),
            _ => throw new System.Exception()
        };
    }

    bool IsSocket(IXRSelectInteractor interactor) => interactor is XRSocketInteractor;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (IsSocket(args.interactorObject)) return;

        detailedText.SetActive(true);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (IsSocket(args.interactorObject)) return;

        detailedText.SetActive(false);
    }
}
