using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BaseGraphView : GraphView
{
    private readonly EditorWindow editorWindow;
    private static readonly Vector3 center = new(Screen.width / 2f, Screen.height / 2f - 100f);

    public BaseGraphView(EditorWindow editorWindow)
    {
        this.editorWindow = editorWindow;
        style.flexGrow = 1;
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort == port ||
            startPort.node == port.node ||
            startPort.direction == port.direction)
                return;

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
    public void ClearGraph()
    {
        var edgesToRemove = edges.ToList();
        foreach (var edge in edgesToRemove)
        {
            RemoveElement(edge);
        }
        var nodesToRemove = nodes.ToList();
        foreach (var node in nodesToRemove)
        {
            RemoveElement(node);
        }
    }
    public void ResetGraph()
    {
        ClearGraph();
        AddNode(center, nameof(EntryNode));
    }
    public void AddNode(Vector2 position, string nodeType)
    {
        BaseNode node = BaseNode.CreateNode(nodeType);
        node.SetPosition(new Rect(position, new Vector2(200, 150)));
        AddElement(node);
    }
    public void SaveGraph(string path)
    {
        var list = new List<BaseNode>();
        foreach (var node in nodes)
        {
            if (node is BaseNode baseNode) list.Add(baseNode);
        }
        string jsonData = "";
        foreach (var node in list)
        {
            jsonData += string.IsNullOrEmpty(jsonData) ? "" : '\n';
            node.SaveData();
            jsonData += JsonUtility.ToJson(node);
        }
        File.WriteAllText(path, jsonData);
    }
    public void LoadGraph(string path)
    {
        ClearGraph();
        var baseNodes = new Dictionary<int, BaseNode>();

        foreach (var json in File.ReadLines(path))
        {
            var node = ReadNodeFromJson(json);
            node.SetPosition(new Rect(node.Position, new Vector2(200, 150)));
            baseNodes.Add(node.GUID, node);
        }

        foreach (var node in baseNodes.Values)
        {
            node.OutputNode = baseNodes.ContainsKey(node.OutputNode) ? node.OutputNode : 0;
            if (node.OutputNode == 0) continue;
            var edge = node.OutputPort.ConnectTo(baseNodes[node.OutputNode].InputPort);
            AddElement(edge);
        }
    }
    private BaseNode ReadNodeFromJson(string json)
    {
        BaseNode node;
        var readNode = JsonUtility.FromJson<EmptyNode>(json);
        var type = readNode.NodeType;
        node = BaseNode.CreateNode(type);
        node.SetDataAs(json);
        AddElement(node);
        return node;
    }
}