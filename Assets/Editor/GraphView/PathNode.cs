using UnityEngine;
using UnityEngine.UIElements;

public class PathNode : BaseNode
{
    private readonly TextField pathUI;
    [SerializeField] private string path;
    public string Path { get => path; private set => path = value; }
    public PathNode(int guid) : base(guid)
    {
        {
            pathUI = new TextField("Path");
            pathUI.value = Path;
            pathUI.RegisterValueChangedCallback(evt =>
            {
                Path = evt.newValue;
            });
            pathUI.style.width = 200;
            mainContainer.Add(pathUI);
        }
        RefreshExpandedState();
        RefreshPorts();
    }
    protected override bool UseInput() => true;
    protected override bool UseOutput() => true;
    public override void SetDataAs(string data)
    {
        base.SetDataAs(data);
        var readNode = JsonUtility.FromJson<PathNode>(data);
        Path = readNode.Path;
        pathUI.value = Path;

    }
}
