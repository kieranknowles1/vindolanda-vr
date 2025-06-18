using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Vindolanda.Quest
{

    [CreateAssetMenu(menuName = "Quests/Objective")]
    public class Objective : GuidSO
    {
        [NonSerialized] public Quest Owner;

        public LocalizedString Description;
        public IEvent OnBegin;
        public IEvent OnEnd;
    }
}