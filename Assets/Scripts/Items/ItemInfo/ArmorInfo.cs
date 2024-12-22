using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Armor", order = 102)]
public class ArmorInfo : EquipmentInfo
{
    public float baseHealth;

    public List<EnhancementFloat> healthEnhancements;

    public ArmorInfo()
    {
        equipmentType = EquipmentType.Armor;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new Armor(_itemInfo);
    }
}

public class Armor : Item
{
    public ArmorInfo armorInfo;

    public int maxHealthEnhanceCount { get; private set; }
    public int currentHealthEnhanceCount { get; private set; }
    public float increaseHealth { get; private set; }
    public float finalHealth { get; private set; }

    public Armor(ItemInfo _itemInfo) : base(_itemInfo, 1)
    {
        armorInfo = _itemInfo as ArmorInfo;
        if (armorInfo == null)
        {
            throw new System.InvalidCastException("Armor 생성자에 전달된 ItemInfo는 ArmorInfo 타입이어야 합니다.");
        }

        currentHealthEnhanceCount = 0;
        increaseHealth = 0.0f;

        finalHealth = armorInfo.baseHealth;

        if (armorInfo.healthEnhancements != null)
            maxHealthEnhanceCount = armorInfo.healthEnhancements.Count;
    }

    public void EnhanceHealth()
    {
        Unequip();
        if (currentHealthEnhanceCount < maxHealthEnhanceCount)
        {
            increaseHealth += armorInfo.healthEnhancements[currentHealthEnhanceCount].increaseAmount;

            currentHealthEnhanceCount++;
        }

        finalHealth = armorInfo.baseHealth + increaseHealth;
        Equip();
    }

    public void Equip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetHP(finalHealth);
    }

    public void Unequip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetHP(-finalHealth);
    }
}