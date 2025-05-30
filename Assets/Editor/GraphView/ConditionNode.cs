using UnityEngine;

public class ConditionNode : BaseNode
{
    public ConditionNode(int guid) : base(guid)
    {
    }

    protected override bool UseInput() => false;

    protected override bool UseOutput() => true;
}
