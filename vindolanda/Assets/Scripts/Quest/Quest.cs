using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class QuestProgress
{
    public event Action OnComplete;

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

    public QuestProgress(Quest quest, QuestProgressSave save)
    {
        Quest = quest;
        Objectives = new();
        foreach (var obj in save.Objectives)
        {
            var objective = (QuestObjective)GuidManager.Instance.Find(new Guid(obj.Key));
            Objectives[objective] = new ObjectiveState(objective, obj.Value);
        }
    }

    /// <summary>
    /// Complete an objective. No-op if the objective is
    /// already complete
    /// </summary>
    /// <param name="obj"></param>
    public void CompleteObjective(QuestObjective obj)
    {
        if (Objectives[obj].IsCompleted) return;
        Objectives[obj].IsCompleted = true;
        Debug.Log($"Completed {obj.Description}");

        if (IsCompleted)
        {
            OnComplete?.Invoke();
            Debug.Log($"{Quest.Name} is completed");
        }
    }

    public bool IsCompleted => Objectives.All(s => s.Value.IsCompleted);
}

[Serializable]
public class QuestProgressSave
{
    public Dictionary<string, QuestObjectiveSave> Objectives = new();
    public QuestProgressSave() { }
    public QuestProgressSave(QuestProgress obj)
    {
        foreach (var objective in obj.Objectives)
        {
            Objectives[objective.Key.Guid.ToString()] = new QuestObjectiveSave(objective.Value);
        }
    }
}

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : GuidSO
{
    public string Name;
    public string Description;
    public List<QuestObjective> Objectives;

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var obj in Objectives)
        {
            if (obj.Owner != null && obj.Owner != this) Debug.LogError($"{obj.name} has multiple owners");
            obj.Owner = this;
        }
    }
}
