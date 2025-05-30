using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public abstract class BaseNode : Node
{
    private static Dictionary<string, Type> types;
    public static Dictionary<string, Type> Types
    {
        get
        {
            if (types == null)
            {
                var baseType = typeof(BaseNode);
                var childs = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => baseType.IsAssignableFrom(t)).ToArray();
                types = new Dictionary<string, Type>();
                foreach (var type in childs)
                {
                    types.Add(type.Name, type);
                }
            }
            return types;
        }
    }
    [SerializeField] private int guid;
    public int GUID { get => guid; protected set => guid = value; }
    [SerializeField] private string nodeType;
    public string NodeType { get => nodeType; protected set => nodeType = value; }
    [SerializeField] private Vector2 position;
    public Vector2 Position { get => position; set => position = value; }
    [SerializeField] private int inputNode;
    public int InputNode { get => inputNode; set => inputNode = value; }
    [SerializeField] private int outputNode;
    public int OutputNode { get => outputNode; set => outputNode = value; }
    public static int CreateGUID => DateTime.Now.Ticks.GetHashCode();
    public readonly Port InputPort;
    public readonly Port OutputPort;
    protected abstract bool UseInput();
    protected abstract bool UseOutput();

    public BaseNode(int guid)
    {
        GUID = guid;
        NodeType = GetType().Name;
        this.title = nodeType;

        if (UseInput())
        {
            InputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
            InputPort.portName = "In";
            inputContainer.Add(InputPort);
        }

        if (UseOutput())
        {
            OutputPort = GeneratePort(Direction.Output, Port.Capacity.Multi);
            OutputPort.portName = "Out";
            outputContainer.Add(OutputPort);
        }

        RefreshPorts();
        RefreshExpandedState();
    }
    public static BaseNode CreateNode(string nodeType)
    {
        BaseNode node = BaseNode.Types.TryGetValue(nodeType, out var type) ?
        (BaseNode)Activator.CreateInstance(type, BaseNode.CreateGUID) :
        new EmptyNode(BaseNode.CreateGUID);
        return node;
    }
    public virtual void SetDataAs(string data)
    {
        var node = JsonUtility.FromJson<EmptyNode>(data);
        GUID = node.GUID;
        NodeType = node.NodeType;
        Position = node.Position;
        InputNode = node.InputNode;
        OutputNode = node.OutputNode;
    }
    public virtual void SaveData()
    {
        Position = GetPosition().position;
        if (UseInput() && InputPort.connected && InputPort.connections.First().output.node is BaseNode inputNode)
        {
            InputNode = inputNode.GUID;
        }
        if (UseOutput() && OutputPort.connected && OutputPort.connections.First().input.node is BaseNode outputNode)
        {
            OutputNode = outputNode.GUID;
        }
    }
    private Port GeneratePort(Direction direction, Port.Capacity capacity)
    {
        return Port.Create<Edge>(Orientation.Horizontal, direction, capacity, typeof(float));
    }
}