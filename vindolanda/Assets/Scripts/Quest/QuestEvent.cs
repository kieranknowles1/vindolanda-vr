using System;
using UnityEngine;
using UnityEngine.Events;

namespace Vindolanda.Quest
{
    /// <summary>
    /// Base class for an event triggered at part of a stage
    /// See also: <see cref="QuestEventDrawer">
    /// MUST be given the [<see cref="SerializeReference"/>] attribute
    /// </summary>
    [Serializable]
    public abstract class QuestEvent
    {
        public abstract void Execute();
    }
}