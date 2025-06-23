using System;

public interface ISelect
{
    public uint SelectedValue { get; }
    public event Action<ISelect> OnSelect;
}
