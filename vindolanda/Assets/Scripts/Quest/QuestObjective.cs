using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objective")]
public class QuestObjective : GuidSO
{
    [NonSerialized] public Quest Owner;

    public string Description;
}

[Serializable]
public class QuestObjectiveSave
{
    public bool IsCompleted;
    public QuestObjectiveSave() { }
    public QuestObjectiveSave(ObjectiveState objective)
    {
        IsCompleted = objective.IsCompleted;
    }
}

[Serializable]
public class ObjectiveState
{
    public QuestObjective Objective;
    public bool IsCompleted = false;

    public ObjectiveState(QuestObjective objective)
    {
        Objective = objective;
    }

    public ObjectiveState(QuestObjective objective, QuestObjectiveSave save)
    {
        Objective = objective;
        IsCompleted = save.IsCompleted;
    }
}
