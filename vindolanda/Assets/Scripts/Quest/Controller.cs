using System.Collections.Generic;
using System.Linq;

namespace Vindolanda.Quest
{
    public class Controller : Saveable
    {
        public class ControllerSave : SaveData
        {
            public Dictionary<int, Quest.StateSave> states;

            public ControllerSave() { }
            public ControllerSave(Controller controller) : base(controller)
            {
                states = controller.states
                    .ToDictionary(kv => kv.Key.Id, kv => new Quest.StateSave(kv.Value));
            }
        }

        protected Dictionary<Quest, Quest.State> states = new();

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

        protected override void Start()
        {
            base.Start();
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
        }
    }
}