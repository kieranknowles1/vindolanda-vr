using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.Localization;

namespace Vindolanda.Quest.Editor
{
    public class DialogueEditor
    {
        [MenuItem("Tools/Export Dialogue")]
        static void ExportDialogue()
        {
            string outPath = EditorUtility.SaveFilePanel("Export Dialogue to CSV", null, "dialogue.csv", "csv");
            using var file = File.OpenWrite(outPath);
            using var writer = new StreamWriter(file);
            writer.WriteLine("Dialogue,Voiced,Text");

            var allDialogue = ComponentExtensions.GetAllScriptableObjects<Dialogue>();
            HashSet<LocalizedString> seenLines = new();

            foreach (var dial in allDialogue)
            {
                var path = AssetDatabase.GetAssetPath(dial);

                foreach (var line in dial.Lines)
                {
                    if (seenLines.Contains(line.Text))
                    {
                        writer.WriteLine($"# Line {line.Text.GetLocalizedString()} is reused.");
                        continue;
                    }
                    seenLines.Add(line.Text);
                    writer.WriteLine($"{path},{!(line.Clip?.IsEmpty ?? true)},\"{line.Text.GetLocalizedString()}\"");
                }
            }
        }
    }
}