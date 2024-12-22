using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanInventoryManager : BaseInventoryManager
{
    public Gun gun = null;
    public Drone drone;
    public Armor armor;
    public Phone phone;
    public Laptop laptop;

    public Item[] quickSlot;

    protected override void Start()
    {
        base.Start();

        quickSlot = new Item[4];
    }

    public override void Equip(Item item, int currentSlotIndex)
    {
        switch(item.itemInfo.itemType)
        {
            case ItemType.Equipment:
                EquipmentInfo equipmentInfo = item.itemInfo as EquipmentInfo;

                switch (equipmentInfo.equipmentType)
                {
                    case EquipmentType.Gun:
                        if (gun != null)
                        {
                            Item tempItem = gun;
                            gun.Unequip();

                            gun = Einventory[currentSlotIndex] as Gun;
                            gun.Equip();

                            Einventory[currentSlotIndex] = tempItem;
                        }
                        else
                        {
                            gun = Einventory[currentSlotIndex] as Gun;
                            gun.Equip();

                            Einventory[currentSlotIndex] = null;
                        }
                        break;
                    case EquipmentType.Drone:
                        if (drone != null)
                        {
                            Item tempItem = drone;
                            drone.Unequip();

                            drone = Einventory[currentSlotIndex] as Drone;
                            drone.Equip();

                            Einventory[currentSlotIndex] = tempItem;
                        }
                        else
                        {
                            drone = Einventory[currentSlotIndex] as Drone;
                            drone.Equip();

                            Einventory[currentSlotIndex] = null;
                        }
                        break;
                    case EquipmentType.Armor:
                        if (armor != null)
                        {
                            Item tempItem = armor;
                            armor.Unequip();

                            armor = Einventory[currentSlotIndex] as Armor;
                            armor.Equip();

                            Einventory[currentSlotIndex] = tempItem;
                        }
                        else
                        {
                            armor = Einventory[currentSlotIndex] as Armor;
                            armor.Equip();

                            Einventory[currentSlotIndex] = null;
                        }
                        break;
                    case EquipmentType.Phone:
                        if (phone != null)
                        {
                            Item tempItem = phone;
                            phone.Unequip();

                            phone = Einventory[currentSlotIndex] as Phone;
                            phone.Equip();

                            Einventory[currentSlotIndex] = tempItem;
                        }
                        else
                        {
                            phone = Einventory[currentSlotIndex] as Phone;
                            phone.Equip();

                            Einventory[currentSlotIndex] = null;
                        }
                        break;
                    case EquipmentType.Laptop:
                        if (laptop != null)
                        {
                            Item tempItem = laptop;
                            laptop.Unequip();

                            laptop = Einventory[currentSlotIndex] as Laptop;
                            laptop.Equip();

                            Einventory[currentSlotIndex] = tempItem;
                        }
                        else
                        {
                            laptop = Einventory[currentSlotIndex] as Laptop;
                            laptop.Equip();

                            Einventory[currentSlotIndex] = null;
                        }
                        break;
                    default:
                        Debug.Log("장비할수 없습니다");
                        break;
                }
                break;
            case ItemType.Consumable:
                ConsumableInfo consumableInfo = item.itemInfo as ConsumableInfo;
                switch (consumableInfo.consumableType)
                {
                    case ConsumableType.HealSelf:
                    case ConsumableType.HealRobot:
                    case ConsumableType.Granade:

                        for (int i = 0; i < 4; i++)
                        {
                            if (quickSlot[i] == null)
                            {
                                quickSlot[i] = Cinventory[currentSlotIndex];
                                Cinventory[currentSlotIndex] = null;
                                break;
                            }
                        }

                        break;
                    default:
                        Debug.Log("장비할수 없습니다");
                        break;
                }
                break;
            default:
                Debug.Log("장비할수 없습니다");
                break;
        }

        HanUIManager hanUIManager = GameManager.Instance.uiManager as HanUIManager;
        hanUIManager.SetInventoryItemUI();
        hanUIManager.SetEquipmentItemUI();
        hanUIManager.SetConsumableItemUI();
        GameManager.Instance.hudManager.SetWheelUI();
    }

    public override void Equip(int currentSlotIndex, int quickSlotIndex)
    {
        switch((GetInventoryItem(ItemType.Consumable, currentSlotIndex).itemInfo as ConsumableInfo).consumableType)
        {
            case ConsumableType.HealSelf:
            case ConsumableType.HealRobot:
            case ConsumableType.Granade:
                if (quickSlot[quickSlotIndex] != null)
                {
                    Item tempItem = quickSlot[quickSlotIndex];
                    quickSlot[quickSlotIndex] = Cinventory[currentSlotIndex];
                    Cinventory[currentSlotIndex] = tempItem;
                }
                else
                {
                    quickSlot[quickSlotIndex] = Cinventory[currentSlotIndex];
                    Cinventory[currentSlotIndex] = null;
                }
                break;
            default:
                break;
        }

        HanUIManager hanUIManager = GameManager.Instance.uiManager as HanUIManager;
        hanUIManager.SetInventoryItemUI();
        hanUIManager.SetEquipmentItemUI();
        hanUIManager.SetConsumableItemUI();
        GameManager.Instance.hudManager.SetWheelUI();
    }

    public override void Unequip(EquipmentType equipmentType)
    {
        switch(equipmentType)
        {
            case EquipmentType.Gun:
                for (int i = 0; i < Einventory.Length; i++)
                {
                    if (Einventory[i] == null)
                    {
                        gun.Unequip();
                        Einventory[i] = gun;
                        gun = null;
                        break;
                    }
                }
                break;
            case EquipmentType.Drone:
                for (int i = 0; i < Einventory.Length; i++)
                {
                    if (Einventory[i] == null)
                    {
                        drone.Unequip();
                        Einventory[i] = drone;
                        drone = null;
                        break;
                    }
                }
                break;
            case EquipmentType.Armor:
                for (int i = 0; i < Einventory.Length; i++)
                {
                    if (Einventory[i] == null)
                    {
                        armor.Unequip();
                        Einventory[i] = armor;
                        armor = null;
                        break;
                    }
                }
                break;
            case EquipmentType.Phone:
                for (int i = 0; i < Einventory.Length; i++)
                {
                    if (Einventory[i] == null)
                    {
                        phone.Unequip();
                        Einventory[i] = phone;
                        phone = null;
                        break;
                    }
                }
                break;
            case EquipmentType.Laptop:
                for (int i = 0; i < Einventory.Length; i++)
                {
                    if (Einventory[i] == null)
                    {
                        laptop.Unequip();
                        Einventory[i] = laptop;
                        laptop = null;
                        break;
                    }
                }
                break;
            default:
                break;
        }

        HanUIManager hanUIManager = GameManager.Instance.uiManager as HanUIManager;
        hanUIManager.SetInventoryItemUI();
        hanUIManager.SetEquipmentItemUI();
        hanUIManager.SetConsumableItemUI();
        GameManager.Instance.hudManager.SetWheelUI();
    }

    public override void Unequip(EquipmentType equipmentType, int targetSlotIndex)
    {
        switch (equipmentType)
        {
            case EquipmentType.Gun:
                if (Einventory[targetSlotIndex] != null)
                {
                    if ((Einventory[targetSlotIndex].itemInfo as EquipmentInfo).equipmentType == equipmentType)
                    {
                        Equip(Einventory[targetSlotIndex], targetSlotIndex);
                    }
                }
                else
                {
                    gun.Unequip();
                    Einventory[targetSlotIndex] = gun;
                    gun = null;
                }
                break;
            case EquipmentType.Drone:
                if (Einventory[targetSlotIndex] != null)
                {
                    if ((Einventory[targetSlotIndex].itemInfo as EquipmentInfo).equipmentType == equipmentType)
                    {
                        Equip(Einventory[targetSlotIndex], targetSlotIndex);
                    }
                }
                else
                {
                    drone.Unequip();
                    Einventory[targetSlotIndex] = drone;
                    drone = null;
                }
                break;
            case EquipmentType.Armor:
                if (Einventory[targetSlotIndex] != null)
                {
                    if ((Einventory[targetSlotIndex].itemInfo as EquipmentInfo).equipmentType == equipmentType)
                    {
                        Equip(Einventory[targetSlotIndex], targetSlotIndex);
                    }
                }
                else
                {
                    armor.Unequip();
                    Einventory[targetSlotIndex] = armor;
                    armor = null;
                }
                break;
            case EquipmentType.Phone:
                if (Einventory[targetSlotIndex] != null)
                {
                    if ((Einventory[targetSlotIndex].itemInfo as EquipmentInfo).equipmentType == equipmentType)
                    {
                        Equip(Einventory[targetSlotIndex], targetSlotIndex);
                    }
                }
                else
                {
                    phone.Unequip();
                    Einventory[targetSlotIndex] = phone;
                    phone = null;
                }
                break;
            case EquipmentType.Laptop:
                if (Einventory[targetSlotIndex] != null)
                {
                    if ((Einventory[targetSlotIndex].itemInfo as EquipmentInfo).equipmentType == equipmentType)
                    {
                        Equip(Einventory[targetSlotIndex], targetSlotIndex);
                    }
                }
                else
                {
                    laptop.Unequip();
                    Einventory[targetSlotIndex] = laptop;
                    laptop = null;
                }
                break;
            default:
                break;
        }

        HanUIManager hanUIManager = GameManager.Instance.uiManager as HanUIManager;
        hanUIManager.SetInventoryItemUI();
        hanUIManager.SetEquipmentItemUI();
        hanUIManager.SetConsumableItemUI();
        GameManager.Instance.hudManager.SetWheelUI();
    }

    public override void Unequip(int quickSlotIndex)
    {
        for (int i = 0; i < Cinventory.Length; i++)
        {
            if (Cinventory[i] == null)
            {
                Cinventory[i] = quickSlot[quickSlotIndex];
                quickSlot[quickSlotIndex] = null;
                break;
            }
        }

        HanUIManager hanUIManager = GameManager.Instance.uiManager as HanUIManager;
        hanUIManager.SetInventoryItemUI();
        hanUIManager.SetEquipmentItemUI();
        hanUIManager.SetConsumableItemUI();
    }

    public override void Unequip(int quickSlotIndex, int targetSlotIndex)
    {
        if (GetInventoryItem(ItemType.Consumable, targetSlotIndex) != null)
        {
            switch ((GetInventoryItem(ItemType.Consumable, targetSlotIndex).itemInfo as ConsumableInfo).consumableType)
            {
                case ConsumableType.HealSelf:
                case ConsumableType.HealRobot:
                case ConsumableType.Granade:
                    Item tempItem = Cinventory[targetSlotIndex];
                    Cinventory[targetSlotIndex] = quickSlot[quickSlotIndex];
                    quickSlot[quickSlotIndex] = tempItem;
                    break;
                default:
                    break;
            }
        }
        else
        {
            Cinventory[targetSlotIndex] = quickSlot[quickSlotIndex];
            quickSlot[quickSlotIndex] = null;
        }

        

        HanUIManager hanUIManager = GameManager.Instance.uiManager as HanUIManager;
        hanUIManager.SetInventoryItemUI();
        hanUIManager.SetEquipmentItemUI();
        hanUIManager.SetConsumableItemUI();
    }

    public override void SwapConsumableItem(int currentSlotIndex, int targetSlotIndex)
    {
        Item tempItem = quickSlot[currentSlotIndex];
        quickSlot[currentSlotIndex] = quickSlot[targetSlotIndex];
        quickSlot[targetSlotIndex] = tempItem;

        HanUIManager hanUIManager = GameManager.Instance.uiManager as HanUIManager;
        hanUIManager.SetInventoryItemUI();
        hanUIManager.SetEquipmentItemUI();
        hanUIManager.SetConsumableItemUI();
    }

    public override Item GetEquipmentItem(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Gun:
                return gun;
            case EquipmentType.Drone:
                return drone;
            case EquipmentType.Armor:
                return armor;
            case EquipmentType.Phone:
                return phone;
            case EquipmentType.Laptop:
                return laptop;
            default:
                return null;
        }
    }

    public override Item GetConsumableItem(int quickSlotIndex)
    {
        return quickSlot[quickSlotIndex];
    }
}
