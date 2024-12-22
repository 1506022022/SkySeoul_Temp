using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Inventory/Gun", order = 100)]
public class GunInfo : EquipmentInfo
{
    public int baseAttack;
    public int baseMagazine;
    public float baseFirerate;

    public List<EnhancementInt> attackEnhancements;
    public List<EnhancementInt> magazineEnhancements;
    public List<EnhancementFloat> firerateEnhancements;

    public GunInfo()
    {
        equipmentType = EquipmentType.Gun;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new Gun(_itemInfo);
    }
}

public class Gun : Item
{
    public GunInfo gunInfo;
    public int currentMagazine;

    public int maxAttackEnhanceCount { get; private set; }
    public int currentAttackEnhanceCount { get; private set; }
    public int increaseAttack { get; private set; }
    public int finalAttack { get; private set; }

    public int maxMagazineEnhanceCount { get; private set; }
    public int currentMagazineEnhanceCount { get; private set; }
    public int increaseMagazine { get; private set; }
    public int finalMagazine { get; private set; }

    public int maxFirerateEnhanceCount { get; private set; }
    public int currentFirerateEnhanceCount { get; private set; }
    public float increaseFirerate { get; private set; }
    public float finalFirerate { get; private set; }


    public Gun(ItemInfo _itemInfo) : base(_itemInfo, 1)
    {
        gunInfo = _itemInfo as GunInfo;
        if (gunInfo == null)
        {
            throw new System.InvalidCastException("Gun 생성자에 전달된 ItemInfo는 GunInfo 타입이어야 합니다.");
        }

        currentMagazine = 0;


        currentAttackEnhanceCount = 0;
        increaseAttack = 0;

        currentMagazineEnhanceCount = 0;
        increaseMagazine = 0;

        currentFirerateEnhanceCount = 0;
        increaseFirerate = 0.0f;

        finalAttack = gunInfo.baseAttack;
        finalMagazine = gunInfo.baseMagazine;
        finalFirerate = gunInfo.baseFirerate;

        if (gunInfo.attackEnhancements != null)
            maxAttackEnhanceCount = gunInfo.attackEnhancements.Count;

        if (gunInfo.magazineEnhancements != null)
            maxMagazineEnhanceCount = gunInfo.magazineEnhancements.Count;

        if (gunInfo.firerateEnhancements != null)
            maxFirerateEnhanceCount = gunInfo.firerateEnhancements.Count;
    }

    public void EnhanceAttack()
    {
        if (currentAttackEnhanceCount < maxAttackEnhanceCount)
        {
            increaseAttack += gunInfo.attackEnhancements[currentAttackEnhanceCount].increaseAmount;

            currentAttackEnhanceCount++;
        }

        finalAttack = gunInfo.baseAttack + increaseAttack;
    }

    public void EnhanceMagazine()
    {
        if (currentMagazineEnhanceCount < maxMagazineEnhanceCount)
        {
            increaseMagazine += gunInfo.attackEnhancements[currentMagazineEnhanceCount].increaseAmount;

            currentMagazineEnhanceCount++;
        }

        finalMagazine = gunInfo.baseMagazine + increaseMagazine;
    }

    public void EnhanceFirerate()
    {
        Unequip();
        if (currentFirerateEnhanceCount < maxFirerateEnhanceCount)
        {
            increaseFirerate += gunInfo.firerateEnhancements[currentFirerateEnhanceCount].increaseAmount;

            currentFirerateEnhanceCount++;
        }

        finalFirerate = gunInfo.baseFirerate + increaseFirerate;
        Equip();
    }

    public void Equip()
    {
        
    }

    public void Unequip()
    {
        
    }

    public void Fire()
    {

    }

    public void Reload()
    {

    }
}