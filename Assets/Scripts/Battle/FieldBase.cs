using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FieldBase<T>
{
    public int Height;
    public int Width;
    public T[] Array;
    public Vector3 Pivot;

    public FieldBase(int height, int width, Vector3 pivot)
    {
        Width = width;
        Height = height;
        Array = new T[height * width];
        Pivot = pivot;
    }
    public void Reset()
    {
        System.Array.Fill(Array, default);
    }
    public List<Vector2Int> GetIndex()
    {
        List<Vector2Int> index = new();
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                index.Add(new(j, i));
            }
        }
        return index;
    }
}
