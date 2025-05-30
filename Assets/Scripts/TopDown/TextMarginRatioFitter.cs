using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TextMarginRatioFitter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ui;
    [Range(0, 10000)] public int height;
    private RectTransform rt;

    private float screenWidth;
    private float screenHeight;

    private void OnValidate()
    {
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
    private void ResizeRectTransform()
    {
        if (ui == null) return;
        if (rt == null) rt = GetComponent<RectTransform>();
        if (rt == null) return;

        var rectHeight = rt.rect.height;
        ui.fontSize = rectHeight * height / 100f;
    }
}
