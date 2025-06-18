using UnityEngine;
using Vindolanda.Quest;

public class SpeakTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Speaker speaker;
    public bool doOnce = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;

        speaker.Say(dialogue);
        if (doOnce) enabled = false;
    }
}
