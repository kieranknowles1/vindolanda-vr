using System.Collections.Generic;
using UnityEngine;

namespace Vindolanda.Quest
{

    [CreateAssetMenu(menuName = "Quest/Dialogue")]
    public class SingleDialogue : Dialogue
    {
        public List<Line> Lines;

        public override List<Line> GetLines() => Lines;
    }
}