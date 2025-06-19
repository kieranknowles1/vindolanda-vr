using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/DefaultEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "DefaultEvent", message: "Something has happened", category: "Events", id: "b5affd5e15793ffeaa10b2f936eaf32a")]
public sealed partial class DefaultEvent : EventChannel { }

