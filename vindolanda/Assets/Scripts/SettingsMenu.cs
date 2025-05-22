using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject vignetteSliderParent;

    [SerializeField] private Slider vignetteSlider;
    [SerializeField] private TMP_Dropdown movementTypeDropdown;

    [SerializeField] private GameObject nauseaWarning;

    public void Start()
    {
        // Grab values from GameSettings
        movementTypeDropdown.value = (int)GameSettings.Instance.Movement.Type;
        vignetteSlider.value = GameSettings.Instance.Movement.VignetteStrength;
        UpdateDisplayedElements();
    }

    public void SetMoveType(int i)
    {
        GameSettings.Instance.Movement.Type = (GameSettings.MovementType)i;
        GameSettings.Instance.ApplyChanges();
        UpdateDisplayedElements();
    }

    public void SetVignetteStrength(float strength)
    {
        GameSettings.Instance.Movement.VignetteStrength = strength;
        GameSettings.Instance.ApplyChanges();
    }

    private void UpdateDisplayedElements()
    {
        // Vignette is only relevant when using smooth movement
        bool smooth = GameSettings.Instance.Movement.Type != GameSettings.MovementType.Teleport;
        vignetteSliderParent.SetActive(smooth);
        nauseaWarning.SetActive(smooth);
    }
}
