using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIManager : MonoBehaviour
{
    public GameObject itemsPrefab;
    public GameObject descriptionPrefab;
    public GameObject enhancementPrefab;

    public EquipmentUI equipmentUI;
    public ItemUI itemUI;
    public DescriptionsUI descriptionUI;
    public EnhancementUI enhancementUI;

    public GameObject dragIconPrefab;
    public GameObject dragIcon;

    public Canvas ICanvas;

    protected virtual void Start()
    {
        dragIcon = Instantiate(dragIconPrefab, GameManager.Instance.inventoryCanvas.transform);
        dragIcon.SetActive(false);

        Instantiate(itemsPrefab, GameManager.Instance.inventoryCanvas.transform);
        Instantiate(descriptionPrefab, GameManager.Instance.inventoryCanvas.transform);
        Instantiate(enhancementPrefab, GameManager.Instance.inventoryCanvas.transform);

        ICanvas = GameManager.Instance.inventoryCanvas.GetComponent<Canvas>();
        ICanvas.enabled = false;
    }

    public void SetInventoryItemUI()
    {
        for (int i = 0; i < 40; i++)
        {
            itemUI.EItemSlot[i].SetImage(GameManager.Instance.inventoryManager.Einventory[i]);
        }

        for (int i = 0; i < 40; i++)
        {
            itemUI.CItemSlot[i].SetImage(GameManager.Instance.inventoryManager.Cinventory[i]);
        }

        for (int i = 0; i < 40; i++)
        {
            itemUI.IItemSlot[i].SetImage(GameManager.Instance.inventoryManager.Iinventory[i]);
        }

        for (int i = 0; i < 40; i++)
        {
            itemUI.MItemSlot[i].SetImage(GameManager.Instance.inventoryManager.Minventory[i]);
        }
    }

    public abstract void SetEquipmentItemUI();

    public abstract void SetConsumableItemUI();

    public void ToggleInventory()
    {
        if (ICanvas.enabled)
        {
            GameManager.Instance.ViewCursor(false);
            GameManager.Instance.cinemachineFreeLook.enabled = true;
            GameManager.Instance.playerCharacter.GetComponent<BaseCharacter>().SetMove(true);
            ICanvas.enabled = false;

            descriptionUI.SetDescription(null);
        }
        else
        {
            GameManager.Instance.ViewCursor(true);
            GameManager.Instance.cinemachineFreeLook.enabled = false;
            GameManager.Instance.playerCharacter.GetComponent<BaseCharacter>().SetMove(false);
            ICanvas.enabled = true;
        }
    }
}
