using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// We manually control the preview's visibility
[ExecuteInEditMode]
// Any GameObject referenced by a behaviour tree needs to have an ID
public class Furniture : GuidComponent
{
    public static string SitAnimName = "SittingIdle";
    public static string StandToSitAnimName = "StandToSit";
    public static int SitVariableId = Animator.StringToHash("Sit");

    public AnimatorOverrideController sitOverrides;
    public GuidComponent entryPoint;

    AnimationClip GetAnimation(string name)
    {
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        sitOverrides.GetOverrides(overrides);
        return overrides.First(o => o.Key.name == name).Value;
    }

#if UNITY_EDITOR
    [SerializeField] GameObject preview;
    GameObject previewInstance = null;

    void DrawPreview(ref GameObject instance, string animation, Transform parent)
    {
        if (instance == null)
        {
            instance = Instantiate(preview);
            instance.hideFlags = HideFlags.HideAndDontSave;
            instance.transform.parent = parent;
            instance.transform.SetPositionAndRotation(parent.position, parent.rotation);
        }

        var sit = GetAnimation(animation);
        sit.SampleAnimation(instance, sit.averageDuration);
    }

    private void OnDrawGizmosSelected()
    {
        if (sitOverrides == null || preview == null) return;

        DrawPreview(ref previewInstance, StandToSitAnimName, entryPoint.transform);
    }

    private void OnEnable()
    {
        if (sitOverrides == null) return;
        if (preview == null) return;

        DrawPreview(ref previewInstance, StandToSitAnimName, entryPoint.transform);
        previewInstance.SetActive(true);
    }

    private void OnDisable()
    {
        if (previewInstance != null)
        {
            previewInstance.SetActive(false);
        }
    }

    private void Start()
    {
        if (previewInstance != null) previewInstance.SetActive(false);
    }
#endif

    float sitDuration = float.NegativeInfinity;
    /// <summary>
    /// How long does it take to go from standing to sitting
    /// </summary>
    public float SitDuration
    {
        get
        {
            if (sitDuration == float.NegativeInfinity)
            {
                var sit = GetAnimation("StandToSit");
                sitDuration = sit.averageDuration;
            }
            return sitDuration;
        }
    }

    public ActorController CurrentActor { get; private set; }
    RuntimeAnimatorController animationBak;

    public enum SitResult
    {
        Success,
        // The furniture was occupied by a different actor, or an actor tried to stand but wasn't sitting here
        Failure,
    }
    public IEnumerator Sit(ActorController actor, Action<SitResult> callback = null)
    {
        if (CurrentActor != null)
        {
            callback?.Invoke(SitResult.Failure);
            yield break;
        }
        CurrentActor = actor;
        // Position actor to enter smoothly
        actor.transform.SetPositionAndRotation(entryPoint.transform.position, entryPoint.transform.rotation);
        actor.ActorAnimator.Halted = true;

        animationBak = actor.animator.runtimeAnimatorController;
        actor.animator.runtimeAnimatorController = sitOverrides;
        actor.animator.SetBool(SitVariableId, true);

        yield return LerpPosition(actor, entryPoint.transform, transform);
        callback?.Invoke(SitResult.Success);
    }

    // HACK: Enabling root motion causes sliding during the transition from SitToStand -> Sit
    IEnumerator LerpPosition(ActorController actor, Transform start, Transform end)
    {
        float time = 0;
        while (time < SitDuration)
        {
            time += Time.deltaTime;
            float ratio = time / SitDuration;
            actor.transform.position = Vector3.Lerp(start.position, end.position, ratio);
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator Stand(ActorController actor, Action<SitResult> callback = null)
    {
        if (CurrentActor != actor)
        {
            callback?.Invoke(SitResult.Failure);
            yield break;
        }
        actor.animator.SetBool(SitVariableId, false);
        yield return LerpPosition(actor, transform, entryPoint.transform);
        actor.ActorAnimator.Halted = false;
        actor.animator.runtimeAnimatorController = animationBak;
        animationBak = null;
        CurrentActor = null;

        callback?.Invoke(SitResult.Success);
    }
}
