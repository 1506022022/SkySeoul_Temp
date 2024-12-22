using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public GameObject[] itemViews = new GameObject[4];

    public ItemSlot[] EItemSlot = new ItemSlot[40];
    public ItemSlot[] CItemSlot = new ItemSlot[40];
    public ItemSlot[] IItemSlot = new ItemSlot[40];
    public ItemSlot[] MItemSlot = new ItemSlot[40];

    void Start()
    {
        GameManager.Instance.uiManager.itemUI = this;

        foreach (GameObject view in itemViews)
        {
            view.GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f;
        }

        StartCoroutine(SetVisible());
    }

    private IEnumerator SetVisible()
    {
        yield return null;

        itemViews[1].SetActive(false);
        itemViews[2].SetActive(false);
        itemViews[3].SetActive(false);
    }
}
