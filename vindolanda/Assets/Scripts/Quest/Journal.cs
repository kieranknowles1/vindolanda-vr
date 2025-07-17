using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Localization;

namespace Vindolanda.Quest {

    public class Journal : MonoBehaviour
    {
        public TextMeshProUGUI summary;
        public TextMeshProUGUI details;
        public LookAtConstraint objectiveArrow;

        public LocalizedString noActiveQuest;

        private void Start()
        {
            ShowPlaceholder();
        }

        void ShowPlaceholder()
        {
            summary.text = noActiveQuest.GetLocalizedString();
            details.text = noActiveQuest.GetLocalizedString();
        }

        public void OnQuestChanged(Quest.State s)
        {
            if (s == null)
            {
                ShowPlaceholder();
                return;
            }

            summary.text = s.Quest.Name.GetLocalizedString();
            details.text = s.CurrentObjective.Description.GetLocalizedString();
        }

        public void OnObjectiveChanged(Quest.State s, Objective obj)
        {
            if (s == GameConstants.Instance.QuestController.ActiveQuest)
            {
                details.text = obj.Description.GetLocalizedString();

                if (s.CurrentObjective.targetId != 0)
                {
                    var target = GuidManager.Instance.Find<GuidComponent>(s.CurrentObjective.targetId);
                    objectiveArrow.gameObject.SetActive(true);
                    objectiveArrow.SetSource(0, new() { sourceTransform = target.transform, weight = 1.0f });
                }
                else
                {
                    objectiveArrow.gameObject.SetActive(false);
                }
            }
        }
    }

}