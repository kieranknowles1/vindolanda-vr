using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[XmlRoot("osm")]
public class OSMData
{
    public struct Bounds
    {
        [XmlAttribute("minlat")] public float minLat;
        [XmlAttribute("minlon")] public float minLon;
    }

    public class Node
    {
        [XmlAttribute("id")] public string id;
        [XmlAttribute("visible")] public bool visible;
        [XmlAttribute("lat")] public float x;
        [XmlAttribute("lon")] public float y;
    }

    public struct NodeRef
    {
        [XmlAttribute("ref")] public string id;
    }
    public struct Tag
    {
        [XmlAttribute("k")] public string key;
        [XmlAttribute("v")] public string value;
    }

    public struct Way
    {
        [XmlElement("nd")] public List<NodeRef> points;
        [XmlElement("tag")] public List<Tag> tags;

        public string GetTag(string key)
        {
            foreach (var tag in tags)
            {
                if (tag.key == key) return tag.value;
            }
            return null;
        }
    }

    [XmlAttribute("version")] public float version;
    [XmlAttribute("attribution")] public string attribution;

    [XmlElement("bounds")] public Bounds bounds;
    [XmlElement("node")] public List<Node> nodes;
    [XmlElement("way")] public List<Way> ways;

    public static OSMData Read(TextAsset asset, float scale = 2000)
    {
        XmlSerializer xml = new XmlSerializer(typeof(OSMData));
        var stream = new MemoryStream(asset.bytes);

        // Fixup data
        var data = (OSMData)xml.Deserialize(stream);
        foreach (var node in data.nodes)
        {
            node.x -= data.bounds.minLat;
            node.y -= data.bounds.minLon;
            node.x *= scale; node.y *= scale;
        }

        return data;
    }
}

class Line
{
    public bool modern;
    public List<Vector3> points = new List<Vector3>();
}

/// <summary>
/// Visualiser for OpenStreetMap datasets. Not the most efficient, but good enough for my needs
/// </summary>
public class OSMLoader : MonoBehaviour
{
    public enum VisibilityType
    {
        Always,
        Selected,
        Never
    }

    [SerializeField]
    TextAsset asset;

    public OSMData data;
    List<Line> lines = new List<Line>();

    public VisibilityType Visibility = VisibilityType.Always;
    public bool ShowModern = false;

    void ReadData()
    {
        data = OSMData.Read(asset);
        var nodes = new Dictionary<string, OSMData.Node>();
        foreach (var n in data.nodes) {
            nodes.Add(n.id, n);
        }

        foreach (var way in data.ways) {
            // OSM dataset is slightly inconsistent, but combining these two tags
            // filters out most modern structures with minimal false positive/negative in our case

            // Most of vindolanda and castle nick's exterior has this
            bool roman = way.GetTag("historic:civilization") == "ancient_roman";
            // Castle nick's interior is missing historic:civilization
            bool barrier = way.GetTag("barrier") != null;

            var line = new Line();
            line.modern = !(roman || barrier);
            foreach (var point in way.points)
            {
                var deref = nodes[point.id];
                // Remap to Unity's axis system
                line.points.Add(new Vector3(-deref.x, 0, deref.y));
            }
            lines.Add(line);
        }
    }

    private void DrawGizmos()
    {
        if (data == null) { ReadData(); }
        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var node in lines)
        {
            if (node.modern && !ShowModern) continue;
            for (int i = 1; i < node.points.Count; i++)
            {
                var prev = node.points[i - 1];
                var current = node.points[i];
                Gizmos.DrawLine(prev, current);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Visibility == VisibilityType.Always) DrawGizmos();
    }
    private void OnDrawGizmosSelected()
    {
        if (Visibility == VisibilityType.Selected) DrawGizmos();
    }
}
