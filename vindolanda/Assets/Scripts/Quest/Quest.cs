using UnityEngine;
using UnityEngine.Localization;

namespace Vindolanda.Quest
{
    [CreateAssetMenu(menuName = "Quest/Quest")]
    public class Quest : GuidSO
    {
        public class State
        {
            public readonly Quest Quest;

            Objective currentObjective;
            public Objective CurrentObjective {
                get => currentObjective;
                set {
                    if (value == currentObjective) return;
                    currentObjective?.OnEnd?.Execute();
                    currentObjective = value;
                    value?.OnBegin?.Execute();
                }
            }

            bool complete = false;
            public bool Complete
            {
                get => complete;
                set
                {
                    if (complete == value) return;
                    complete = value;
                    if (complete) CurrentObjective = null;
                }
            }
            public State(Quest quest) {  Quest = quest; }
            public State(Quest quest, StateSave state)
            {
                Quest = quest;
                currentObjective = GuidManager.Instance.Find<Objective>(state.objective);
                complete = state.complete;
            }
        }

        public class StateSave
        {
            public int objective;
            public bool complete;

            public StateSave() { }
            public StateSave(State state)
            {
                objective = state.CurrentObjective.Id;
                complete = state.Complete;
            }
        }

        public LocalizedString Name;
    }
}