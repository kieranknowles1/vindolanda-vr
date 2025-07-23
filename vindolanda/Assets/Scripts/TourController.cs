using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem;

public class TourController : MonoBehaviour
{
    public DefaultEvent trySayTourDialogue;
    public InputActionReference sayDialogue;

    public BehaviorGraphAgent agent;

    public bool GuideFollowing => agent.GetVariableValue<bool>("Following");

    private void Awake()
    {
        GameConstants.Instance.Tour = this;
    }

    private void OnEnable()
    {
        sayDialogue.action.performed += OnButtonPressed;
    }

    private void OnDisable()
    {
        sayDialogue.action.performed -= OnButtonPressed;
    }

    void OnButtonPressed(InputAction.CallbackContext _)
    {
        trySayTourDialogue.SendEventMessage();
    }
}
