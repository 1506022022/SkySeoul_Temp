using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Misc", menuName = "Inventory/Misc", order = 400)]
public class MiscInfo : ItemInfo
{
    public MiscInfo()
    {
        itemType = ItemType.Misc;
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new Misc(_itemInfo, count);
    }
}

public class Misc : Item
{
    public MiscInfo miscInfo;

    public Misc(ItemInfo _itemInfo, int count) : base(_itemInfo, count)
    {
        miscInfo = _itemInfo as MiscInfo;
        if (miscInfo == null)
        {
            throw new System.InvalidCastException("Misc 생성자에 전달된 ItemInfo는 MiscInfo 타입이어야 합니다.");
        }
    }
}