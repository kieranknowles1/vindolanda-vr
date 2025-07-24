using System;
using System.Linq;
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
    public enum State
    {
        Default,
        InFlight,
        Embedded
    };

    public Rigidbody Body { get; private set; }
    SphereCollider trigger;
    public MeshRenderer Renderer;
    public Transform tip;

    State state = State.Default;
    public State CurrentState
    {
        get => state;
        set
        {
            state = value;
            trigger.enabled = value == State.InFlight;
            Body.SetFrozen(value == State.Embedded);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Body = GetComponent<Rigidbody>();
        trigger = GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (state != State.InFlight) return;
        transform.rotation = Quaternion.LookRotation(Body.linearVelocity.normalized);

        var distance = Body.linearVelocity.magnitude * Time.deltaTime * 1.5f;
        if (!Physics.Raycast(tip.position, tip.forward, out var hit, distance, layerMask: ~Layers.Projectile, QueryTriggerInteraction.Ignore)) return;

        var mat = MaterialData.GetExtraData(hit.collider.sharedMaterial);
        CurrentState = mat.arrowsStick ? State.Embedded : State.Default;

        transform.position = hit.point - (tip.rotation * tip.localPosition);

        var target = hit.collider.GetInterface<IHitTarget>();
        target?.OnHit(this);
    }
}
