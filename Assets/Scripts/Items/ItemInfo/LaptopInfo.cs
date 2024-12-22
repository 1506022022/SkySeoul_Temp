using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Laptop", menuName = "Inventory/Laptop", order = 104)]
public class LaptopInfo : EquipmentInfo
{
    public float baseHackper;

    public List<EnhancementFloat> HackperEnhancements;

    public LaptopInfo()
    {
        equipmentType = EquipmentType.Laptop;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new Laptop(_itemInfo);
    }
}

public class Laptop : Item
{
    public LaptopInfo laptopInfo;

    public int maxHackperEnhanceCount { get; private set; }
    public int currentHackperEnhanceCount { get; private set; }
    public float increaseHackper { get; private set; }
    public float finalHackper { get; private set; }

    public Laptop(ItemInfo _itemInfo) : base(_itemInfo, 1)
    {
        laptopInfo = _itemInfo as LaptopInfo;
        if (laptopInfo == null)
        {
            throw new System.InvalidCastException("Laptop 생성자에 전달된 ItemInfo는 LaptopInfo 타입이어야 합니다.");
        }

        currentHackperEnhanceCount = 0;
        increaseHackper = 0.0f;

        finalHackper = laptopInfo.baseHackper;

        if (laptopInfo.HackperEnhancements != null)
            maxHackperEnhanceCount = laptopInfo.HackperEnhancements.Count;
    }

    public void EnhanceHackper()
    {
        Unequip();
        if (currentHackperEnhanceCount < maxHackperEnhanceCount)
        {
            increaseHackper += laptopInfo.HackperEnhancements[currentHackperEnhanceCount].increaseAmount;

            currentHackperEnhanceCount++;
        }

        finalHackper = laptopInfo.baseHackper + increaseHackper;
        Equip();
    }

    public void Equip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetHackper(finalHackper);
    }

    public void Unequip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetHackper(-finalHackper);
    }
}