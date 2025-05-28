using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    private Dictionary<Quest, QuestProgress> States = new();

    // TODO: Remove
    public Quest testQuest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
}
