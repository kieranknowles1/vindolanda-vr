using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(TMP_Dropdown))]
[Tooltip("Localize a TMP_Dropdown since Unity doesn't do it for us")]
public class LocalizeDropdown : MonoBehaviour
{
    [Serializable]
    public struct Entry
    {
        public LocalizedString text;
        public Sprite image;
    }

    public List<Entry> entries;

    private void Start()
    {
        var dropdown = GetComponent<TMP_Dropdown>();
        dropdown.options = entries.Select(e => new TMP_Dropdown.OptionData()
        {
            color = Color.white,
            image = e.image,
            text = e.text.GetLocalizedString()
        }).ToList();
    }
}
