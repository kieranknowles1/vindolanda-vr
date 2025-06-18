using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Vindolanda.Quest
{

    [CreateAssetMenu(menuName = "Quest/Objective")]
    public class Objective : GuidSO
    {
        [NonSerialized] public Quest Owner;

        public LocalizedString Description;
        public QuestEvent OnBegin;
        public QuestEvent OnEnd;
    }
}