using UnityEngine;
using UnityEngine.UI;

public class UI_Default
{
    public static GameObject UI_CreateImage(string _name, GameObject _parentObj, Image _image)
    {
        GameObject imageObj = new GameObject("Background");
        imageObj.transform.SetParent(_parentObj.transform); // �θ���
        Image image = imageObj.AddComponent<Image>();
        image.color = Color.black; // ���� ����
        return imageObj;
    }


    public static void UI_SizeRescaling(RectTransform UI, float RescalingSize)
    {
        UI.position *= RescalingSize;
        UI.sizeDelta *= RescalingSize;
        UI.localScale *= RescalingSize;
    }
}
