using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Vindolanda.Quest.Editor
{
    public static class DialogueEditor
    {
        static string GetTableEntryKey(LocalizedString str)
        {
            var table = LocalizationSettings.StringDatabase.GetTable(str.TableReference);
            return str.TableEntryReference.ResolveKeyName(table.SharedData);
        }

        static string GetTableEntryKey<T>(LocalizedAsset<T> asset) where T : Object
        {
            var table = LocalizationSettings.AssetDatabase.GetTable(asset.TableReference);
            return asset.TableEntryReference.ResolveKeyName(table.SharedData);
        }

        static List<Dialogue> GetAllDialogue()
        {
            return ComponentExtensions.GetAllScriptableObjects<Dialogue>();
        }

        [MenuItem("Tools/Dialogue/Export Dialogue")]
        static void ExportDialogue()
        {
            string outPath = EditorUtility.SaveFilePanel("Export Dialogue to CSV", null, "dialogue.csv", "csv");
            using var file = File.OpenWrite(outPath);
            using var writer = new StreamWriter(file);
            writer.WriteLine("Dialogue,ID,Voiced,Text");

            var allDialogue = GetAllDialogue();
            HashSet<LocalizedString> seenLines = new();

            foreach (var dial in allDialogue)
            {
                var path = AssetDatabase.GetAssetPath(dial);

                foreach (var line in dial.Lines)
                {
                    if (seenLines.Contains(line.Text))
                    {
                        Debug.LogWarning($"# Line {line.Text.GetLocalizedString()} is reused. This is not supported.");
                        continue;
                    }
                    seenLines.Add(line.Text);
                    writer.WriteLine($"{path},{GetTableEntryKey(line.Text)},{!(line.Clip?.IsEmpty ?? true)},\"{line.Text.GetLocalizedString()}\"");
                }
            }
        }

        [MenuItem("Tools/Dialogue/Create Asset Entries")]
        static void CreateAssetEntries()
        {
            const string ASSETS_TABLE = "AssetsTable";
            var assetsTable = LocalizationSettings.AssetDatabase.GetTable(ASSETS_TABLE).SharedData;

            bool touchedAny = false;
            foreach (var dial in GetAllDialogue())
            {
                bool touched = false;
                foreach (var line in dial.Lines)
                {
                    if (line.Clip.TableEntryReference.ReferenceType != TableEntryReference.Type.Empty) continue;
                    Debug.Log($"Creating asset for {line.Text}");

                    var key = GetTableEntryKey(line.Text);
                    assetsTable.AddKey(key);
                    line.Clip = new()
                    {
                        TableReference = ASSETS_TABLE,
                        TableEntryReference = key
                    };
                    touched = true;
                    touchedAny = true;
                }
                // We need to use SetDirty as Undo.RecordObject only applies to scene objects
                // This forces the editor to save these objects
                if (touched) EditorUtility.SetDirty(dial);
            }

            if (touchedAny)
                EditorUtility.SetDirty(assetsTable);
            else
                Debug.Log("Nothing to do");
        }
    }
}