using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class QuestProgress
{
    public Quest Quest;
    public Dictionary<QuestObjective, ObjectiveState> Objectives;

    public QuestProgress(Quest quest)
    {
        Quest = quest;
        Objectives = new();
        foreach (var obj in Quest.Objectives) {
            Objectives[obj] = new(obj);
        }
    }

    public void CompleteObjective(QuestObjective obj)
    {
        Objectives[obj].IsCompleted = true;
        Debug.Log($"Completed {obj.Description}");
    }

    public bool IsCompleted => Objectives.All(s => s.Value.IsCompleted);
}


[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string Name;
    public string Description;
    public List<QuestObjective> Objectives;

    private void OnEnable()
    {
        foreach (var obj in Objectives)
        {
            if (obj.Owner != null) Debug.LogError($"{obj.name} has multiple owners");
            obj.Owner = this;
        }
    }
}
