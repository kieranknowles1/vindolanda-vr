using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Serializable] public class Line
    {
        public string Text;
        public AudioClip Clip;
    }
    public List<Line> Lines;
}
