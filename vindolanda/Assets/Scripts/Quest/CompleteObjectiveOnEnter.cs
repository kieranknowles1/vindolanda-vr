using UnityEngine;

/// <summary>
/// Complete a quest objective on trigger enter, or do nothing
/// if it is not started or the objective is already completed
/// </summary>
public class CompleteObjectiveOnEnter : MonoBehaviour
{
    public QuestObjective Objective;

    private void OnTriggerEnter(Collider other)
    {
        var quests = GameConstants.Instance.QuestController;

        var state = quests.GetState(Objective.Owner);
        if (state == null) return;

        state.CompleteObjective(Objective);
    }
}
