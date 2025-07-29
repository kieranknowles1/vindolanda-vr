using UnityEngine;
using UnityEngine.Localization;

// HACK: LocalizedStrings can't be used in a blackboard, so we need to use this
// to serialize them properly for runtime
[CreateAssetMenu(menuName = "Quest/Choices")]
public class DialogueChoices : ScriptableObject
{
    public LocalizedString positive;
    public LocalizedString negative;
}
