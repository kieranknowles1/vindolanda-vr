using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Random = UnityEngine.Random;

namespace Vindolanda
{
    public class DeliveryController : MonoBehaviour
    {
        [Serializable]
        public struct WritingTabletContents
        {
            public LocalizedString title;
            public LocalizedString text;
        }

        public GameObject writingTablet;
        public List<WritingTabletContents> tabletTexts;

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
            for (int i = 0; i < 10; i++)
            {
                var x = SpawnWritingTablet();
                x.transform.position = GameConstants.Instance.Player.transform.position + Vector3.forward;
            }
        }
    }
}