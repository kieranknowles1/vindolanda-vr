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

    // TODO: Don't like holding initial values in the settings menu. This should be part of PlayerController
    [Header("Defaults")]
    [SerializeField] private PlayerController.MovementType moveType;
    [SerializeField] private float vignetteStrength;

    private void Start()
    {
        // Force UI and game state to match defaults given here
        MoveType = moveType;
        VignetteStrength = vignetteStrength;
    }

    public PlayerController.MovementType MoveType
    {
        get => moveType;
        set {
            moveType = value;
            movementTypeDropdown.value = (int)value;

            bool smooth = value != PlayerController.MovementType.Teleport;

            // Vignette is only relevant when using smooth movement
            vignetteSliderParent.SetActive(smooth);
            nauseaWarning.SetActive(smooth);
            playerController.SetMovementType(value);
        }
    }
    public void SetMoveTypeInt(int i)
    {
        MoveType = (PlayerController.MovementType)i;
    }

    public float VignetteStrength
    {
        get => vignetteStrength;
        set
        {
            vignetteStrength = value;
            vignetteSlider.value = value;
            playerController.SetVignetteStrength(value);
        }
    }
}
