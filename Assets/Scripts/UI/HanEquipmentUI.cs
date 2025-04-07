using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HanEquipmentUI : EquipmentUI
{
    [Header("UI Switch Button")]
    public Button itemUIButton;
    public Button enhanceUIButton;

    [Header("Equipment Slot UI")]
    public EquipmentSlot gunSlot;
    public EquipmentSlot droneSlot;
    public EquipmentSlot armorSlot;
    public EquipmentSlot phoneSlot;
    public EquipmentSlot laptopSlot;

    [Header("Consumable Slot UI")]
    public ConsumableSlot[] consumableSlots;

    void Start()
    {
        GameManager.Instance.uiManager.equipmentUI = this;

        StartCoroutine(SetButton());
    }

    public IEnumerator SetButton()
    {
        yield return null;

        GameObject itemUIObj = GameManager.Instance.uiManager.itemUI.gameObject;
        GameObject descriptionUIObj = GameManager.Instance.uiManager.descriptionUI.gameObject;
        GameObject enhanceUIObj = GameManager.Instance.uiManager.enhancementUI.gameObject;

        itemUIButton.onClick.AddListener(() => itemUIObj.SetActive(true));
        itemUIButton.onClick.AddListener(() => descriptionUIObj.SetActive(true));
        itemUIButton.onClick.AddListener(() => enhanceUIObj.SetActive(false));
        itemUIButton.onClick.AddListener(() => GameManager.Instance.uiManager.enhancementUI.SetEnhance(null));

        enhanceUIButton.onClick.AddListener(() => itemUIObj.SetActive(false));
        enhanceUIButton.onClick.AddListener(() => descriptionUIObj.SetActive(false));
        enhanceUIButton.onClick.AddListener(() => enhanceUIObj.SetActive(true));

        enhanceUIObj.SetActive(false);
    }
}
