using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Inventory/Ingredient", order = 300)]
public class IngredientInfo : ItemInfo
{
    public IngredientInfo()
    {
        itemType = ItemType.Ingredient;
    }

    public override Item CreateItem(ItemInfo _itemInfo, int count)
    {
        return new Ingredient(_itemInfo, count);
    }
}

public class Ingredient : Item
{
    public IngredientInfo ingredientInfo;

    public Ingredient(ItemInfo _itemInfo, int count) : base(_itemInfo, count)
    {
        ingredientInfo = _itemInfo as IngredientInfo;
        if (ingredientInfo == null)
        {
            throw new System.InvalidCastException("Ingredient 생성자에 전달된 ItemInfo는 IngredientInfo 타입이어야 합니다.");
        }

    }
}