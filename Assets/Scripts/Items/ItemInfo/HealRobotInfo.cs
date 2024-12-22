using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealRobot", menuName = "Inventory/HealRobot", order = 201)]
public class HealRobotInfo : ConsumableInfo
{
    public HealRobotInfo()
    {
        consumableType = ConsumableType.HealRobot;
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new HealRobot(_itemInfo, count);
    }
}

public class HealRobot : Item
{
    public HealRobotInfo healRobotInfo;

    public HealRobot(ItemInfo _itemInfo, int count) : base(_itemInfo, count)
    {
        healRobotInfo = _itemInfo as HealRobotInfo;
        if (healRobotInfo == null)
        {
            throw new System.InvalidCastException("HealRobot 생성자에 전달된 ItemInfo는 HealRobotInfo 타입이어야 합니다.");
        }
    }

    public void Set()
    {

    }

    public void Unset()
    {

    }
}