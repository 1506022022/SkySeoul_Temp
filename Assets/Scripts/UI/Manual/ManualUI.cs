using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UI_Default;

public class ManualUI : MonoBehaviour
{
    private float _width = 1920;
    private float _height = 1080;

    [SerializeField] private GameObject _canvasObj;
    [SerializeField] private GameObject _backgroundObj;
    [SerializeField] private GameObject[] _manualCases;

    private Transform _background;
    private UI_PageController _pageController;
    private int _pageIndex = 0;

    private void Start()
    {
        _background = Instantiate(_backgroundObj).transform;
        _background.SetParent(_canvasObj.transform, false);
        
        _pageController = _background.GetComponent<UI_PageController>();

        SetManualCase();

        _pageController.LeftButton.onClick.AddListener(OnLeftButtonClick);
        _pageController.RightButton.onClick.AddListener(OnRightButtonClick);
        _pageController.ExitButton.onClick.AddListener(OffManualUI);

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
        _canvasObj.SetActive(true);
        if (_manualCases.Length == 1)
        {
            _pageController.LeftButton.gameObject.SetActive(false);
            _pageController.RightButton.gameObject.SetActive(false);
        }
        else if (_manualCases.Length > 1)
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
        if (_manualCases.Last().activeSelf)
        {
            _pageController.RightButton.gameObject.SetActive(true);
        }

        _manualCases[_pageIndex].gameObject.SetActive(false);
        _manualCases[_pageIndex - 1].gameObject.SetActive(true);

        _pageIndex--;


        if (_manualCases.First().activeSelf)
        {
            _pageController.LeftButton.gameObject.SetActive(false);
        }
    }

    public void OnRightButtonClick()
    {
        if (_manualCases.Last().activeSelf)
        {
            _pageController.LeftButton.gameObject.SetActive(true);
        }

        _manualCases[_pageIndex].gameObject.SetActive(false);
        _manualCases[_pageIndex + 1].gameObject.SetActive(true);

        _pageIndex++;

        if (_manualCases.Last().activeSelf)
        {
            _pageController.RightButton.gameObject.SetActive(false);
        }
    }

    private void SetManualCase()
    {
        if (_manualCases != null)
        {
            int i = 0;
            foreach (GameObject page in _manualCases)
            {
                _manualCases[i] = Instantiate(page);
                _manualCases[i].transform.SetParent(_pageController.Page.transform, false);
                _manualCases[i].SetActive(false);
                i++;
            }
        }
    }
}
