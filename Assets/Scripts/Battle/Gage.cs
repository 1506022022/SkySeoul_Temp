using System;
using UnityEngine;

namespace Battle
{
    public class Gage
    {
        public event Action OnEmpty;
        float value;
        public float Value
        {
            get => value;
            set
            {
                if (this.value == value) return;
                this.value = Mathf.Max(value, 0);
                if (this.value == 0f) OnEmpty.Invoke();
            }
        }
        public Gage(float max) { value = max; }
    }

}
