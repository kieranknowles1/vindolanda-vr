using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// We manually control the preview's visibility
[ExecuteInEditMode]
public class Furniture : MonoBehaviour
{
    public AnimatorOverrideController sitOverrides;
    public GameObject entryPoint;

#if UNITY_EDITOR
    [SerializeField] GameObject preview;
    GameObject previewInstance = null;
    GameObject previewInstanceStand = null;

    void DrawPreview(ref GameObject instance, string animation)
    {
        if (instance == null)
        {
            instance = Instantiate(preview);
            instance.hideFlags = HideFlags.HideAndDontSave;
            instance.transform.parent = transform;
            instance.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        sitOverrides.GetOverrides(overrides);
        var sit = overrides.First(o => o.Key.name == animation).Value;
        sit.SampleAnimation(instance, 0);

        // Apply root motion from sit -> stand for its preview
        instance.transform.position = transform.position - (sit.averageSpeed * sit.averageDuration);
    }

    private void OnDrawGizmosSelected()
    {
        if (sitOverrides == null || preview == null) return;

        DrawPreview(ref previewInstance, "SittingIdle");
        DrawPreview(ref previewInstanceStand, "StandToSit");
    }

    private void OnEnable()
    {
        if (sitOverrides == null) return;
        if (preview == null) return;

        previewInstance.SetActive(true);
        DrawPreview(ref previewInstance, "SittingIdle");
        DrawPreview(ref previewInstanceStand, "StandToSit");

        if (entryPoint != null)
        {
            if (entryPoint.transform.localPosition != previewInstanceStand.transform.localPosition)
            {
                entryPoint.transform.localPosition = previewInstanceStand.transform.localPosition;
            }
        }
    }

    private void OnDisable()
    {
        if (previewInstance != null)
        {
            previewInstance.SetActive(false);
        }
    }
#endif
}
