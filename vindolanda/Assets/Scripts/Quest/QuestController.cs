using System;
using System.Collections.Generic;

public class QuestSaveData : SaveData
{
    public Dictionary<int, QuestProgressSave> States = new();

    public QuestSaveData() { }
    public QuestSaveData(QuestController obj) : base(obj)
    {
        foreach (var quest in obj.States)
        {
            States[quest.Key.Id] = new QuestProgressSave(quest.Value);
        }
    }
}

public class QuestController : Saveable
{
    public Dictionary<Quest, QuestProgress> States
    {
        get; private set;
    } = new();

    // TODO: Remove
    public Quest testQuest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        GameConstants.Instance.QuestController = this;
        StartQuest(testQuest);
    }

    /// <summary>
    /// Start a new quest
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">If the quest is already active</exception>
    public QuestProgress StartQuest(Quest q)
    {
        if (States.ContainsKey(q))
        {
            throw new InvalidOperationException($"Tried to start {q.name}, but it is already running");
        }
        var progress = new QuestProgress(q);
        States[q] = progress;
        return progress;
    }

    /// <summary>
    /// Get the QuestProgress for a quest, or null if it is not started
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public QuestProgress GetState(Quest q)
    {
        return States[q];
    }

    public override SaveData Save()
    {
        return new QuestSaveData(this);
    }
    public override void Load(SaveData data)
    {
        base.Load(data);
        var questData = (QuestSaveData)data;
        States = new();

        foreach (var state in questData.States)
        {
            var quest = GuidManager.Instance.Find<Quest>(state.Key);
            States[quest] = new QuestProgress(quest, state.Value);
        }
    }
}
