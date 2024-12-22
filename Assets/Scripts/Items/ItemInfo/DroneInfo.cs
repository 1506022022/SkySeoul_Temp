using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Drone", menuName = "Inventory/Drone", order = 101)]
public class DroneInfo : EquipmentInfo
{
    public float baseShield;

    public List<EnhancementFloat> shieldEnhancements;

    public DroneInfo()
    {
        equipmentType = EquipmentType.Drone;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new Drone(_itemInfo);
    }
}

public class Drone : Item
{
    public DroneInfo droneInfo;

    public int maxShieldEnhanceCount { get; private set; }
    public int currentShieldEnhanceCount { get; private set; }
    public float increaseShield { get; private set; }
    public float finalShield { get; private set; }

    public Drone(ItemInfo _itemInfo) : base(_itemInfo, 1)
    {
        droneInfo = _itemInfo as DroneInfo;
        if (droneInfo == null)
        {
            throw new System.InvalidCastException("Drone 생성자에 전달된 ItemInfo는 DroneInfo 타입이어야 합니다.");
        }

        currentShieldEnhanceCount = 0;
        increaseShield = 0.0f;

        finalShield = droneInfo.baseShield;

        if (droneInfo.shieldEnhancements != null)
            maxShieldEnhanceCount = droneInfo.shieldEnhancements.Count;
    }

    public void EnhanceShield()
    {
        Unequip();
        if (currentShieldEnhanceCount < maxShieldEnhanceCount)
        {
            increaseShield += droneInfo.shieldEnhancements[currentShieldEnhanceCount].increaseAmount;

            currentShieldEnhanceCount++;
        }

        finalShield = droneInfo.baseShield + increaseShield;
        Equip();
    }

    public void Equip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetSH(finalShield);
    }

    public void Unequip()
    {
        GameManager.Instance.playerCharacter.GetComponent<HanCharacter>().SetSH(-finalShield);
    }
}