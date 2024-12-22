using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemType itemType;
    public int slotIndex;

    private bool isItemExist;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetImage(Item item)
    {
        if (item != null)
        {
            if (item.itemInfo.sprite != null)
            {
                image.sprite = item.itemInfo.sprite;
            }
            else
            {
                image.sprite = null;
            }
            Color _color = image.color;
            _color.a = 1.0f;
            image.color = _color;

            isItemExist = true;
        }
        else
        {
            image.sprite = null;
            Color _color = image.color;
            _color.a = 0.0f;
            image.color = _color;

            isItemExist = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isItemExist)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.uiManager.descriptionUI.SetDescription(GameManager.Instance.inventoryManager.GetInventoryItem(itemType, slotIndex));
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            switch (itemType)
            {
                case ItemType.Equipment:
                case ItemType.Consumable:
                    GameManager.Instance.inventoryManager.Equip(GameManager.Instance.inventoryManager.GetInventoryItem(itemType, slotIndex), slotIndex);
                    break;
                default:
                    break;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isItemExist)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.uiManager.dragIcon.GetComponent<Image>().sprite = GameManager.Instance.inventoryManager.GetInventoryItem(itemType, slotIndex).itemInfo.sprite;
            GameManager.Instance.uiManager.dragIcon.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isItemExist)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.uiManager.dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isItemExist)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.uiManager.dragIcon.SetActive(false);

            // Slot 베이스 클래스에서 enum으로 switch문 돌렸으면 또 괜찮을거 같은데
            ItemSlot itemSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>();
            EquipmentSlot equipmentSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<EquipmentSlot>();
            ConsumableSlot consumableSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ConsumableSlot>();

            if (itemSlot)
            {
                GameManager.Instance.inventoryManager.SwapInventoryItem(itemType, slotIndex, eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>().slotIndex);
            }
            else if (equipmentSlot)
            {
                if (equipmentSlot.equipmentType == (GameManager.Instance.inventoryManager.GetInventoryItem(itemType, slotIndex).itemInfo as EquipmentInfo).equipmentType)
                {
                    GameManager.Instance.inventoryManager.Equip(GameManager.Instance.inventoryManager.GetInventoryItem(itemType, slotIndex), slotIndex);
                }
            }
            else if (consumableSlot)
            {
                GameManager.Instance.inventoryManager.Equip(slotIndex, consumableSlot.slotIndex);
            }
        }
    }
}
