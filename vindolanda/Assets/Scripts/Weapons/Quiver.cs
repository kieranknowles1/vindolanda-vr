using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Quiver : XRBaseInteractable
{
    [SerializeField] GameObject arrowPrefab;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        var hand = args.interactorObject;
        var arrow = Instantiate(arrowPrefab, hand.transform.position, hand.transform.rotation).GetComponent<XRGrabInteractable>();
        interactionManager.SelectEnter(hand, arrow);
    }
}
