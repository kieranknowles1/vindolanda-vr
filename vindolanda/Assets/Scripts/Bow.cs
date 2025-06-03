using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] Transform arrowNockStart;
    [SerializeField] Transform arrowNockEnd;
    [SerializeField] Transform topConnection;
    [SerializeField] Transform bottomConnection;

    // TODO: Don't serialize
    [SerializeField, Range(0, 1)] float drawStrength;

    // TODO: Proper draw/release system
    private void OnDrawGizmos()
    {
        // This isn't as realistic, in reality the bow itself bends, the string doesn't stretch. I had a nice animation
        // but Unity couldn't import it :(
        Gizmos.color = Color.black;
        var position = Vector3.Lerp(arrowNockStart.position, arrowNockEnd.position, drawStrength);

        Gizmos.DrawLine(position, topConnection.position);
        Gizmos.DrawLine(position, bottomConnection.position);
    }
}
