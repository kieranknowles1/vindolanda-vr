using UnityEngine;

[Tooltip("Hide the GameObject in play mode. Should be combined with the EditorOnly tag")]
public class HideInPlayMode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (!gameObject.CompareTag(TagHandle.GetExistingTag("EditorOnly")))
        {
            Debug.LogWarning($"{nameof(HideInPlayMode)} should be tagged EditorOnly");
        }
    }
}
