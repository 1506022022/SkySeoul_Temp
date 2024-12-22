using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientViewSlot : MonoBehaviour
{
    public int id { get; private set; }
    public int requireCount { get; private set; }
    public Image image;
    public TextMeshProUGUI requiredText;

    public void SetSlot(RequireItem requireItem)
    {
        id = requireItem.itemID;
        requireCount = requireItem.requireCount;
    }
}
