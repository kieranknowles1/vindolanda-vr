using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
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

        static bool IsVoiced(Dialogue.Line line)
        {
            return line.Clip != null && !line.Clip.IsEmpty && line.Clip.LoadAsset() != null;
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
                    writer.WriteLine($"{path},{GetTableEntryKey(line.Text)},{IsVoiced(line)},\"{line.Text.GetLocalizedString()}\"");
                }
            }
        }

        const string AssetsTable = "AssetsTable";

        [MenuItem("Tools/Dialogue/Create Asset Entries")]
        static void CreateAssetEntries()
        {
            
            var assetsTable = LocalizationSettings.AssetDatabase.GetTable(AssetsTable).SharedData;

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
                        TableReference = AssetsTable,
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
                Debug.Log($"{nameof(CreateAssetEntries)}: Nothing to do");
        }

        [MenuItem("Tools/Dialogue/Import Dialogue")]
        static void ImportDialogueAssets()
        {
            CreateAssetEntries(); // We rely on these later
            var dialogues = GetAllDialogue()
                .SelectMany(dial => dial.Lines)
                .Where(line => !IsVoiced(line));
            var assetTable = LocalizationSettings.AssetDatabase.GetTable(AssetsTable); // Value for current language

            if (assetTable.LocaleIdentifier != "en")
            {
                Debug.LogWarning("Non-English languages are not currently supported");
            }

            var addressSettings = AddressableAssetSettingsDefaultObject.Settings;
            var addressGroup = addressSettings.FindGroup("Localization-Assets-English (en)");

            List<AddressableAssetEntry> added = new();
            foreach (var dialogue in dialogues)
            {
                var key = GetTableEntryKey(dialogue.Clip);
                var assets = AssetDatabase.FindAssets(key);
                if (assets.Length == 0)
                {
                    Debug.Log($"{key}: No assets found");
                    continue;
                }
                if (assets.Length > 1)
                {
                    Debug.Log($"{key}: Ambiguous assets found");
                    continue;
                }

                var path = AssetDatabase.GUIDToAssetPath(assets[0]);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null)
                {
                    Debug.Log($"{path} is not an AudioClip");
                    continue;
                }
                assetTable.AddEntry(key, assets[0]);
                var entry = addressSettings.CreateOrMoveEntry(assets[0], addressGroup, true);
                entry.address = key;
                entry.labels.Add($"Locale-{assetTable.LocaleIdentifier.Code}");
                added.Add(entry);
            }

            addressSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, added,postEvent: true);
            EditorUtility.SetDirty(assetTable);
        }
    }
}