using System;
using UnityEngine;

[ExecuteAlways]
public class MarginRatioFitter : MonoBehaviour
{
    [Range(0, 1)] public float left;
    [Range(0, 1)] public float right;
    [Range(0, 1)] public float height;
    [Range(0, 1)] public float posY;

    private float preRight;
    private float preLeft;
    private RectTransform rt;

    private float screenWidth;
    private float screenHeight;

    private void OnValidate()
    {
        ClampMargins();
        ResizeRectTransform();
    }

    private void Update()
    {
        if (Screen.width != screenWidth || screenHeight != Screen.height)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            ResizeRectTransform();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (Screen.width != screenWidth || screenHeight != Screen.height)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            ResizeRectTransform();
        }
    }
    private void ClampMargins()
    {
        if (preRight != right)
        {
            right = Mathf.Min(1 - left, right);
            preRight = right;
        }
        if (preLeft != left)
        {
            left = Mathf.Min(1 - right, left);
            preLeft = left;
        }
    }
    private void ResizeRectTransform()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        if (rt == null) return;
        var root = rt.parent.GetComponent<RectTransform>();

        float rectWidth = rt.rect.width;
        float rectHeight = rt.rect.height;

        if (root != null && rt.anchorMin.x != rt.anchorMax.x)
        {
            rectWidth = root.rect.width;
            rectHeight = root.rect.height;
        }
        if (root != null && rt.anchorMin.y! != rt.anchorMax.y)
        {
            rectWidth = root.rect.width;
            rectHeight = root.rect.height;
        }

        float leftMargin = rectWidth * left;
        float rightMargin = rectWidth * right;
        float heightMargin = rectHeight * height;
        float posYMargin = rectHeight * posY;

        rt.offsetMin = new(leftMargin, rt.offsetMin.y);
        rt.offsetMax = new(-rightMargin, rt.offsetMax.y);
        rt.sizeDelta = new(rt.sizeDelta.x, heightMargin);

        var anchoredPos = rt.anchoredPosition;
        anchoredPos.y = posYMargin;
        rt.anchoredPosition = anchoredPos;

    }
}
