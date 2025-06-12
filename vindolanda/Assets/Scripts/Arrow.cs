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
    public Rigidbody Body { get; private set; }
    SphereCollider trigger;
    public MeshRenderer Renderer;

    /// <summary>
    /// Is the arrow stuck in an object and unable to move?
    /// </summary>
    public bool Stuck
    {
        get => !trigger.enabled;
        set {
            trigger.enabled = !value;
            Body.SetFrozen(value);
        }
    }

    public void Start()
    {
        Body = GetComponent<Rigidbody>();
        trigger = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var forwardVelocity = Vector3.Dot(Body.linearVelocity, transform.forward);


        var target = other.GetInterface<IHitTarget>();
        target?.OnHit(this);

        Stuck = true;
    }
}
