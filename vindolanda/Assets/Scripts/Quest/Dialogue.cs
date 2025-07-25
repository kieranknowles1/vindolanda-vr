using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace Vindolanda.Quest
{
    public abstract class Dialogue : ScriptableObject
    {
        [Serializable]
        public class Line
        {
            public LocalizedString Text;
            public LocalizedAsset<AudioClip> Clip;
        }

        [Tooltip("Event triggered when an actor begins speaking a line.")]
        public UnityEvent OnBegin;
        [Tooltip("Event triggered when an actor stops speaking a line, even if interrupted.")]
        public UnityEvent OnEnd;

        /// <summary>
        /// Get the lines to play for this dialogue. Is not guaranteed to be deterministic, so cache the result
        /// </summary>
        /// <returns></returns>
        public abstract List<Line> GetLines();
    }
}