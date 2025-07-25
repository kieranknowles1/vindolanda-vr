using System.Collections.Generic;
using UnityEngine;

namespace Vindolanda.Quest
{
    [CreateAssetMenu(menuName = "Quest/Multi Dialogue")]
    public class MultiDialogue : Dialogue
    {
        [Header("Warning: Children's events will not be called")]
        public List<Dialogue> options = new();

        private void OnValidate()
        {
            foreach (var opt in options)
            {
                if (opt == null) continue;
                if (opt.OnBegin.GetPersistentEventCount() > 0 || opt.OnEnd.GetPersistentEventCount() > 0)
                {
                    Debug.LogWarning($"{nameof(OnBegin)} and {nameof(OnEnd)} of {opt.name} in {name} will not be called!", this);
                }
            }
        }

        public override List<Line> GetLines()
        {
            var final = options[Random.Range(0, options.Count)];
            return final.GetLines();
        }
    }
}