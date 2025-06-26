using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class TutorialBowl : XRGrabInteractable
{
    public Tutorial tutorial;

    Coroutine routine;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        routine = StartCoroutine(WaitHold());
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        StopCoroutine(routine);
    }

    IEnumerator WaitHold()
    {
        yield return new WaitForSeconds(1);
        tutorial.OnItemHeld();
    }
}
