using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace Vindolanda.Quest
{

    [CreateAssetMenu(menuName = "Quest/Objective")]
    public class Objective : GuidSO
    {
        [NonSerialized] public Quest Owner;

        public LocalizedString Description;
        public UnityEvent OnBegin;
        public UnityEvent OnEnd;

        [Tooltip("If set, objective marker will point to this object")]
        public int targetId;
    }
}