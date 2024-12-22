using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HanHUDManager : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider spBar;
    [SerializeField] private Slider shBar;

    [SerializeField] private Image[] slotImages;

    [SerializeField] private WheelUI[] wheelUIs;

    private HanCharacter hanCharacter;
    private HanInventoryManager inventoryManager;

    void Start()
    {
        StartCoroutine(InitHUD());
    }

    void Update()
    {
        SetBarUI();
        SetSlotUI();

    }

    private void SetBarUI()
    {
        if (hanCharacter == null)
            return;

        hpBar.value = hanCharacter.currentHP / hanCharacter.maxHP;
        spBar.value = hanCharacter.currentSP / hanCharacter.maxSP;
        shBar.value = hanCharacter.currentSH / hanCharacter.maxSH;
    }

    private void SetSlotUI()
    {
        if (inventoryManager == null)
            return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (inventoryManager.quickSlot[i] != null)
            {
                slotImages[i].sprite = inventoryManager.quickSlot[i].itemInfo.sprite;
                Color _color = slotImages[i].color;
                _color.a = 1.0f;
                slotImages[i].color = _color;
            }
            else
            {
                slotImages[i].sprite = null;
                Color _color = slotImages[i].color;
                _color.a = 0.0f;
                slotImages[i].color = _color;
            }
        }
    }

    public void SetWheelUI()
    {
        if (hanCharacter == null)
            return;
        if (inventoryManager == null)
            return;

        hanCharacter.SetHand();

        int equipCount = 0;

        if (inventoryManager.gun != null)
            equipCount++;
        if (inventoryManager.phone != null)
            equipCount++;
        if (inventoryManager.laptop != null)
            equipCount++;

        switch (equipCount)
        {
            case 0:
                wheelUIs[0].gameObject.SetActive(false);
                wheelUIs[1].gameObject.SetActive(false);
                wheelUIs[2].gameObject.SetActive(false);
                break;
            case 1:
                wheelUIs[0].gameObject.SetActive(true);
                wheelUIs[1].gameObject.SetActive(false);
                wheelUIs[2].gameObject.SetActive(false);

                switch (hanCharacter.currentHand)
                {
                    case 0:
                        wheelUIs[0].image.sprite = inventoryManager.gun.gunInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.gun.gunInfo.itemName;
                        wheelUIs[0].itemContent.text = "Åº¾à¼ö";
                        break;
                    case 1:
                        wheelUIs[0].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                        wheelUIs[0].itemContent.text = "";
                        break;
                    case 2:
                        wheelUIs[0].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                        wheelUIs[0].itemContent.text = "";
                        break;
                }
                break;
            case 2:
                wheelUIs[0].gameObject.SetActive(true);
                wheelUIs[1].gameObject.SetActive(true);
                wheelUIs[2].gameObject.SetActive(true);

                switch (hanCharacter.currentHand)
                {
                    case 0:
                        wheelUIs[0].image.sprite = inventoryManager.gun.gunInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.gun.gunInfo.itemName;
                        wheelUIs[0].itemContent.text = "Åº¾à¼ö";

                        if (inventoryManager.phone != null)
                        {
                            wheelUIs[1].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                            wheelUIs[1].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                            wheelUIs[1].itemContent.text = "";

                            wheelUIs[2].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                            wheelUIs[2].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                            wheelUIs[2].itemContent.text = "";
                        }
                        else
                        {
                            wheelUIs[1].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                            wheelUIs[1].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                            wheelUIs[1].itemContent.text = "";

                            wheelUIs[2].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                            wheelUIs[2].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                            wheelUIs[2].itemContent.text = "";
                        }
                        break;
                    case 1:
                        wheelUIs[0].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                        wheelUIs[0].itemContent.text = "";

                        if (inventoryManager.laptop != null)
                        {
                            wheelUIs[1].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                            wheelUIs[1].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                            wheelUIs[1].itemContent.text = "";

                            wheelUIs[2].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                            wheelUIs[2].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                            wheelUIs[2].itemContent.text = "";
                        }
                        else
                        {
                            wheelUIs[1].image.sprite = inventoryManager.gun.gunInfo.sprite;
                            wheelUIs[1].itemName.text = inventoryManager.gun.gunInfo.itemName;
                            wheelUIs[1].itemContent.text = "Åº¾à¼ö";

                            wheelUIs[2].image.sprite = inventoryManager.gun.gunInfo.sprite;
                            wheelUIs[2].itemName.text = inventoryManager.gun.gunInfo.itemName;
                            wheelUIs[2].itemContent.text = "Åº¾à¼ö";
                        }
                        break;
                    case 2:
                        wheelUIs[0].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                        wheelUIs[0].itemContent.text = "";

                        if (inventoryManager.gun != null)
                        {
                            wheelUIs[1].image.sprite = inventoryManager.gun.gunInfo.sprite;
                            wheelUIs[1].itemName.text = inventoryManager.gun.gunInfo.itemName;
                            wheelUIs[1].itemContent.text = "Åº¾à¼ö";

                            wheelUIs[2].image.sprite = inventoryManager.gun.gunInfo.sprite;
                            wheelUIs[2].itemName.text = inventoryManager.gun.gunInfo.itemName;
                            wheelUIs[2].itemContent.text = "Åº¾à¼ö";
                        }
                        else
                        {
                            wheelUIs[1].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                            wheelUIs[1].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                            wheelUIs[1].itemContent.text = "";

                            wheelUIs[2].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                            wheelUIs[2].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                            wheelUIs[2].itemContent.text = "";
                        }
                        break;
                }
                break;
            case 3:
                wheelUIs[0].gameObject.SetActive(true);
                wheelUIs[1].gameObject.SetActive(true);
                wheelUIs[2].gameObject.SetActive(true);

                switch (hanCharacter.currentHand)
                {
                    case 0:
                        wheelUIs[0].image.sprite = inventoryManager.gun.gunInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.gun.gunInfo.itemName;
                        wheelUIs[0].itemContent.text = "Åº¾à¼ö";

                        wheelUIs[1].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                        wheelUIs[1].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                        wheelUIs[1].itemContent.text = "";

                        wheelUIs[2].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                        wheelUIs[2].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                        wheelUIs[2].itemContent.text = "";
                        break;
                    case 1:
                        wheelUIs[0].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                        wheelUIs[0].itemContent.text = "";

                        wheelUIs[1].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                        wheelUIs[1].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                        wheelUIs[1].itemContent.text = "";

                        wheelUIs[2].image.sprite = inventoryManager.gun.gunInfo.sprite;
                        wheelUIs[2].itemName.text = inventoryManager.gun.gunInfo.itemName;
                        wheelUIs[2].itemContent.text = "Åº¾à¼ö";
                        break;
                    case 2:
                        wheelUIs[0].image.sprite = inventoryManager.laptop.laptopInfo.sprite;
                        wheelUIs[0].itemName.text = inventoryManager.laptop.laptopInfo.itemName;
                        wheelUIs[0].itemContent.text = "";

                        wheelUIs[1].image.sprite = inventoryManager.gun.gunInfo.sprite;
                        wheelUIs[1].itemName.text = inventoryManager.gun.gunInfo.itemName;
                        wheelUIs[1].itemContent.text = "Åº¾à¼ö";

                        wheelUIs[2].image.sprite = inventoryManager.phone.phoneInfo.sprite;
                        wheelUIs[2].itemName.text = inventoryManager.phone.phoneInfo.itemName;
                        wheelUIs[2].itemContent.text = "";
                        break;
                }
                break;
        }
    }

    private IEnumerator InitHUD()
    {
        yield return null;

        hanCharacter = GameManager.Instance.playerCharacter.GetComponent<BaseCharacter>() as HanCharacter;
        inventoryManager = GameManager.Instance.inventoryManager as HanInventoryManager;
    }
}
