using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class BranchPathNode : BaseNode
{
    public string True;
    public string False;
    private readonly TextField trueUI;
    private readonly TextField falseUI;

    public BranchPathNode(int guid) : base(guid)
    {
        {
            trueUI = new TextField("True");
            trueUI.value = True;
            trueUI.RegisterValueChangedCallback(evt =>
            {
                True = evt.newValue;
            });
            trueUI.style.width = 200;
            mainContainer.Add(trueUI);
        }

        {
            falseUI = new TextField("False");
            falseUI.value = False;
            falseUI.RegisterValueChangedCallback(evt =>
            {
                False = evt.newValue;
            });
            falseUI.style.width = 200;
            mainContainer.Add(falseUI);
        }

        RefreshExpandedState();
        RefreshPorts();
    }
    public override void SetDataAs(string json)
    {
        base.SetDataAs(json);
        var readNode = JsonUtility.FromJson<BranchPathNode>(json);
        True = readNode.True;
        trueUI.value = True;
        False = readNode.False;
        falseUI.value = False;
    }
    protected override bool UseInput() => true;
    protected override bool UseOutput() => true;
}