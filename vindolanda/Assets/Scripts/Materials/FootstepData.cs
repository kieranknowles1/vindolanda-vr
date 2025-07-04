using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Footsteps")]
public class FootstepData : ScriptableObject
{
#if UNITY_EDITOR
    public UnityEditor.DefaultAsset sourceDirectory;
#endif

    public List<AudioClip> walk;
}
