using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Vindolanda.Quest
{
    public class Controller : Saveable
    {
        public class ControllerSave : SaveData
        {
            public Dictionary<int, Quest.StateSave> states;
            public int? activeQuest;

            public ControllerSave() { }
            public ControllerSave(Controller controller) : base(controller)
            {
                states = controller.states
                    .ToDictionary(kv => kv.Key.Id, kv => new Quest.StateSave(kv.Value));
                activeQuest = controller.ActiveQuest?.Quest?.Id;
            }
        }

        protected Dictionary<Quest, Quest.State> states = new();
        protected Quest.State activeQuest;
        public Quest.State ActiveQuest
        {
            get => activeQuest;
            set {
                if (value == activeQuest) return;

                OnActiveQuestChanged.Invoke(value);
                activeQuest = value;
            }
        }

        public UnityEvent<Quest.State> OnActiveQuestChanged;
        public UnityEvent<Quest.State, Objective> OnQuestObjectiveChanged;

        public Quest.State GetState(Quest quest)
        {
            if (states.TryGetValue(quest, out var state))
            {
                return state;
            }
            state = new Quest.State(quest);
            states.Add(quest, state);
            print($"Start quest {quest.name}");
            return state;
        }

        protected void Start()
        {
            GameConstants.Instance.QuestController = this;
        }

        public override SaveData Save()
        {
            return new ControllerSave(this);
        }
        public override void Load(SaveData data)
        {
            base.Load(data);
            var questData = (ControllerSave)data;
            states = new();

            foreach (var state in questData.states)
            {
                var quest = GuidManager.Instance.Find<Quest>(state.Key);
                states[quest] = new Quest.State(quest, state.Value);
            }

            // TODO: Should we be sending OnActiveQuestChanged during load?
            ActiveQuest = questData.activeQuest != null
                ? states[GuidManager.Instance.Find<Quest>(questData.activeQuest.Value)]
                : null;
        }
    }
}