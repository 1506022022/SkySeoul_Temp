using System.Collections.Generic;
using UnityEngine;

public class Field<T>
{
    public readonly int Radius;
    public readonly T[,] Array;
    public T DefaultValue;
    public Vector3 Pivot { get; private set; }

    public Field(int radius, Vector3 pivot)
    {
        Array = new T[radius * 2 + 1, radius * 2 + 1];
        Radius = radius;
        Pivot = pivot;
    }
    public void Reset()
    {
        foreach (var index in GetIndex())
        {
            Array[index.y, index.x] = DefaultValue;
        }
    }
    public List<Vector2Int> GetIndex()
    {
        List<Vector2Int> index = new();
        for (int i = 0; i < Radius * 2 + 1; i++)
        {
            for (int j = 0; j < Radius * 2 + 1; j++)
            {
                index.Add(new(j, i));
            }
        }
        return index;
    }
}
