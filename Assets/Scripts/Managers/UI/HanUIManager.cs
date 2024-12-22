using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanUIManager : BaseUIManager
{
    public GameObject hanEquipmentPrefab;

    protected override void Start()
    {
        base.Start();

        Instantiate(hanEquipmentPrefab, GameManager.Instance.inventoryCanvas.transform);

        dragIcon.transform.SetAsLastSibling();
    }

    public override void SetEquipmentItemUI()
    {
        HanEquipmentUI hanEquipmentUI = equipmentUI as HanEquipmentUI;
        HanInventoryManager hanInventoryManager = GameManager.Instance.inventoryManager as HanInventoryManager;

        hanEquipmentUI.gunSlot.SetImage(hanInventoryManager.gun);
        hanEquipmentUI.droneSlot.SetImage(hanInventoryManager.drone);
        hanEquipmentUI.armorSlot.SetImage(hanInventoryManager.armor);
        hanEquipmentUI.phoneSlot.SetImage(hanInventoryManager.phone);
        hanEquipmentUI.laptopSlot.SetImage(hanInventoryManager.laptop);
    }

    public override void SetConsumableItemUI()
    {
        HanEquipmentUI hanEquipmentUI = equipmentUI as HanEquipmentUI;
        HanInventoryManager hanInventoryManager = GameManager.Instance.inventoryManager as HanInventoryManager;

        for (int i = 0; i < 4; i++)
        {
            hanEquipmentUI.consumableSlots[i].SetImage(hanInventoryManager.quickSlot[i]);
        }
    }
}
