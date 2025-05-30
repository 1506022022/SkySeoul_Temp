using System;
using UnityEngine;

namespace TopDown
{
    public class Selecter : MonoBehaviour, ISelectManager
    {
        public uint SelectedValue { get; private set; }

        public event Action<ISelectManager> OnSelect;

        public void SelectValue(int selectedValue)
        {
            SelectedValue = (uint)selectedValue;
            OnSelect?.Invoke(this);
        }
    }

}
