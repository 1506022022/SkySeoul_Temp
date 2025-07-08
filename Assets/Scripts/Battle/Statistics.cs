using System;
using UnityEngine;

[Serializable]
public class Statistics
{
    public event Action<Statistics> OnValueChanged;
    [SerializeField, Min(1)] float maxValue;
    public float MaxValue
    {
        get => maxValue;
        set
        {
            maxValue = Mathf.Max(0, value);
            Value = Mathf.Min(this.value, maxValue);
        }
    }
    float value;
    public float Value
    {
        get => value;
        set
        {
            float clamped = Mathf.Clamp(value, 0, maxValue);
            if (Math.Abs(clamped - this.value) > 0.01f)
            {
                this.value = clamped;
                OnValueChanged?.Invoke(this);
            }
        }
    }
    public float Ratio => value / maxValue;

    public Statistics(float max)
    {
        Initialize(max, max);
    }

    public void Initialize(float value, float maxValue)
    {
        this.value = value;
        this.maxValue = maxValue;
    }
    public void Initialize()
    {
        Initialize(this.maxValue, this.maxValue);
    }
}