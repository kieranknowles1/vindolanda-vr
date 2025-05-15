using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    // TODO: Store settings and update the panel to match
    [SerializeField] private GameObject vignetteSlider;
    [SerializeField] private PlayerController playerController;

    public void SetVignetteStrength(float strength)
    {
        playerController.SetVignetteStrength(strength);
    }

    public void SetMovementType(int i)
    {
        PlayerController.MovementType type = (PlayerController.MovementType)i;

        // Vignette is only relevant when using smooth movement
        vignetteSlider.SetActive(type != PlayerController.MovementType.Teleport);
        playerController.SetMovementType(type);
    }
}
