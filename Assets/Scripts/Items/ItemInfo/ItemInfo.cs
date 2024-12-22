using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInfo : ScriptableObject
{
    public int id;
    public ItemType itemType { get; protected set; }
    public string itemName;
    public int maxCount;
    public List<string> desciptions;

    public Sprite sprite;
    public GameObject itemPrefab;

    protected virtual void OnEnable()
    {
        if(sprite == null)
        {
            sprite = Resources.Load<Sprite>("Sprite/NoImage");
        }
    }

    public abstract Item CreateItem(ItemInfo _itemInfo, int count);
}

public abstract class EquipmentInfo : ItemInfo
{
    public EquipmentType equipmentType { get; protected set; }

    public EquipmentInfo()
    {
        itemType = ItemType.Equipment;
    }
}

public abstract class ConsumableInfo : ItemInfo
{
    public ConsumableType consumableType { get; protected set; }

    public ConsumableInfo()
    {
        itemType = ItemType.Consumable;
    }
}

[System.Serializable]
public class EnhancementInt
{
    public int increaseAmount;
    public List<RequireItem> requireItems;
}

[System.Serializable]
public class EnhancementFloat
{
    public float increaseAmount;
    public List<RequireItem> requireItems;
}

[System.Serializable]
public class RequireItem
{
    public int itemID;
    public int requireCount;
}


public class Item
{
    public ItemInfo itemInfo;
    public int currentCount;

    public Item(ItemInfo _itemInfo, int count)
    {
        itemInfo = _itemInfo;
        currentCount = count;
    }
}