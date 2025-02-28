using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UI_Default;

public class ManualUI : MonoBehaviour
{
    private float _width = 1920;
    private float _height = 1080;

    [SerializeField] private GameObject _canvasObj;
    [SerializeField] private GameObject _backgroundObj;
    [SerializeField] private UI_ManualData[] _manualData;
    [SerializeField] private GameObject[] _manualCase;

    private GameObject[] _manualList;
    private Transform _background;
    private UI_PageController _pageController;
    private int _pageIndex = 0;

    private void Start()
    {
        _manualList = new GameObject[_manualData.Length];

        _background = Instantiate(_backgroundObj).transform;
        _background.SetParent(_canvasObj.transform, false);
        
        _pageController = _background.GetComponent<UI_PageController>();

        _pageController.LeftButton.onClick.AddListener(OnLeftButtonClick);
        _pageController.RightButton.onClick.AddListener(OnRightButtonClick);
        _pageController.ExitButton.onClick.AddListener(OffManualUI);

        SetManualCase();

        OffManualUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void SizeRescaling()
    {
        float _rescalingSize = (Screen.width / _width > Screen.height / _height ? Screen.height / _height : Screen.width / _width);
        _width = Screen.width;
        _height = Screen.height;

        foreach (RectTransform rectTransform in _canvasObj.GetComponentsInChildren<RectTransform>())
        {
            UI_SizeRescaling(rectTransform, _rescalingSize);
        }
    }

    public void OnManualUI()
    {
        _pageIndex = 0;
        _canvasObj.SetActive(true);
        if (_manualList.Length == 1)
        {
            _pageController.LeftButton.gameObject.SetActive(false);
            _pageController.RightButton.gameObject.SetActive(false);
        }
        else if (_manualList.Length > 1)
        {
            _pageController.LeftButton.gameObject.SetActive(false);
            _pageController.RightButton.gameObject.SetActive(true);
        }
    }

    public void OffManualUI()
    {
        _canvasObj.SetActive(false);
    }

    public void OnLeftButtonClick()
    {
        if (_manualList.Last().activeSelf)
        {
            _pageController.RightButton.gameObject.SetActive(true);
        }

        _manualList[_pageIndex].gameObject.SetActive(false);
        _manualList[_pageIndex - 1].gameObject.SetActive(true);

        _pageIndex--;


        if (_manualList.First().activeSelf)
        {
            _pageController.LeftButton.gameObject.SetActive(false);
        }
    }

    public void OnRightButtonClick()
    {
        if (_manualList.First().activeSelf)
        {
            _pageController.LeftButton.gameObject.SetActive(true);
        }

        _manualList[_pageIndex].gameObject.SetActive(false);
        _manualList[_pageIndex + 1].gameObject.SetActive(true);

        _pageIndex++;

        if (_manualList.Last().activeSelf)
        {
            _pageController.RightButton.gameObject.SetActive(false);
        }
    }

    private void SetManualCase()
    {
        if (_manualData != null)
        {
            int i = 0;
            foreach (UI_ManualData _pageData in _manualData)
            {
                if (_pageData.Title == null)
                {
                    continue;
                }
                else if (_pageData.Description == "")
                {
                    _manualList[i] = Instantiate(_manualCase[0]);
                    _manualList[i].GetComponentInChildren<TextMeshProUGUI>().text = _pageData.Title;
                    _manualList[i].GetComponentInChildren<Image>().sprite = _pageData.Image;
                }
                else if (_pageData.Image == null)
                {
                    _manualList[i] = Instantiate(_manualCase[0]); 
                    _manualList[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = _pageData.Title;
                    _manualList[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = _pageData.Description;
                }
                else
                {
                    _manualList[i] = Instantiate(_manualCase[0]);
                    _manualList[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = _pageData.Title;
                    _manualList[i].GetComponentInChildren<Image>().sprite = _pageData.Image;
                    _manualList[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = _pageData.Description;
                }
                _manualList[i].transform.SetParent(_pageController.Page.transform, false);
                _manualList[i].SetActive(false);
                i++;
            }
        }
    }
}
