using System;

public interface ISelectManager
{
    public uint SelectedValue { get; }
    public event Action<ISelectManager> OnSelect;
}
