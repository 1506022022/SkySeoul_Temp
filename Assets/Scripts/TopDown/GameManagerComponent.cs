using UnityEngine;

namespace TopDown
{
    public class GameManagerComponent : MonoBehaviour
    {
        private ModeSet modeSet;
        private IGameMode gameMode;
        private ILoadManager loadManager;
        [SerializeField] private MapType OnPlayMode = MapType.Lobby;

        private bool IsNull(object item, string name)
        {
            if (item == null)
            {
                Debug.Log($"{DM_ERROR.NOT_FOUND} {name}");
                ApplicationControl.QuitGame();
            }
            return item == null;
        }
        private void Awake()
        {
            InitioalizeModeSet();
            SetGameMode();
        }
        private void InitioalizeModeSet()
        {
            modeSet = Resources.Load<ModeSet>(nameof(ModeSet));
            if (IsNull(modeSet, nameof(modeSet))) return;
            modeSet.MapType = OnPlayMode;
        }
        private void SetGameMode()
        {
            if (IsNull(modeSet, nameof(modeSet))) return;
            gameMode = GameModeFactory.CreateMode(modeSet.MapType);

            if (IsNull(gameMode, nameof(gameMode))) return;
            gameMode.OnQuit += OnExitGameMode;

            loadManager = UIManagerFactory.CreateUIManager(modeSet.MapType);
            if (IsNull(loadManager, nameof(loadManager))) return;
            loadManager.Load();
            loadManager.OnLoaded += OnLoaded;
        }
        private void OnLoaded()
        {
            loadManager.OnLoaded -= OnLoaded;

            if (IsNull(gameMode, nameof(gameMode))) return;
            gameMode.Initialize(modeSet);
        }
        private void ClearGameMode()
        {
            gameMode.OnQuit -= OnExitGameMode;
            gameMode = null;
        }
        private void UnloadResource()
        {
            loadManager.Unload();
        }
        private void OnExitGameMode()
        {
            UnloadResource();
            ClearGameMode();
            SetGameMode();
        }
    }
}
