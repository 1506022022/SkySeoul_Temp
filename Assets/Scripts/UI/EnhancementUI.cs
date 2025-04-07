using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancementUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;

    [SerializeField] Image equipmentImage;
    [SerializeField] Button enhanceButton;

    [SerializeField] Transform enhanceViewTransform;
    [SerializeField] Transform ingredientViewTransform;

    [SerializeField] GameObject EnhanceViewButtonPrefab;
    [SerializeField] GameObject IngredientViewSlotPrefab;

    private List<IngredientViewSlot> ingredientViewSlots = new List<IngredientViewSlot>();

    void Start()
    {
        GameManager.Instance.uiManager.enhancementUI = this;

        enhanceButton.interactable = false;
    }

    void FixedUpdate()
    {
        bool isRequiredMet = true;

        if (ingredientViewSlots.Count != 0)
        {
            foreach (IngredientViewSlot slot in ingredientViewSlots)
            {
                int count = GameManager.Instance.inventoryManager.CountItem(slot.id);
                int require = slot.requireCount;
                slot.requiredText.text = $"{count}/{require}";

                if (count < require)
                {
                    isRequiredMet = false;
                }
            }
        }
        else
        {
            isRequiredMet = false;
        }

        if (isRequiredMet)
        {
            enhanceButton.interactable = true;
        }
        else
        {
            enhanceButton.interactable = false;
        }
    }

    public void SetEnhance(Item item)
    {
        if (item != null)
        {
            if (item.itemInfo.itemType != ItemType.Equipment)
                return;

            nameText.text = item.itemInfo.itemName;

            if (item.itemInfo.sprite == null)
            {
                equipmentImage.sprite = null;
            }
            else
            {
                equipmentImage.sprite = item.itemInfo.sprite;
            }
            Color _color = equipmentImage.color;
            _color.a = 1.0f;
            equipmentImage.color = _color;
        }
        else
        {
            nameText.text = string.Empty;

            equipmentImage.sprite = null;
            Color _color = equipmentImage.color;
            _color.a = 0.0f;
            equipmentImage.color = _color;

            ClearEnhanceUI();
            return;
        }

        ClearEnhanceUI();

        switch ((item.itemInfo as EquipmentInfo).equipmentType)
        {
            case EquipmentType.Gun:

                Gun gun = item as Gun;

                GameObject attackButtonObj = Instantiate(EnhanceViewButtonPrefab, enhanceViewTransform);
                EnhanceViewButton attackEnhanceViewButton = attackButtonObj.GetComponent<EnhanceViewButton>();
                Button attackButton = attackButtonObj.GetComponent<Button>();

                if (gun.currentAttackEnhanceCount < gun.maxAttackEnhanceCount)
                {
                    attackEnhanceViewButton.gradeText.text = $"피해량\t{gun.currentAttackEnhanceCount} > <color=#00ddff>{(gun.currentAttackEnhanceCount + 1 >= gun.maxAttackEnhanceCount ? "MAX" : gun.currentAttackEnhanceCount + 1)}</color>";
                    attackEnhanceViewButton.amountText.text = $"{gun.finalAttack} > <color=#00ddff>{gun.finalAttack + gun.gunInfo.attackEnhancements[gun.currentAttackEnhanceCount].increaseAmount}</color>";

                    attackButton.onClick.AddListener(() => SetEnhanceUI(gun.gunInfo.attackEnhancements[gun.currentAttackEnhanceCount].requireItems));
                    attackButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => RemoveItems(gun.gunInfo.attackEnhancements[gun.currentAttackEnhanceCount].requireItems)));
                    attackButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(gun.EnhanceAttack));
                    attackButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => SetEnhance(item)));
                }
                else
                {
                    attackEnhanceViewButton.gradeText.text = $"피해량\tMAX";
                    attackEnhanceViewButton.amountText.text = $"{gun.finalAttack}";

                    attackButton.onClick.AddListener(() => SetEnhance(item));
                }

                GameObject magazineButtonObj = Instantiate(EnhanceViewButtonPrefab, enhanceViewTransform);
                EnhanceViewButton magazineEnhanceViewButton = magazineButtonObj.GetComponent<EnhanceViewButton>();
                Button magazineButton = magazineButtonObj.GetComponent<Button>();

                if (gun.currentMagazineEnhanceCount < gun.maxMagazineEnhanceCount)
                {
                    magazineEnhanceViewButton.gradeText.text = $"최대 탄창\t{gun.currentMagazineEnhanceCount} > <color=#00ddff>{(gun.currentMagazineEnhanceCount + 1 >= gun.maxMagazineEnhanceCount ? "MAX" : gun.currentMagazineEnhanceCount + 1)}</color>";
                    magazineEnhanceViewButton.amountText.text = $"{gun.finalMagazine} > <color=#00ddff>{gun.finalMagazine + gun.gunInfo.magazineEnhancements[gun.currentMagazineEnhanceCount].increaseAmount}</color>";

                    magazineButton.onClick.AddListener(() => SetEnhanceUI(gun.gunInfo.magazineEnhancements[gun.currentMagazineEnhanceCount].requireItems));
                    magazineButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => RemoveItems(gun.gunInfo.magazineEnhancements[gun.currentMagazineEnhanceCount].requireItems)));
                    magazineButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(gun.EnhanceMagazine));
                    magazineButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => SetEnhance(item)));
                }
                else
                {
                    magazineEnhanceViewButton.gradeText.text = $"최대 탄창\tMAX";
                    magazineEnhanceViewButton.amountText.text = $"{gun.finalMagazine}";

                    magazineButton.onClick.AddListener(() => SetEnhance(item));
                }

                GameObject firerateButtonObj = Instantiate(EnhanceViewButtonPrefab, enhanceViewTransform);
                EnhanceViewButton firerateEnhanceViewButton = firerateButtonObj.GetComponent<EnhanceViewButton>();
                Button firerateButton = firerateButtonObj.GetComponent<Button>();

                if (gun.currentFirerateEnhanceCount < gun.maxFirerateEnhanceCount)
                {
                    firerateEnhanceViewButton.gradeText.text = $"발사 속도\t{gun.currentFirerateEnhanceCount} > <color=#00ddff>{(gun.currentFirerateEnhanceCount + 1 >= gun.maxFirerateEnhanceCount ? "MAX" : gun.currentFirerateEnhanceCount + 1)}</color>";
                    firerateEnhanceViewButton.amountText.text = $"{gun.finalFirerate} > <color=#00ddff>{gun.finalFirerate + gun.gunInfo.firerateEnhancements[gun.currentFirerateEnhanceCount].increaseAmount}</color>";

                    firerateButton.onClick.AddListener(() => SetEnhanceUI(gun.gunInfo.firerateEnhancements[gun.currentFirerateEnhanceCount].requireItems));
                    firerateButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => RemoveItems(gun.gunInfo.firerateEnhancements[gun.currentFirerateEnhanceCount].requireItems)));
                    firerateButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(gun.EnhanceFirerate));
                    firerateButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => SetEnhance(item)));
                }
                else
                {
                    firerateEnhanceViewButton.gradeText.text = $"발사 속도\tMAX";
                    firerateEnhanceViewButton.amountText.text = $"{gun.finalFirerate}";

                    firerateButton.onClick.AddListener(() => SetEnhance(item));
                }


                break;
            case EquipmentType.Drone:

                Drone drone = item as Drone;

                GameObject shieldButtonObj = Instantiate(EnhanceViewButtonPrefab, enhanceViewTransform);
                EnhanceViewButton shieldEnhanceViewButton = shieldButtonObj.GetComponent<EnhanceViewButton>();
                Button shieldButton = shieldButtonObj.GetComponent<Button>();

                if (drone.currentShieldEnhanceCount < drone.maxShieldEnhanceCount)
                {
                    shieldEnhanceViewButton.gradeText.text = $"추가 실드\t{drone.currentShieldEnhanceCount} > <color=#00ddff>{(drone.currentShieldEnhanceCount + 1 >= drone.maxShieldEnhanceCount ? "MAX" : drone.currentShieldEnhanceCount + 1)}</color>";
                    shieldEnhanceViewButton.amountText.text = $"{drone.finalShield} > <color=#00ddff>{drone.finalShield + drone.droneInfo.shieldEnhancements[drone.currentShieldEnhanceCount].increaseAmount}</color>";

                    shieldButton.onClick.AddListener(() => SetEnhanceUI(drone.droneInfo.shieldEnhancements[drone.currentShieldEnhanceCount].requireItems));
                    shieldButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => RemoveItems(drone.droneInfo.shieldEnhancements[drone.currentShieldEnhanceCount].requireItems)));
                    shieldButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(drone.EnhanceShield));
                    shieldButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => SetEnhance(item)));
                }
                else
                {
                    shieldEnhanceViewButton.gradeText.text = $"추가 실드\tMAX";
                    shieldEnhanceViewButton.amountText.text = $"{drone.finalShield}";

                    shieldButton.onClick.AddListener(() => SetEnhance(item));
                }

                break;
            case EquipmentType.Armor:

                Armor armor = item as Armor;

                GameObject armorButtonObj = Instantiate(EnhanceViewButtonPrefab, enhanceViewTransform);
                EnhanceViewButton armorEnhanceViewButton = armorButtonObj.GetComponent<EnhanceViewButton>();
                Button armorButton = armorButtonObj.GetComponent<Button>();

                if (armor.currentHealthEnhanceCount < armor.maxHealthEnhanceCount)
                {
                    armorEnhanceViewButton.gradeText.text = $"추가 체력\t{armor.currentHealthEnhanceCount} > <color=#00ddff>{(armor.currentHealthEnhanceCount + 1 >= armor.maxHealthEnhanceCount ? "MAX" : armor.currentHealthEnhanceCount + 1)}</color>";
                    armorEnhanceViewButton.amountText.text = $"{armor.finalHealth} > <color=#00ddff>{armor.finalHealth + armor.armorInfo.healthEnhancements[armor.currentHealthEnhanceCount].increaseAmount}</color>";

                    armorButton.onClick.AddListener(() => SetEnhanceUI(armor.armorInfo.healthEnhancements[armor.currentHealthEnhanceCount].requireItems));
                    armorButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => RemoveItems(armor.armorInfo.healthEnhancements[armor.currentHealthEnhanceCount].requireItems)));
                    armorButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(armor.EnhanceHealth));
                    armorButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => SetEnhance(item)));
                }
                else
                {
                    armorEnhanceViewButton.gradeText.text = $"추가 체력\tMAX";
                    armorEnhanceViewButton.amountText.text = $"{armor.finalHealth}";

                    armorButton.onClick.AddListener(() => SetEnhance(item));
                }

                break;
            case EquipmentType.Phone:

                Phone phone = item as Phone;

                GameObject phoneButtonObj = Instantiate(EnhanceViewButtonPrefab, enhanceViewTransform);
                EnhanceViewButton phoneEnhanceViewButton = phoneButtonObj.GetComponent<EnhanceViewButton>();
                Button phoneButton = phoneButtonObj.GetComponent<Button>();

                if (phone.currentCostEnhanceCount < phone.maxCostEnhanceCount)
                {
                    phoneEnhanceViewButton.gradeText.text = $"추가 코스트\t{phone.currentCostEnhanceCount} > <color=#00ddff>{(phone.currentCostEnhanceCount + 1 >= phone.maxCostEnhanceCount ? "MAX" : phone.currentCostEnhanceCount + 1)}</color>";
                    phoneEnhanceViewButton.amountText.text = $"{phone.finalCost} > <color=#00ddff>{phone.finalCost + phone.phoneInfo.CostEnhancements[phone.currentCostEnhanceCount].increaseAmount}</color>";

                    phoneButton.onClick.AddListener(() => SetEnhanceUI(phone.phoneInfo.CostEnhancements[phone.currentCostEnhanceCount].requireItems));
                    phoneButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => RemoveItems(phone.phoneInfo.CostEnhancements[phone.currentCostEnhanceCount].requireItems)));
                    phoneButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(phone.EnhanceCost));
                    phoneButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => SetEnhance(item)));
                }
                else
                {
                    phoneEnhanceViewButton.gradeText.text = $"추가 코스트\tMAX";
                    phoneEnhanceViewButton.amountText.text = $"{phone.finalCost}";

                    phoneButton.onClick.AddListener(() => SetEnhance(item));
                }

                break;
            case EquipmentType.Laptop:

                Laptop laptop = item as Laptop;

                GameObject laptopButtonObj = Instantiate(EnhanceViewButtonPrefab, enhanceViewTransform);
                EnhanceViewButton laptopEnhanceViewButton = laptopButtonObj.GetComponent<EnhanceViewButton>();
                Button laptopButton = laptopButtonObj.GetComponent<Button>();

                if (laptop.currentHackperEnhanceCount < laptop.maxHackperEnhanceCount)
                {
                    laptopEnhanceViewButton.gradeText.text = $"해킹 시간 감소\t{laptop.currentHackperEnhanceCount} > <color=#00ddff>{(laptop.currentHackperEnhanceCount + 1 >= laptop.maxHackperEnhanceCount ? "MAX" : laptop.currentHackperEnhanceCount + 1)}</color>";
                    laptopEnhanceViewButton.amountText.text = $"{laptop.finalHackper} > <color=#00ddff>{laptop.finalHackper + laptop.laptopInfo.HackperEnhancements[laptop.currentHackperEnhanceCount].increaseAmount}</color>";

                    laptopButton.onClick.AddListener(() => SetEnhanceUI(laptop.laptopInfo.HackperEnhancements[laptop.currentHackperEnhanceCount].requireItems));
                    laptopButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => RemoveItems(laptop.laptopInfo.HackperEnhancements[laptop.currentHackperEnhanceCount].requireItems)));
                    laptopButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(laptop.EnhanceHackper));
                    laptopButton.onClick.AddListener(() => enhanceButton.onClick.AddListener(() => SetEnhance(item)));
                }
                else
                {
                    laptopEnhanceViewButton.gradeText.text = $"해킹 시간 감소\tMAX";
                    laptopEnhanceViewButton.amountText.text = $"{laptop.finalHackper}";

                    laptopButton.onClick.AddListener(() => SetEnhance(item));
                }

                break;
            default:
                break;
        }
    }

    public void SetEnhanceUI(List<RequireItem> requireItemList)
    {
        foreach (Transform child in ingredientViewTransform)
        {
            Destroy(child.gameObject);
        }
        ingredientViewSlots.Clear();
        enhanceButton.interactable = false;
        enhanceButton.onClick.RemoveAllListeners();

        foreach (RequireItem requireItem in requireItemList)
        {
            IngredientViewSlot slot = Instantiate(IngredientViewSlotPrefab, ingredientViewTransform).GetComponent<IngredientViewSlot>();
            slot.SetSlot(requireItem);

            ingredientViewSlots.Add(slot);

            ItemInfo targetInfo = GameManager.Instance.itemInfoList.FirstOrDefault(itemInfo => itemInfo.id == requireItem.itemID);

            if(targetInfo != null)
            {
                slot.image.sprite = targetInfo.sprite;
            }
            else
            {
                Debug.Log("강화 재료 중 해당 id의 정보가 없습니다.");
            }
        }

        if (requireItemList.Count == 0)
        {
            Debug.Log("강화 재료 정보가 비어있습니다.");
        }
    }

    public void ClearEnhanceUI()
    {
        foreach (Transform child in enhanceViewTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in ingredientViewTransform)
        {
            Destroy(child.gameObject);
        }

        ingredientViewSlots.Clear();

        enhanceButton.interactable = false;
        enhanceButton.onClick.RemoveAllListeners();
    }

    public void RemoveItems(List<RequireItem> requireItemList)
    {
        foreach (RequireItem requireItem in requireItemList)
        {
            GameManager.Instance.inventoryManager.RemoveItem(requireItem.itemID, requireItem.requireCount);
        }
    }
}
