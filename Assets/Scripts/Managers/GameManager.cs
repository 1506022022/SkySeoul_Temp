using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public HanHUDManager hudManager { get; private set; }
    public BaseInventoryManager inventoryManager { get; private set; }
    public BaseUIManager uiManager { get; private set; }

    private CharacterType characterType;

    public CinemachineFreeLook cinemachineFreeLook;

    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject hanPrefab;
    [SerializeField] GameObject hanHUDPrefab;
    [SerializeField] GameObject hanInventoryManagerPrefab;
    [SerializeField] GameObject hanUIManagerPrefab;

    public GameObject playerCharacter { get; private set; }

    public GameObject hudCanvas;
    public GameObject inventoryCanvas;
    public List<ItemInfo> itemInfoList;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        characterType = PlayerData.Instance().characterType;
    }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    void Start()
    {
        if (hudCanvas.activeSelf == false)
            hudCanvas.SetActive(true);

        if (inventoryCanvas.activeSelf == false)
            inventoryCanvas.SetActive(true);

        switch (characterType)
        {
            case CharacterType.Han:

                playerCharacter = Instantiate(hanPrefab, spawnPoint);
                playerCharacter.transform.parent = null;
                playerCharacter.AddComponent<HanController>();

                cinemachineFreeLook.Follow = playerCharacter.transform;
                cinemachineFreeLook.LookAt = playerCharacter.transform;

                hudManager = Instantiate(hanHUDPrefab, hudCanvas.transform).GetComponent<HanHUDManager>();
                inventoryManager = Instantiate(hanInventoryManagerPrefab, inventoryCanvas.transform).GetComponent<HanInventoryManager>();
                uiManager = Instantiate(hanUIManagerPrefab, inventoryCanvas.transform).GetComponent<HanUIManager>();

                break;
            case CharacterType.Jo:
                break;
            case CharacterType.Other:
                break;
            default:
                break;
        }

        ViewCursor(false);
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ViewCursor(bool value)
    {
        if (value)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
