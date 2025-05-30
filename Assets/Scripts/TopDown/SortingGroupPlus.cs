using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class SortingGroupPlus : MonoBehaviour
{
    [SerializeField] private SortingGroup sortingGroup;
    private List<Canvas> canvas;
    private Dictionary<Canvas, int> originLayers;

    private void Initialized()
    {
        canvas = GetComponentsInChildren<Canvas>().ToList();
        originLayers = new();
        for (int i = 0; i < canvas.Count; i++)
        {
            originLayers.Add(canvas[i], canvas[i].sortingOrder);
        }
    }
    public void SetOrderInLayer(int orderInLayer)
    {
        if (originLayers == null || canvas == null) Initialized();
        if (originLayers == null || canvas == null || sortingGroup == null) return;
        sortingGroup.sortingOrder = orderInLayer;
        for (int i = 0; i < canvas.Count; i++)
        {
            canvas[i].sortingOrder = originLayers[canvas[i]] + orderInLayer;
        }
    }
}
