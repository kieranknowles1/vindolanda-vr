using UnityEngine;
using UnityEngine.InputSystem;

public class TourController : MonoBehaviour
{
    public DefaultEvent trySayTourDialogue;
    public InputActionReference sayDialogue;

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
