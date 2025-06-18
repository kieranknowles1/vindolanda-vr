using UnityEngine;
using Vindolanda.Quest;

[Tooltip("Trigger an event when the trigger is entered.")]
public class EventTrigger : MonoBehaviour
{
    public QuestEvent onEnter;
    public bool singleUse;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;

        onEnter.Execute();

        if (singleUse) enabled = false;
    }
}
