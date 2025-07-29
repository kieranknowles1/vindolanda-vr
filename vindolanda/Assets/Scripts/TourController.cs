using Unity.Behavior;
using UnityEngine;

public class TourController : MonoBehaviour
{
    public BehaviorGraphAgent agent;

    public bool GuideFollowing => agent.GetVariableValue<bool>("Following");
}
