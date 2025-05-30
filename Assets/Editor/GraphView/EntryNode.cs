using UnityEngine;

public class EntryNode : BaseNode
{
    public EntryNode(int guid = 0) : base(0)
    {
    }

    protected override bool UseInput() => false;

    protected override bool UseOutput() => true;
}
