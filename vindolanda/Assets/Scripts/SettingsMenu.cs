using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject vignetteSliderParent;

    [SerializeField] private Slider vignetteSlider;
    [SerializeField] private TMP_Dropdown movementTypeDropdown;

    [SerializeField] private PlayerController playerController;

    [SerializeField] private GameObject nauseaWarning;

    public void Start()
    {
        // Grab values from the player
        movementTypeDropdown.value = (int)playerController.MoveType;
        vignetteSlider.value = playerController.VignetteStrength;
        UpdateDisplayedElements();
    }

    public void SetMoveType(int i)
    {
        PlayerController.MovementType type = (PlayerController.MovementType)i;
        playerController.MoveType = type;
        UpdateDisplayedElements();
    }

    public void SetVignetteStrength(float strength)
    {
        playerController.VignetteStrength = strength;
    }

    private void UpdateDisplayedElements()
    {
        // Vignette is only relevant when using smooth movement
        bool smooth = playerController.MoveType != PlayerController.MovementType.Teleport;
        vignetteSliderParent.SetActive(smooth);
        nauseaWarning.SetActive(smooth);
    }
}
