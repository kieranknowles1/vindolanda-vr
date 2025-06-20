using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Vindolanda.Quest {

    public class Journal : MonoBehaviour
    {
        public TextMeshProUGUI summary;
        public TextMeshProUGUI details;

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
            }
        }
    }

}