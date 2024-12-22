using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI descriptionText;

    [SerializeField] Image image;

    private Item currentItem;

    void Start()
    {
        GameManager.Instance.uiManager.descriptionUI = this;

        SetDescription(null);
    }

    public void SetDescription(Item item)
    {
        currentItem = item;

        if (item != null)
        {
            if (currentItem.currentCount <= 0)
            {
                currentItem = null;
                SetDescription();
                return;
            }

            nameText.text = item.itemInfo.itemName;
            countText.text = item.currentCount.ToString();
            typeText.text = item.itemInfo.itemType.ToString();
            descriptionText.text = string.Join("\n", item.itemInfo.desciptions);

            if (item.itemInfo.sprite == null)
            {
                image.sprite = null;
            }
            else
            {
                image.sprite = item.itemInfo.sprite;
            }
            Color _color = image.color;
            _color.a = 1.0f;
            image.color = _color;
        }
        else
        {
            nameText.text = string.Empty;
            countText.text = string.Empty;
            typeText.text = string.Empty;
            descriptionText.text = string.Empty;

            image.sprite = null;
            Color _color = image.color;
            _color.a = 0.0f;
            image.color = _color;
        }
    }

    public void SetDescription()
    {
        SetDescription(currentItem);
    }
}
