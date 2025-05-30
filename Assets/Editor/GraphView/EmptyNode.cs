public class EmptyNode : BaseNode
{
    public EmptyNode(int guid) : base(guid)
    {
    }

    protected override bool UseInput() => false;
    protected override bool UseOutput() => false;
}
