using System;
using Unity.Behavior;
using UnityEngine;
using Vindolanda.Quest;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/DialogueEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "DialogueEvent", message: "Agent wants to say [Dialogue]", category: "Events", id: "f6479d90aa3c34e2c620a0ed4ffe4ca6")]
public sealed partial class DialogueEvent : EventChannel<Dialogue> { }

