using System;
using UnityEngine;

namespace TopDown
{
    public class Selecter : MonoBehaviour, ISelect
    {
        public uint SelectedValue { get; private set; }

        public event Action<ISelect> OnSelect;

        public void SelectValue(int selectedValue)
        {
            SelectedValue = (uint)selectedValue;
            OnSelect?.Invoke(this);
        }
    }

}
