using TMPro;
using TopDown;
using UnityEngine;

public class MapView : MonoBehaviour
{
    public MapData MapData { get; private set; }
    [SerializeField] private SpriteRenderer mapPrecut;
    [SerializeField] private TextMeshProUGUI mapName;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SortingGroupPlus SortingGroup;
    public float MapPrecutWidth
    {
        get
        {
            return Mathf.Max(GetWidthOfPicture(background), GetWidthOfPicture(mapPrecut));
        }
    }
    public float MapPrecutHeight
    {
        get
        {
            return Mathf.Max(GetHeightOfPicture(background), GetHeightOfPicture(mapPrecut));
        }
    }
    private float GetWidthOfPicture(SpriteRenderer sp)
    {
        float result = 0f;
        if (sp != null && sp.sprite != null)
        {
            Vector2 spriteSize = sp.sprite.rect.size;
            Vector2 worldSize = spriteSize / sp.sprite.pixelsPerUnit;
            result = worldSize.x;
        }
        return result;
    }
    private float GetHeightOfPicture(SpriteRenderer sp)
    {
        float result = 0f;
        if (sp != null && sp.sprite != null)
        {
            Vector2 spriteSize = sp.sprite.rect.size;
            Vector2 worldSize = spriteSize / sp.sprite.pixelsPerUnit;
            result = worldSize.y;
        }
        return result;
    }
    public void SetOrderInLayer(int orderInLayer)
    {
        SortingGroup?.SetOrderInLayer(orderInLayer);
    }
    private void SetMapPrecut(string path)
    {
        mapPrecut.sprite = Resources.Load<Sprite>(path);
        if (mapPrecut.sprite == null) mapPrecut.sprite = Resources.Load<Sprite>("00000");
    }
    private void SetMapName(string name)
    {
        mapName.text = name ?? "??";
    }
    public void SetMapData(MapData mapData)
    {
        this.MapData = mapData;
        SetMapName(mapData.Name);
        SetMapPrecut(mapData.PrecutPath);
    }
}