using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Phone", menuName = "Inventory/Phone", order = 103)]
public class PhoneInfo : EquipmentInfo
{
    public int baseCost;

    public List<EnhancementInt> CostEnhancements;

    public PhoneInfo()
    {
        equipmentType = EquipmentType.Phone;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new Phone(_itemInfo);
    }
}

public class Phone : Item
{
    public PhoneInfo phoneInfo;

    public int maxCostEnhanceCount { get; private set; }
    public int currentCostEnhanceCount { get; private set; }
    public int increaseCost { get; private set; }
    public int finalCost { get; private set; }

    public Phone(ItemInfo _itemInfo) : base(_itemInfo, 1)
    {
        phoneInfo = _itemInfo as PhoneInfo;
        if (phoneInfo == null)
        {
            throw new System.InvalidCastException("Phone 생성자에 전달된 ItemInfo는 PhoneInfo 타입이어야 합니다.");
        }

        currentCostEnhanceCount = 0;
        increaseCost = 0;

        finalCost = phoneInfo.baseCost;

        if (phoneInfo.CostEnhancements != null)
            maxCostEnhanceCount = phoneInfo.CostEnhancements.Count;
    }

    public void EnhanceCost()
    {
        Unequip();
        if (currentCostEnhanceCount < maxCostEnhanceCount)
        {
            increaseCost += phoneInfo.CostEnhancements[currentCostEnhanceCount].increaseAmount;

            currentCostEnhanceCount++;
        }

        finalCost = phoneInfo.baseCost + increaseCost;
        Equip();
    }

    public void Equip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetCost(finalCost);
    }

    public void Unequip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetCost(-finalCost);
    }
}