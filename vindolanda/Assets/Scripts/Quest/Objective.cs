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
        [SerializeField] int targetId;

        GameObject targetObject;
        public GameObject TargetObject {
            get {
                if (targetObject == null && targetId != 0)
                    targetObject = GuidManager.Instance.Find<GuidComponent>(targetId).gameObject;
                return targetObject;
            }
            set => targetObject = value;
        }

        private void Awake()
        {
            if (targetId != 0) TargetObject = GuidManager.Instance.Find<GuidComponent>(targetId).gameObject;
        }
    }
}