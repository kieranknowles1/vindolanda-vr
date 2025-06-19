using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public interface IWeapon
{

}

public interface IHitTarget
{
    void OnHit(IWeapon weapon);
}

[RequireComponent(typeof(Rigidbody))]
public class Arrow : XRGrabInteractable, IWeapon
{
    Rigidbody body;
    SphereCollider trigger;
    public MeshRenderer Renderer;
    public Transform tip;

    /// <summary>
    /// Is the arrow stuck in an object and unable to move?
    /// </summary>
    public bool Stuck
    {
        get => !trigger.enabled;
        set {
            trigger.enabled = !value;
            body.SetFrozen(value);
        }
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        trigger = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var forwardVelocity = Vector3.Dot(body.linearVelocity, transform.forward);


        var target = other.GetInterface<IHitTarget>();
        target?.OnHit(this);

        Stuck = true;
    }
}
