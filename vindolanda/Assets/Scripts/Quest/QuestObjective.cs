using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objective")]
public class QuestObjective : ScriptableObject
{
    [NonSerialized] public Quest Owner;

    public string Description;
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
}