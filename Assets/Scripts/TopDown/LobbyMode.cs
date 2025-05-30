using System;

namespace TopDown
{
    public class LobbyMode : IGameMode
    {
        public event Action OnQuit;
        private ModeSet modeSet;
        private MyLoader<ISelectManager> resources;
        private ISelectManager playerableCharacterSelecter;
        private ISelectManager playMapSelecter;
        private ISelectManager gameStartSelecter;

        public void Initialize(ModeSet set)
        {
            SetQuitEvent();
            InitModeSet(set);
            InitSelecters();
        }
        private void SetQuitEvent()
        {
            OnQuit += OnQuitMode;
        }
        private void OnQuitMode()
        {
            resources.Unload();
        }
        private void InitModeSet(ModeSet set)
        {
            modeSet = set;
            modeSet.MapID = 0;
            modeSet.MapType = MapType.Lobby;
            modeSet.PlayableCharacterID = 0;
        }
        private void InitSelecters()
        {
            resources = new();
            playerableCharacterSelecter = resources.Load(nameof(LobbyMode), nameof(playerableCharacterSelecter));
            playMapSelecter = resources.Load(nameof(LobbyMode), nameof(playMapSelecter));
            gameStartSelecter = resources.Load(nameof(LobbyMode), nameof(gameStartSelecter));

            if (playerableCharacterSelecter == null || playMapSelecter == null || gameStartSelecter == null)
            {
                OnQuit?.Invoke();
                return;
            }

            playerableCharacterSelecter.OnSelect += OnSelectPlayerableCharacter;
            playMapSelecter.OnSelect += OnSelectPlayMap;
            gameStartSelecter.OnSelect += OnSelectGameStart;
        }
        private void OnSelectPlayerableCharacter(ISelectManager selecter)
        {
            modeSet.PlayableCharacterID = playerableCharacterSelecter.SelectedValue;
        }
        private void OnSelectPlayMap(ISelectManager manager)
        {
            modeSet.MapID = playMapSelecter.SelectedValue;
            modeSet.MapType = MapType.BattleMap;
        }
        private void OnSelectGameStart(ISelectManager manager)
        {
            OnQuit?.Invoke();
        }

    }
}
