using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class LobbyMode : IGameMode
    {
        public event Action OnQuit;
        public event Action OnLoaded;

        ModeSet modeSet;
        ISelect playerableCharacter;
        ISelect playMap;
        ISelect gameStart;
        ILoad loader;

        public void Load(ModeSet set)
        {
            var name = MapType.Lobby.ToString() + "Loader";
            loader = LoaderFactory.Load<ILoad>(name, name);
            var list = new List<Loader>();
            list.Add(new SceneLoader(MapType.Lobby.ToString()));
            list.Add(Loader<AudioClip, AudioClip>.GetLoader(nameof(AudioClip)));
            list.Add(Loader<GameObject, ParticleSystem>.GetLoader(nameof(ParticleSystem)));
            loader.Initialize(list);
            loader.Load();
            InitModeSet(set);
            InitSelecters();
        }
        void OnQuitMode()
        {
            loader?.Unload();
            LoaderFactory.Unload();
            OnQuit?.Invoke();
        }
        void InitModeSet(ModeSet set)
        {
            modeSet = set;
            modeSet.MapID = 0;
            modeSet.MapType = MapType.Lobby;
            modeSet.PlayableCharacterID = 0;
        }
        void InitSelecters()
        {
            playerableCharacter = LoaderFactory.Load<ISelect>(nameof(LobbyMode), nameof(playerableCharacter));
            playMap = LoaderFactory.Load<ISelect>(nameof(LobbyMode), nameof(playMap));
            gameStart = LoaderFactory.Load<ISelect>(nameof(LobbyMode), nameof(gameStart));

            if (playerableCharacter is null || playMap is null || gameStart is null)
            {
                OnQuitMode();
                return;
            }

            playerableCharacter.OnSelect += OnSelectPlayerableCharacter;
            playMap.OnSelect += OnSelectPlayMap;
            gameStart.OnSelect += OnSelectGameStart;
        }
        void OnSelectPlayerableCharacter(ISelect selecter)
        {
            modeSet.PlayableCharacterID = playerableCharacter.SelectedValue;
        }
        void OnSelectPlayMap(ISelect selecter)
        {
            modeSet.MapID = playMap.SelectedValue;
            modeSet.MapType = MapType.BattleMap;
        }
        void OnSelectGameStart(ISelect selecter)
        {
            OnQuitMode();
        }

    }
}