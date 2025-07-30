using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Random = UnityEngine.Random;

namespace Vindolanda
{

    public class DeliveryController : Saveable
    {
        public class DeliverySaveData : SaveData
        {
            public int questGiverId;
            public int questTargetId;

            public DeliverySaveData() { }

            public DeliverySaveData(DeliveryController controller) : base(controller)
            {
                questGiverId = controller.questGiver.Id;
                questTargetId = controller.questTarget.Id;
            }
        }

        [Serializable]
        public struct WritingTabletContents
        {
            public LocalizedString title;
            public LocalizedString text;
        }

        public GameObject writingTablet;
        ActorController questGiver;
        ActorController questTarget;
        List<ActorController> targets;
        public List<WritingTabletContents> tabletTexts;

        public override SaveData Save()
        {
            return new DeliverySaveData(this);
        }

        public override void Load(SaveData data)
        {
            base.Load(data);
            var deliverData = (DeliverySaveData)data;
            questGiver = GuidManager.Instance.Find<ActorController>(deliverData.questGiverId);
            questTarget = GuidManager.Instance.Find<ActorController>(deliverData.questTargetId);
        }

        public WritingTablet SpawnWritingTablet()
        {
            var instance = Instantiate(writingTablet).GetComponent<WritingTablet>();

            var contents = tabletTexts[Random.Range(0, tabletTexts.Count)];
            var text = instance.detailedText.GetComponentInChildren<LocalizeStringEvent>();
            text.StringReference = contents.text;
            instance.summary.StringReference = contents.title;

            return instance;
        }

        private void Start()
        {
            targets = GameObject.FindObjectsByType<ActorController>(FindObjectsSortMode.None)
                .Where(obj => obj.allowGenericQuests)
                .ToList();
            print($"{targets.Count} potential delivery targets");

            for (int i = 0; i < 10; i++)
            {
                var x = SpawnWritingTablet();
                x.transform.position = GameConstants.Instance.Player.transform.position + Vector3.forward;
            }
        }
    }
}