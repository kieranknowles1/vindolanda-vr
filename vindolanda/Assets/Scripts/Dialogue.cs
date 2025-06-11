using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Data/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Serializable] public class Line
    {
        public LocalizedString Text;
        public LocalizedAsset<AudioClip> Clip;
    }
    public List<Line> Lines;
}
