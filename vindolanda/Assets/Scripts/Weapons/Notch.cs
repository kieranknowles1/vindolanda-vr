using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Notch : MonoBehaviour
{
    public GameObject arrowPrefab;

    Arrow arrow;
    IXRInteractor hand;
    
    [SerializeField] BowAnimator animator;
    [SerializeField] Transform drawStart;
    [SerializeField] Transform drawEnd;
    [SerializeField] Transform arrowSpawn;

    [SerializeField] float releaseSpeed = 30;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Arrow>(out var newArrow)) return;
        if (!newArrow.isSelected) return;
        if (arrow == newArrow) return;

        arrow = newArrow;
        hand = newArrow.GetOldestInteractorSelecting();
        arrow.Renderer.enabled = false;
        animator.ArrowVisible = true;

        arrow.selectExited.AddListener(Release);
    }

    void Release(SelectExitEventArgs e)
    {
        // FIRE!
        var fired = Instantiate(arrowPrefab, arrowSpawn.transform.position, arrowSpawn.transform.rotation).GetComponent<Arrow>();
        fired.Body.linearVelocity = releaseSpeed * GetDrawLevel() * fired.transform.forward;
        fired.CurrentState = Arrow.State.InFlight;

        // Cleanup
        arrow.selectExited.RemoveListener(Release);
        animator.ArrowVisible = false;
        Destroy(arrow.gameObject);
        arrow = null;
        animator.DrawLevel = 0;
    }

    float LengthAlongLine(Vector3 a, Vector3 b, Vector3 point)
    {
        // Project start along end to get the closest approach
        Vector3 ab = b - a;
        Vector3 ap = point - a;
        return Vector3.Dot(ap, ab.normalized) * 2;
    }

    float GetDrawLevel() => Mathf.Clamp(LengthAlongLine(drawStart.position, drawEnd.position, hand.transform.position), 0, 1);

    private void Update()
    {
        if (arrow == null) return;

        animator.DrawLevel = GetDrawLevel();
    }
}
