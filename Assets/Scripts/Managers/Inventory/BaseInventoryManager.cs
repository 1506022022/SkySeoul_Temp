using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseInventoryManager : MonoBehaviour
{
    public Item[] Einventory;
    public Item[] Cinventory;
    public Item[] Iinventory;
    public Item[] Minventory;

    

    protected virtual void Start()
    {
        Einventory = new Item[40];
        Cinventory = new Item[40];
        Iinventory = new Item[40];
        Minventory = new Item[40];
    }

    public int GetItem(int id, int count)
    {
        ItemInfo itemInfo;
        itemInfo = GameManager.Instance.itemInfoList.FirstOrDefault(item => item.id == id);

        if (itemInfo == null)
        {
            Debug.Log("존재하지않는 ID 입니다.");
            return 0;
        }

        switch (itemInfo.itemType)
        {
            case ItemType.Equipment:
                {
                    for (int i = 0; i <= Einventory.Length; i++)
                    {
                        if (i >= Einventory.Length)
                        {
                            return 1;
                        }

                        if (Einventory[i] == null)
                        {
                            Einventory[i] = itemInfo.CreateItem(itemInfo, count);
                            break;
                        }
                    }
                }
                break;
            case ItemType.Consumable:
                for (int i = 0; i < Cinventory.Length; i++)
                {
                    if (Cinventory[i] != null && Cinventory[i].itemInfo.id == id)
                    {
                        int availableSpace = Cinventory[i].itemInfo.maxCount - Cinventory[i].currentCount;
                        int toAdd = (count < availableSpace) ? count : availableSpace;
                        Cinventory[i].currentCount += toAdd;
                        count -= toAdd;

                        if (count == 0)
                        {
                            GameManager.Instance.uiManager.SetInventoryItemUI();
                            return 0;
                        }
                    }
                }

                for (int i = 0; i <= Cinventory.Length; i++)
                {
                    if (i >= Cinventory.Length)
                    {
                        Debug.Log("full inventory");
                        GameManager.Instance.uiManager.SetInventoryItemUI();
                        return count;
                    }

                    if (Cinventory[i] == null)
                    {
                        Cinventory[i] = itemInfo.CreateItem(itemInfo, (count < itemInfo.maxCount) ? count : itemInfo.maxCount);
                        count -= Cinventory[i].currentCount;

                        if (count <= 0)
                        {
                            GameManager.Instance.uiManager.SetInventoryItemUI();
                            return 0;
                        }
                    }
                }

                if (count > 0)
                {
                    GetItem(id, count);
                }

                break;
            case ItemType.Ingredient:
                for (int i = 0; i < Iinventory.Length; i++)
                {
                    if (Iinventory[i] != null && Iinventory[i].itemInfo.id == id)
                    {
                        int availableSpace = Iinventory[i].itemInfo.maxCount - Iinventory[i].currentCount;
                        int toAdd = (count < availableSpace) ? count : availableSpace;
                        Iinventory[i].currentCount += toAdd;
                        count -= toAdd;

                        if (count == 0)
                        {
                            GameManager.Instance.uiManager.SetInventoryItemUI();
                            return 0;
                        }
                    }
                }

                for (int i = 0; i <= Iinventory.Length; i++)
                {
                    if (i >= Iinventory.Length)
                    {
                        Debug.Log("full inventory");
                        GameManager.Instance.uiManager.SetInventoryItemUI();
                        return count;
                    }

                    if (Iinventory[i] == null)
                    {
                        Iinventory[i] = itemInfo.CreateItem(itemInfo, (count < itemInfo.maxCount) ? count : itemInfo.maxCount);
                        count -= Iinventory[i].currentCount;

                        if (count <= 0)
                        {
                            GameManager.Instance.uiManager.SetInventoryItemUI();
                            return 0;
                        }
                    }
                }

                if (count > 0)
                {
                    GetItem(id, count);
                }


                break;
            case ItemType.Misc:
                for (int i = 0; i < Minventory.Length; i++)
                {
                    if (Minventory[i] != null && Minventory[i].itemInfo.id == id)
                    {
                        int availableSpace = Minventory[i].itemInfo.maxCount - Minventory[i].currentCount;
                        int toAdd = (count < availableSpace) ? count : availableSpace;
                        Minventory[i].currentCount += toAdd;
                        count -= toAdd;

                        if (count == 0)
                        {
                            GameManager.Instance.uiManager.SetInventoryItemUI();
                            return 0;
                        }
                    }
                }

                for (int i = 0; i <= Minventory.Length; i++)
                {
                    if (i >= Minventory.Length)
                    {
                        Debug.Log("full inventory");
                        GameManager.Instance.uiManager.SetInventoryItemUI();
                        return count;
                    }

                    if (Minventory[i] == null)
                    {
                        Minventory[i] = itemInfo.CreateItem(itemInfo, (count < itemInfo.maxCount) ? count : itemInfo.maxCount);
                        count -= Minventory[i].currentCount;

                        if (count <= 0)
                        {
                            GameManager.Instance.uiManager.SetInventoryItemUI();
                            return 0;
                        }
                    }
                }

                if (count > 0)
                {
                    GetItem(id, count);
                }
                break;
            default:
                break;
        }

        GameManager.Instance.uiManager.SetInventoryItemUI();
        return 0;
    }

    public bool RemoveItem(int id, int count)
    {
        if (count <= CountItem(id))
        {
            for (int i = Einventory.Length - 1; i >= 0; i--)
            {
                if (Einventory[i] != null && Einventory[i].itemInfo.id == id)
                {
                    int remain = count - Einventory[i].currentCount;
                    if (remain > 0)
                    {
                        count = remain;
                        Einventory[i].currentCount = 0;
                        Einventory[i] = null;
                    }
                    else
                    {
                        Einventory[i].currentCount -= count;
                        if (Einventory[i].currentCount == 0)
                        {
                            Einventory[i] = null;
                        }
                        break;
                    }
                }
            }

            for (int i = Cinventory.Length - 1; i >= 0; i--)
            {
                if (Cinventory[i] != null && Cinventory[i].itemInfo.id == id)
                {
                    int remain = count - Cinventory[i].currentCount;
                    if (remain > 0)
                    {
                        count = remain;
                        Cinventory[i].currentCount = 0;
                        Cinventory[i] = null;
                    }
                    else
                    {
                        Cinventory[i].currentCount -= count;
                        if (Cinventory[i].currentCount == 0)
                        {
                            Cinventory[i] = null;
                        }
                        break;
                    }
                }
            }

            for (int i = Iinventory.Length - 1; i >= 0; i--)
            {
                if (Iinventory[i] != null && Iinventory[i].itemInfo.id == id)
                {
                    int remain = count - Iinventory[i].currentCount;
                    if (remain > 0)
                    {
                        count = remain;
                        Iinventory[i].currentCount = 0;
                        Iinventory[i] = null;
                    }
                    else
                    {
                        Iinventory[i].currentCount -= count;
                        if (Iinventory[i].currentCount == 0)
                        {
                            Iinventory[i] = null;
                        }
                        break;
                    }
                }
            }

            for (int i = Minventory.Length - 1; i >= 0; i--)
            {
                if (Minventory[i] != null && Minventory[i].itemInfo.id == id)
                {
                    int remain = count - Minventory[i].currentCount;
                    if (remain > 0)
                    {
                        count = remain;
                        Minventory[i].currentCount = 0;
                        Minventory[i] = null;
                    }
                    else
                    {
                        Minventory[i].currentCount -= count;
                        if (Minventory[i].currentCount == 0)
                        {
                            Minventory[i] = null;
                        }
                        break;
                    }
                }
            }

            GameManager.Instance.uiManager.SetInventoryItemUI();
            GameManager.Instance.uiManager.descriptionUI.SetDescription();
            return true;
        }

        return false;
    }

    public int CountItem(int id)
    {
        int count = 0;

        foreach (Item item in Einventory)
        {
            if (item != null)
            {
                if (item.itemInfo.id == id)
                {
                    count += item.currentCount;
                }
            }
        }

        foreach (Item item in Cinventory)
        {
            if (item != null)
            {
                if (item.itemInfo.id == id)
                {
                    count += item.currentCount;
                }
            }
        }

        foreach (Item item in Iinventory)
        {
            if (item != null)
            {
                if (item.itemInfo.id == id)
                {
                    count += item.currentCount;
                }
            }
        }

        foreach (Item item in Minventory)
        {
            if (item != null)
            {
                if (item.itemInfo.id == id)
                {
                    count += item.currentCount;
                }
            }
        }

        return count;
    }

    public Item GetInventoryItem(ItemType itemType, int index)
    {
        Item item;

        switch (itemType)
        {
            case ItemType.Equipment:
                item = Einventory[index];
                break;
            case ItemType.Consumable:
                item = Cinventory[index];
                break;
            case ItemType.Ingredient:
                item = Iinventory[index];
                break;
            case ItemType.Misc:
                item = Minventory[index];
                break;
            default:
                item = null;
                break;
        }

        return item;
    }

    public void SwapInventoryItem(ItemType itemType, int currentSlotIndex, int targetSlotIndex)
    {
        switch (itemType)
        {
            case ItemType.Equipment:
                if (Einventory[targetSlotIndex] == null)
                {
                    Einventory[targetSlotIndex] = Einventory[currentSlotIndex];
                    Einventory[currentSlotIndex] = null;
                }
                else
                {
                    Item tempItem = Einventory[targetSlotIndex];
                    Einventory[targetSlotIndex] = Einventory[currentSlotIndex];
                    Einventory[currentSlotIndex] = tempItem;
                }
                break;
            case ItemType.Consumable:
                if (Cinventory[targetSlotIndex] == null)
                {
                    Cinventory[targetSlotIndex] = Cinventory[currentSlotIndex];
                    Cinventory[currentSlotIndex] = null;
                }
                else
                {
                    Item tempItem = Cinventory[targetSlotIndex];
                    Cinventory[targetSlotIndex] = Cinventory[currentSlotIndex];
                    Cinventory[currentSlotIndex] = tempItem;
                }
                break;
            case ItemType.Ingredient:
                if (Iinventory[targetSlotIndex] == null)
                {
                    Iinventory[targetSlotIndex] = Iinventory[currentSlotIndex];
                    Iinventory[currentSlotIndex] = null;
                }
                else
                {
                    Item tempItem = Iinventory[targetSlotIndex];
                    Iinventory[targetSlotIndex] = Iinventory[currentSlotIndex];
                    Iinventory[currentSlotIndex] = tempItem;
                }
                break;
            case ItemType.Misc:
                if (Minventory[targetSlotIndex] == null)
                {
                    Minventory[targetSlotIndex] = Minventory[currentSlotIndex];
                    Minventory[currentSlotIndex] = null;
                }
                else
                {
                    Item tempItem = Minventory[targetSlotIndex];
                    Minventory[targetSlotIndex] = Minventory[currentSlotIndex];
                    Minventory[currentSlotIndex] = tempItem;
                }
                break;
            default:
                break;
        }

        GameManager.Instance.uiManager.SetInventoryItemUI();
    }

    public abstract void Equip(Item item, int currentSlotIndex);
    public abstract void Equip(int currentSlotIndex, int quickSlotIndex);
    public abstract void Unequip(EquipmentType equipmentType);
    public abstract void Unequip(EquipmentType equipmentType, int targetSlotIndex);
    public abstract void Unequip(int quickSlotIndex);
    public abstract void Unequip(int quickSlotIndex, int targetSlotIndex);
    public abstract void SwapConsumableItem(int currentSlotIndex, int targetSlotIndex);
    public abstract Item GetEquipmentItem(EquipmentType equipmentType);
    public abstract Item GetConsumableItem(int quickSlotIndex);


    
}
