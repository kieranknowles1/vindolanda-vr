using System;
using UnityEngine;

namespace Vindolanda.Quest
{

    [Serializable]
    public class SetObjectiveEvent : QuestEvent
    {
        public Quest quest;
        public Objective objective;

        public override void Execute()
        {
            GameConstants.Instance.QuestController.GetState(quest).CurrentObjective = objective;
        }
    }

}