using System;
using UnityEngine;

namespace Vindolanda.Quest
{
    [Serializable]
    public abstract class QuestEvent : ScriptableObject
    {
        public abstract void Execute();
    }
}