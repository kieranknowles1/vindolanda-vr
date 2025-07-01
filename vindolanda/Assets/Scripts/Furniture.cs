using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// We manually control the preview's visibility
[ExecuteInEditMode]
public class Furniture : MonoBehaviour
{
    public static string SitAnimName = "SittingIdle";
    public static string StandToSitAnimName = "StandToSit";
    public static int SitVariableId = Animator.StringToHash("Sit");

    public AnimatorOverrideController sitOverrides;
    public Transform entryPoint;

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

        DrawPreview(ref previewInstance, StandToSitAnimName, entryPoint);
    }

    private void OnEnable()
    {
        if (sitOverrides == null) return;
        if (preview == null) return;

        DrawPreview(ref previewInstance, StandToSitAnimName, entryPoint);
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
    public IEnumerator Sit(ActorController actor)
    {
        if (CurrentActor != null)
        {
            throw new System.Exception($"Furniture already in use by {CurrentActor.name}");
        }
        CurrentActor = actor;
        // Position actor to enter smoothly
        actor.transform.SetPositionAndRotation(entryPoint.transform.position, entryPoint.transform.rotation);

        animationBak = actor.animator.runtimeAnimatorController;
        actor.animator.runtimeAnimatorController = sitOverrides;
        actor.animator.SetBool(SitVariableId, true);

        yield return LerpPosition(actor, entryPoint, transform);
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

    public IEnumerator Stand(ActorController actor)
    {
        if (CurrentActor != actor)
        {
            throw new System.Exception($"{actor.name} tried to stand from furniture it is not sitting");
        }
        actor.animator.SetBool(SitVariableId, false);
        yield return LerpPosition(actor, transform, entryPoint);
        actor.animator.runtimeAnimatorController = animationBak;
        animationBak = null;
        CurrentActor = null;
    }
}
